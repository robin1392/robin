using RWCoreNetwork.NetPacket;
using System.Net;
using System.Net.Sockets;


namespace RWCoreNetwork.NetService
{
    public class NetClientService : NetBaseService
    {
        public NetClientService(PacketHandler packetHandler, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval)
            : base(packetHandler, maxConnection, bufferSize, keepAliveTime, keepAliveInterval)
        {
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
                userToken.NetState = ENetState.Connected;
                _netEventQueue.Enqueue(userToken);
            }
        }


        protected override void CallClientDisconnectedCallback(UserToken userToken)
        {
            lock (_netEventQueue)
            {
                userToken.NetState = ENetState.Disconnected;
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
