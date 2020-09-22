using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

using RWCoreNetwork.NetPacket;

namespace RWCoreNetwork.NetService
{
    public class NetServerService : NetBaseService
    {
        // 프로토콜 정의 델리게이트
        public delegate bool InterceptProtocolDelegate(UserToken userToken, short protocolId, byte[] msg);
        public InterceptProtocolDelegate InterceptProtocol;


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


        /// <summary>
        /// 패킷을 큐에 추가한다.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="protocolId"></param>
        /// <param name="msg"></param>
        protected override void OnMessageCompleted(UserToken userToken, short protocolId, byte[] msg)
        {
            if (userToken == null)
            {
                return;
            }


            if (_packetHandler == null)
            {
                return;
            }


            // 패킷 큐에 저장하기 전에 사용자가 지정한 처리를 수행한다.
            if (InterceptProtocol != null)
            {
                if (InterceptProtocol(userToken, protocolId, msg) == true)
                {
                    return;
                }
            }


            // 패킷처리 큐에 추가한다.
            _packetHandler.EnqueuePacket(new Packet(userToken.GetPeer(), protocolId, msg, msg.Length));
        }


        /// <summary>
        /// 릴레이 패킷을 전송한다.
        /// </summary>
        /// <param name="userToken">전송 유저 토큰</param>
        /// <param name="protocolId"></param>
        /// <param name="msg"></param>
        void SendRelayPacket(UserToken userToken, short protocolId, byte[] msg)
        {
            foreach (var elem in _userTokenMap)
            {
                if (elem.Value.Id == userToken.Id)
                {
                    continue;
                }

                elem.Value.Send(protocolId, msg);
            }
        }
    }
}
