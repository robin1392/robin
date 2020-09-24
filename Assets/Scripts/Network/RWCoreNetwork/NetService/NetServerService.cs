using System.Net.Sockets;
using RWCoreNetwork.NetPacket;

namespace RWCoreNetwork.NetService
{
    public class NetServerService : NetBaseService
    {
        private Listener _listener;



        public NetServerService(PacketHandler packetHandler, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval)
            : base(packetHandler, maxConnection, bufferSize, keepAliveTime, keepAliveInterval)
        {
            _listener = new Listener();
            _listener.OnNewClientCallback += OnConnectedClient;
        }


        /// <summary>
        /// 서버 네트워크 서비스를 시작한다.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="backlog"></param>
        /// <returns></returns>
        public bool Start(string host, int port, int backlog)
        {
            return _listener.Start(host, port, backlog);
        }


        /// <summary>
        /// 새로운 클라이언트가 접속 성공 했을 때 호출됩니다.
        /// AcceptAsync의 콜백 매소드에서 호출되며 여러 스레드에서 동시에 호출될 수 있기 때문에 공유자원에 접근할 때는 주의해야 합니다.
        /// </summary>
        /// <param name="client_socket"></param>
        /// <param name="token"></param>
        private void OnConnectedClient(Socket clientSocket, object token)
        {
            OpenClientSocket(clientSocket);
        }
    }
}
