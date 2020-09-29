using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using RWCoreLib.Log;
using RWCoreNetwork.NetPacket;


namespace RWCoreNetwork.NetService
{
    public class NetClientService : NetBaseService
    {
        // 네트워크 상태 큐
        Queue<UserToken> _netEventQueue;



        public NetClientService(PacketHandler packetHandler, ILog logger, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring)
            : base(packetHandler, logger, maxConnection, bufferSize, keepAliveTime, keepAliveInterval, onMonitoring)
        {
            _netEventQueue = new Queue<UserToken>();
        }


        /// <summary>
        /// N개의 클라이언트를 서버에 연결한다.
        /// </summary>
        /// <param name="serverAddr"></param>
        /// <param name="port"></param>
        /// <param name="clientCount"></param>
        public void Connect(string serverAddr, int port, int clientCount)
        {
            for (int i = 0; i < clientCount; i++)
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
                OpenClientSocket(socket);
            }
            else
            {
                // failed.
                //Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
            }
        }


        protected override void CallClientConnectedCallback(UserToken userToken)
        {
            lock (_netEventQueue)
            {
                _netEventQueue.Enqueue(userToken);
            }
        }


        /// <summary>
        /// 연결 이벤트 처리
        /// </summary>
        protected override void ProcessConnectionEvent()
        {
            // 서버 연결/해제 이벤트를 처리한다.
            UserToken userToken = null;
            lock (_netEventQueue)
            {
                if (_netEventQueue.Count == 0)
                {
                    return;
                }

                userToken = _netEventQueue.Dequeue();
            }


            switch (userToken.NetState)
            {
                case ENetState.Connected:
                    {
                        if (ClientConnectedCallback != null)
                        {
                            ClientConnectedCallback(userToken);
                        }
                    }
                    break;
                case ENetState.Disconnected:
                    {
                        if (ClientDisconnectedCallback != null)
                        {
                            ClientDisconnectedCallback(userToken);
                        }
                    }
                    break;
            }
        }


        protected override void CallClientDisconnectedCallback(UserToken userToken)
        {
            lock (_netEventQueue)
            {
                _netEventQueue.Enqueue(userToken);
            }
        }


        /// <summary>
        /// 서버와의 연결을 끊는다.
        /// </summary>
        public void Disconnect()
        {
            foreach (var elem in _userTokenMap.Values)
            {
                elem.Disconnect();
            }
        }



    }
}
