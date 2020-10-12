using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using RWCoreLib.Log;
using RWCoreNetwork.NetPacket;


namespace RWCoreNetwork.NetService
{
    public class NetClientService : NetBaseService
    {
        public ClientSession ClientSession { get; set; }

        // 네트워크 상태 큐
        Queue<ClientSession> _netEventQueue;

        

        public NetClientService(PacketHandler packetHandler, ILog logger, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring)
            : base(packetHandler, logger, bufferSize, keepAliveTime, keepAliveInterval, onMonitoring)
        {
            _netEventQueue = new Queue<ClientSession>();

            ClientSession = new ClientSession(_logger, _bufferSize);
            ClientSession.CompletedMessageCallback += OnMessageCompleted;
        }


        /// <summary>
        /// N개의 클라이언트를 서버에 연결한다.
        /// </summary>
        /// <param name="serverAddr"></param>
        /// <param name="port"></param>
        public void Connect(string serverAddr, int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.LingerState = new LingerOption(true, 10);
            socket.NoDelay = true;


            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), port);

            bool pendiing = socket.ConnectAsync(args);
            if (pendiing == false)
            {
                OnConnectCompleted(socket, args);
            }
        }


        /// <summary>
        /// 서버와 연결 성공했다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Socket socket = sender as Socket;


                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                receiveArgs.SetBuffer(new byte[_bufferSize], 0, _bufferSize);
                receiveArgs.UserToken = ClientSession;

                SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                sendArgs.SetBuffer(new byte[_bufferSize], 0, _bufferSize);
                sendArgs.UserToken = ClientSession;

                // 소켓 옵션 설정.
                socket.LingerState = new LingerOption(true, 10);
                socket.NoDelay = true;


                // 패킷 수신 시작
                BeginReceive(socket, receiveArgs, sendArgs);


                // 서버와의 연결이 성공하면 서버로 세션 상태를 요청한다.
                // 응답으로 신규연결/재연결 여부를 전달 받을 수 있다.
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, ClientSession.SessionId);
                    ClientSession.Send((int)EInternalProtocol.AUTH_CLIENT_SESSION_REQ, 
                        ms.ToArray(), 
                        ms.ToArray().Length);
                }
            }
            else
            {
                // failed.
                //Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
            }
        }


        /// <summary>
        /// 서버와의 연결을 끊는다.
        /// </summary>
        public void Disconnect()
        {
            if (ClientSession.NetState == ENetState.Disconnected)
            {
                return;
            }
            
            ClientSession.Disconnect();
        }


        public override void Update()
        {
            // 연결 이벤트 처리
            ProcessConnectionEvent();


            // 패킷을 처리한다.
            _packetHandler.Update();
        }


        /// <summary>
        /// 연결 이벤트 처리
        /// </summary>
        protected void ProcessConnectionEvent()
        {
            // 서버 연결/해제 이벤트를 처리한다.
            ClientSession clientSession = null;
            lock (_netEventQueue)
            {
                if (_netEventQueue.Count == 0)
                {
                    return;
                }

                clientSession = _netEventQueue.Dequeue();
            }


            switch (clientSession.NetState)
            {
                case ENetState.Connected:
                    {
                        if (ClientConnectedCallback != null)
                        {
                            ClientConnectedCallback(clientSession, clientSession.SessionState);
                        }
                    }
                    break;
                case ENetState.Online:
                    {
                        if (ClientOnlineCallback != null)
                        {
                            ClientOnlineCallback(clientSession, clientSession.GetPeer());
                        }
                    }
                    break;
                case ENetState.Disconnected:
                    {
                        if (ClientDisconnectedCallback != null)
                        {
                            ClientDisconnectedCallback(clientSession, clientSession.SessionState);
                        }
                    }
                    break;
            }
        }

        
        protected override void CloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();
            clientSession.NetState = ENetState.Disconnected;


            lock (_netEventQueue)
            {
                _netEventQueue.Enqueue(clientSession);
            }
        }


        protected override void OnMessageCompleted(ClientSession clientSession, byte[] msg)
        {
            if (clientSession == null)
            {
                return;
            }


            if (_packetHandler == null)
            {
                return;
            }


            int protocolId = BitConverter.ToInt32(msg, 0);
            int length = BitConverter.ToInt32(msg, Defines.PROTOCOL_ID);
            byte[] buffer = new byte[_bufferSize];
            Array.Copy(msg, Defines.HEADER_SIZE, buffer, 0, length);


            if (protocolId == (int)EInternalProtocol.AUTH_CLIENT_SESSION_ACK)
            {
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(buffer))
                {
                    bool isReconnect = (bool)bf.Deserialize(ms);
                    short sessionState = (short)bf.Deserialize(ms);


                    clientSession.NetState = (isReconnect == false) 
                        ? ENetState.Connected 
                        : ENetState.Online;

                    clientSession.SessionState = (ESessionState)sessionState;


                    lock (_netEventQueue)
                    {
                        _netEventQueue.Enqueue(clientSession);
                    }
                }
            }
            else
            {
                // 패킷처리 큐에 추가한다.
                _packetHandler.EnqueuePacket(clientSession.GetPeer(), msg);

            }
        }
    }
}
