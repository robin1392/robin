using System;
using System.Net;
using System.Net.Sockets;

namespace RWCoreNet
{
    public enum EConnectStatus
    {
        None,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected
    }

    /// <summary>
	/// Endpoint정보를 받아서 서버에 접속한다.
	/// 접속하려는 서버 하나당 인스턴스 한개씩 생성하여 사용하면 된다.
	/// </summary>
    public class Connector
    {
        public delegate void ConnectedHandler(ClientSession session);
        public ConnectedHandler OnConnectedCallback { get; set; }

        private Socket m_client;

        private NetworkService m_networkService;

        public EConnectStatus ConnectStatus { get; private set; }


        public Connector(NetworkService networkService)
        {
            m_networkService = networkService;
            OnConnectedCallback = null;
            ConnectStatus = EConnectStatus.None;
        }

        public void Connect(IPEndPoint endpoint)
        {
            m_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endpoint;

            ConnectStatus = EConnectStatus.Connecting;

            bool pendiing = m_client.ConnectAsync(args);
            if (pendiing == false)
            {
                OnConnectCompleted(null, args);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                ConnectStatus = EConnectStatus.Connected;

                ClientSession session = new ClientSession(m_networkService);

                m_networkService.OnConnectCompleted(m_client, session);

                if (OnConnectedCallback != null)
                {
                    OnConnectedCallback(session);
                }
            }
            else
            {
				// failed.
				Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
            }
        }

        public void Disconnect()
        {
            OnConnectedCallback = null;
            m_networkService = null;
        }
    }
}