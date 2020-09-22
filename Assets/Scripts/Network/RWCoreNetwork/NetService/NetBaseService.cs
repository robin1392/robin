using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using RWCoreNetwork.NetPacket;

namespace RWCoreNetwork.NetService
{
    public enum ENetState : byte
    {
        // 접속 완료.
        Connected,

        // 연결 끊김.
        Disconnected,

        // 종료
        End,
    }


    public class NetBaseService
    {
        public delegate void ClientConnectDelegate(UserToken token);
        public ClientConnectDelegate ClientConnectedCallback { get; set; }
        public ClientConnectDelegate ClientDisconnectedCallback { get; set; }

        
        // 유저 토큰 맵
        protected Dictionary<int, UserToken> _userTokenMap;


        // 패킷 핸들러
        protected PacketHandler _packetHandler;


        private readonly int preAllocCount = 2;
        private readonly int _keepAliveTime;
        private readonly int _keepAliveInterval;


        // 메시지 송수신시 필요한 오브젝트
        private SocketAsyncEventArgsPool _receiveEventAragePool;
        private SocketAsyncEventArgsPool _sendEventAragePool;


        // 메시지 수신, 전송시 비동기 소켓에서 사용할 버퍼를 관리하는 객체
        private BufferManager _bufferManager;


        // 네트워크 상태 큐
        protected Queue<UserToken> _netEventQueue;



        public NetBaseService(PacketHandler packetHandler, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval)
        {
            ClientConnectedCallback = null;
            ClientDisconnectedCallback = null;
            _userTokenMap = new Dictionary<int, UserToken>();
            _netEventQueue = new Queue<UserToken>();
            _packetHandler = packetHandler;


            // SocketAsyncEventArgs object pool 생성
            _receiveEventAragePool = new SocketAsyncEventArgsPool(maxConnection);
            _sendEventAragePool = new SocketAsyncEventArgsPool(maxConnection);

            // 버퍼 할당
            _bufferManager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            _bufferManager.InitBuffer();


            // SocketAsyncEventArgs object pool 할당
            SocketAsyncEventArgs args;
            for (int i = 0; i < maxConnection; i++)
            {
                UserToken userToken = new UserToken(bufferSize, i + 1);
                userToken.CompletedMessageCallback += OnMessageCompleted;


                // receive pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                    args.UserToken = userToken;

                    _bufferManager.SetBuffer(args);
                    _receiveEventAragePool.Push(args);
                }

                // send pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                    args.UserToken = userToken;

                    _bufferManager.SetBuffer(args);
                    _sendEventAragePool.Push(args);
                }
            }

            _keepAliveTime = keepAliveTime;
            _keepAliveInterval = keepAliveInterval;
        }


        public virtual void Update() 
        {
            // 연결 이벤트 처리
            ProcessConnectionEvent();
            
            // 수신된 패킷을 처리한다.
            _packetHandler.Update();
        }


        /// <summary>
        /// 연결 이벤트 처리
        /// </summary>
        protected void ProcessConnectionEvent()
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


        protected void BeginReceive(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 됨. 둘 다 동일한 userToken 물고 있음
            UserToken userToken = receiveArgs.UserToken as UserToken;
            userToken.SetEventArgs(receiveArgs, sendArgs);


            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용함
            userToken.Socket = clientSocket;
            userToken.SetKeepAlive(_keepAliveTime, _keepAliveInterval);


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
        /// 비동기 수신 작업이 완료 될 때 호출됩니다
        /// 원격 호스트가 연결을 닫은 경우 소켓이 닫힙니다
        /// </summary>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // 원격 호스트가 연결을 종료했는지 확인
            UserToken userToken = e.UserToken as UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //Console.WriteLine(string.Format("[{0}] Receive message. handle {1},  count {2}", Thread.CurrentThread.ManagedThreadId, userToken.Socket.Handle, _connectedCount));

                // 이후의 작업은 CUserToekn에 맡긴다.
                userToken.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);

                // 다음 메시지 수신을 위해서 다시 ReceiveAsync 메소드를 호출한다.
                bool pending = userToken.Socket.ReceiveAsync(e);
                if (pending == false)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                //Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
                CloseClientsocket(userToken);
            }
        }


        /// <summary>
        /// 패킷 송신 완료 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            UserToken userToken = e.UserToken as UserToken;
            userToken.ProcessSend(e);
        }


        /// <summary>
        /// 소켓을 오픈한다.
        /// </summary>
        /// <param name="socket"></param>
        protected virtual void OpenClientSocket(Socket socket)
        {
            SocketAsyncEventArgs receiveArgs = _receiveEventAragePool.Pop();
            SocketAsyncEventArgs sendArgs = _sendEventAragePool.Pop();


            // 소켓 옵션 설정.
            socket.LingerState = new LingerOption(true, 10);
            socket.NoDelay = true;


            // 패킷 수신 시작
            UserToken userToken = receiveArgs.UserToken as UserToken;
            BeginReceive(socket, receiveArgs, sendArgs);


            // 접속 유저 토큰 추가
            lock (_userTokenMap)
            {
                _userTokenMap.Add(userToken.Id, userToken);
            }


            userToken.NetState = ENetState.Connected;
            CallClientConnectedCallback(userToken);
        }


        /// <summary>
        /// 소켓을 닫는다.
        /// </summary>
        /// <param name="userToken"></param>
        protected virtual void CloseClientsocket(UserToken userToken)
        {
            userToken.OnRemoved();
            userToken.NetState = ENetState.Disconnected;


            CallClientDisconnectedCallback(userToken);


            if (_receiveEventAragePool != null)
            {
                _receiveEventAragePool.Push(userToken.ReceiveEventArgs);
               // userToken.ReceiveEventArgs = null;
            }


            if (_sendEventAragePool != null)
            {
                _sendEventAragePool.Push(userToken.SendEventArgs);
                //userToken.SendEventArgs = null;
            }


            // 유저 토큰 제거
            lock(_userTokenMap)
            {
                _userTokenMap.Remove(userToken.Id);
            }
            
           // userToken.Socket = null;
        }


        /// <summary>
        /// 패킷을 큐에 추가한다.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="protocolId"></param>
        /// <param name="msg"></param>
        protected virtual void OnMessageCompleted(UserToken userToken, short protocolId, byte[] msg)
        {
            if (userToken == null)
            {
                return;
            }


            if (_packetHandler == null)
            {
                return;
            }


            // 패킷처리 큐에 추가한다.
            _packetHandler.EnqueuePacket(userToken.GetPeer(), protocolId, msg);
        }


        public List<UserToken> GetUserTokenList()
        {
            lock(_userTokenMap)
            {
                return _userTokenMap.Values.ToList();
            }
        }

        public int GetUserTokenCount()
        {
            lock (_userTokenMap)
            {
                return _userTokenMap.Count;
            }
        }


        protected virtual void CallClientConnectedCallback(UserToken userToken)
        {
            if (ClientConnectedCallback != null)
            {
                ClientConnectedCallback(userToken);
            }
        }


        protected virtual void CallClientDisconnectedCallback(UserToken userToken)
        {
            if (ClientDisconnectedCallback != null)
            {
                ClientDisconnectedCallback(userToken);
            }
        }

        public int GetPacketQueueCount()
        {
            return _packetHandler.Count();
        }
    }
}
