using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RWCoreLib.Log;
using RWCoreNetwork.NetPacket;

namespace RWCoreNetwork.NetService
{
    public enum ENetState : byte
    {
        Connecting,

        // 신규 소켓 연결 상태
        Connected,

        // 온라인 상태(재접속)
        Online,

        // 오프라인 상태(소켓 닫힘)
        Offline,

        // 연결 해제 상태(세션 제거)
        Disconnected,

        // 종료
        End,
    }


    internal enum EInternalProtocol
    {
        AUTH_CLIENT_SESSION_REQ = 1,
        AUTH_CLIENT_SESSION_ACK,
    }


    internal struct SocketConnectedNotify
    {
    }


    /// <summary>
    /// 네트워크 기본 서비스 클래스
    /// 소켓 연결 및 패킷의 송수신과 관련된 전반적인 처리를 담당한다.
    /// </summary>
    public class NetBaseService
    {
        public delegate void ClientReconnectDelegate(ClientSession clientSession, Peer peer);
        public delegate void ClientConnectDelegate(ClientSession clientSession);

        public ClientConnectDelegate ClientConnectedCallback { get; set; }
        public ClientConnectDelegate ClientDisconnectedCallback { get; set; }
        public ClientReconnectDelegate ClientOnlineCallback { get; set; }
        public ClientConnectDelegate ClientOfflineCallback { get; set; }



        // 패킷 핸들러
        protected PacketHandler _packetHandler;

        // 네트워크 상태 모니터링 핸들러
        protected NetMonitorHandler _netMonitorHandler;


        // 로그
        protected ILog _logger;


        protected readonly bool _onMonitoring;
        protected readonly int _bufferSize;
        private readonly int _keepAliveTime;
        private readonly int _keepAliveInterval;




        public NetBaseService(PacketHandler packetHandler, ILog logger, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring)
        {
            ClientConnectedCallback = null;
            ClientDisconnectedCallback = null;
            _netMonitorHandler = new NetMonitorHandler(logger, 10);


            _packetHandler = packetHandler;
            _logger = logger;


            _keepAliveTime = keepAliveTime;
            _keepAliveInterval = keepAliveInterval;
            _onMonitoring = onMonitoring;
            _bufferSize = bufferSize;
        }


        public void Clear()
        {
            if (_netMonitorHandler != null)
            {
                _netMonitorHandler.Print();
                _netMonitorHandler.Clear();
            }
        }


        public virtual void Update() 
        {
        }


        /// <summary>
        /// 소켓을 닫는다.
        /// </summary>
        /// <param name="clientSession"></param>
        protected virtual void CloseClientsocket(ClientSession clientSession, SocketError error)
        {
        }


        /// <summary>
        /// 비동기 패킷 수신을 시작한다.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="receiveArgs"></param>
        /// <param name="sendArgs"></param>
        protected void BeginReceive(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 됨. 둘 다 동일한 clientSession 물고 있음
            ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            clientSession.SetEventArgs(receiveArgs, sendArgs);


            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용함
            clientSession.Socket = clientSocket;
            clientSession.SetKeepAlive(_keepAliveTime, _keepAliveInterval);
            clientSession.NetState = ENetState.Connecting;


            // 데이터를 받을 수 있도록 소켓 메소드를 호출해준다.
            // 비동기로 수신할 경우 워커 스레드에서 대기중으로 있다가 Completed에 설정해놓은 메소드가 호출된다.
            // 동기로 완료될 경우에는 직접 완료 메소드를 호출해줘야 한다.
            bool pending = clientSocket.ReceiveAsync(receiveArgs);
            if (pending == false)
            {
                ProcessReceive(receiveArgs);
            }
        }


        /// <summary>
        /// 패킷 수신 완료 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation != SocketAsyncOperation.Receive)
            {
                throw new ArgumentException("The last operation completed on the socket was not a receive.");
            }

            ProcessReceive(e);
        }


        /// <summary>
        /// 비동기 수신 작업이 완료 될 때 호출됩니다. 
        /// 원격 호스트가 연결을 닫은 경우 소켓이 닫힙니다
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // 원격 호스트가 연결을 종료했는지 확인
            ClientSession clientSession = e.UserToken as ClientSession;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // 이후의 작업은 clientSession에 맡긴다.
                clientSession.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);
    

                // 다음 메시지 수신을 위해서 다시 ReceiveAsync 메소드를 호출한다.
                bool pending = clientSession.Socket.ReceiveAsync(e);
                if (pending == false)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientsocket(clientSession, e.SocketError);
            }
        }


        /// <summary>
        /// 패킷 송신 완료 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            ClientSession clientSession = e.UserToken as ClientSession;
            byte[] msg = clientSession.ProcessSend(e);

            if (_onMonitoring == true)
            {
                _netMonitorHandler.SetSendPacket(clientSession.Id, msg);
            }
        }


        /// <summary>
        /// 패킷을 큐에 추가한다.
        /// </summary>
        /// <param name="clientSession"></param>
        /// <param name="protocolId"></param>
        /// <param name="msg"></param>
        protected virtual void OnMessageCompleted(ClientSession clientSession, byte[] msg)
        {
        }
    }
}
