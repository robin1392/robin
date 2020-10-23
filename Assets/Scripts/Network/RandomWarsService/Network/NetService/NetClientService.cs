using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using RandomWarsService.Core;
using RandomWarsService.Network.NetPacket;

namespace RandomWarsService.Network.NetService
{
    public class NetClientService : NetBaseService
    {
        public ClientSession ClientSession { get; set; }

        // 네트워크 상태 큐
        private Queue<ClientSession> _netEventQueue;

        private byte _retryConnectCount;


        public NetClientService(PacketHandler packetHandler, ILog logger, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring)
            : base(packetHandler, logger, bufferSize, keepAliveTime, keepAliveInterval, onMonitoring)
        {
            _netEventQueue = new Queue<ClientSession>();

            ClientSession = new ClientSession(_logger, _bufferSize);
            ClientSession.CompletedMessageCallback += OnMessageCompleted;

            _retryConnectCount = 0;
        }


        public override void Update()
        {
            // 연결 이벤트 처리
            ProcessConnectionEvent();


            // 패킷을 처리한다.
            _packetHandler.Update();
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


                _retryConnectCount = 0;


                // 서버와의 연결이 성공하면 서버로 세션 상태를 요청한다.
                // 응답으로 신규연결/재연결 여부를 전달 받을 수 있다.
                SendInternalAuthSessionReq(ClientSession);
            }
            else
            {
                if (_retryConnectCount++ > 4)
                {
                    if (ClientSession.NetState == ENetState.Connecting)
                    {
                        ClientSession.NetState = ENetState.Connected;
                    }
                    else
                    {
                        ClientSession.NetState = ENetState.Reconnected;
                    }

                    ClientSession.DisconnectState = ESessionState.TimeOut;
                    lock (_netEventQueue)
                    {
                        _netEventQueue.Enqueue(ClientSession);
                    }
                    return;
                }

                IPEndPoint remoteIpEndPoint = e.RemoteEndPoint as IPEndPoint;
                Connect(remoteIpEndPoint.Address.ToString(), remoteIpEndPoint.Port);
                _logger.Error(string.Format("Failed to connect server(error: {0}), Retry to connect. address: {1}", e.SocketError, remoteIpEndPoint.Address + ":" + remoteIpEndPoint.Port));
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
                            ClientConnectedCallback(clientSession, clientSession.DisconnectState);
                        }
                    }
                    break;
                case ENetState.Reconnected:
                    {
                        clientSession.NetState = ENetState.Connected;

                        if (ClientReconnectedCallback != null)
                        {
                            ClientReconnectedCallback(clientSession, clientSession.DisconnectState);
                        }
                    }
                    break;
                case ENetState.Disconnected:
                    {
                        if (ClientDisconnectedCallback != null)
                        {
                            ClientDisconnectedCallback(clientSession, clientSession.DisconnectState);
                        }
                    }
                    break;
            }
        }

        
        protected override void OnCloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();
            clientSession.NetState = ENetState.Disconnected;


            lock (_netEventQueue)
            {
                _netEventQueue.Enqueue(clientSession);
            }
        }


        protected override void OnMessageCompleted(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            if (clientSession == null)
            {
                return;
            }


            if (_packetHandler == null)
            {
                return;
            }


            if (ProcessInternalPacket(clientSession, protocolId, msg, length) == false)
            {
                // 패킷처리 큐에 추가한다.
                _packetHandler.EnqueuePacket(clientSession.GetPeer(), protocolId, msg, length);
            }
        }


        protected override bool ProcessInternalPacket(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch((EInternalProtocol)protocolId)
            {
                case EInternalProtocol.AUTH_SESSION_ACK:
                    {
                        ENetState netState;
                        ESessionState sessionState;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            netState = (ENetState)(byte)bf.Deserialize(ms);
                            sessionState = (ESessionState)(short)bf.Deserialize(ms);
                        }


                        if (netState == ENetState.Connected)
                        {
                            if (clientSession.NetState == ENetState.Connecting)
                            {
                                clientSession.NetState = ENetState.Connected;
                            }
                            else
                            {
                                clientSession.NetState = ENetState.Reconnected;
                            }
                        }


                        clientSession.DisconnectState = sessionState;
                        lock (_netEventQueue)
                        {
                            _netEventQueue.Enqueue(clientSession);
                        }
                    }
                    break;
                case EInternalProtocol.DISCONNECT_SESSION_NOTIFY:
                    {
                        ESessionState sessionState;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            sessionState = (ESessionState)(short)bf.Deserialize(ms);
                        }

                        clientSession.DisconnectState = sessionState;
                    }
                    break;
                case EInternalProtocol.PAUSE_SESSION_ACK:
                case EInternalProtocol.RESUME_SESSION_ACK:
                    {
                    }
                    break;
                default:
                    {
                        return false;
                    }
            }

            return true;
        }


        public void PauseSession(ClientSession clientSession)
        {
            clientSession.PauseStartTimeTick = DateTime.UtcNow.Ticks;
            SendInternalPauseSessionReq(clientSession);
        }


        public void ResumeSession(ClientSession clientSession)
        {
            if (clientSession.PauseStartTimeTick == 0)
            {
                return;
            }

            if (clientSession.PauseStartTimeTick + (TimeSpan.TicksPerSecond * 10) < DateTime.UtcNow.Ticks)
            {
                clientSession.GetPeer().Disconnect(ESessionState.Wait);
                return;
            }

            clientSession.NetState = ENetState.Connected;
            SendInternalResumeSessionReq(clientSession);
        }
    }
}
