using System;
using System.Threading;
using System.Net.Sockets;

using RWCoreNetwork.NetPacket;

namespace RWCoreNetwork
{
    public class NetworkService
    {
        // 클라이언트의 접속을 받아들이기 위한 객체
        Listener m_clientListener;

        // 메시지 송수신시 필요한 오브젝트
        SocketAsyncEventArgsPool m_receiveEventAragePool;
        SocketAsyncEventArgsPool m_sendEventAragePool;

        // 메시지 수신, 전송시 비동기 소켓에서 사용할 버퍼를 관리하는 객체
        BufferManager m_bufferManager;

        // 패킷 처리자
        public PacketHandler PacketHandler { get; private set; }


        // 클라이언트의 접속이 이루어졌을 때 호출되는 델리게이트
        public delegate void SessionHandler(UserToken token);
        public SessionHandler OnSessionCreatedCallback { get; set; }

        int m_connectedCount;
        readonly int preAllocCount = 2;

        public NetworkService(PacketHandler packetHandler)
        {
            PacketHandler = packetHandler;
            m_connectedCount = 0;
            OnSessionCreatedCallback = null;
        }

        public void Initialize(int maxConnection, int bufferSize)
        {
            m_clientListener = new Listener();
            m_clientListener.OnNewClientCallback += OnNewClient;

            // 버퍼 할당
            m_bufferManager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            m_bufferManager.InitBuffer();

            // SocketAsyncEventArgs object pool 생성
            m_receiveEventAragePool = new SocketAsyncEventArgsPool(maxConnection);
            m_sendEventAragePool = new SocketAsyncEventArgsPool(maxConnection);

            // SocketAsyncEventArgs object pool 할당
            SocketAsyncEventArgs args;
            for (int i = 0; i < maxConnection; i++)
            {
                UserToken userToken = new UserToken(this);

                // receive pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                    args.UserToken = userToken;

                    m_bufferManager.SetBuffer(args);
                    m_receiveEventAragePool.Push(args);                    
                }
                
                // send pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                    args.UserToken = userToken;

                    m_bufferManager.SetBuffer(args);
                    m_sendEventAragePool.Push(args);                    
                }
            }
        }

        public bool Start(string host, int port, int backlog)
        {
            return m_clientListener.Start(host, port, backlog);
        }

        /// <summary>
        /// 원격 서버에 접속 성공 했을 때 호출됩니다.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="userToken"></param>
        public void OnConnectCompleted(Socket socket, UserToken userToken)
        {
            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            receiveArgs.UserToken = userToken;
            receiveArgs.SetBuffer(new byte[1024], 0, 1024);

            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            sendArgs.UserToken = userToken;
            sendArgs.SetBuffer(new byte[1024], 0, 1024);

            BeginReceive(socket, receiveArgs, sendArgs);   
        }

        /// <summary>
        /// 새로운 클라이언트가 접속 성공 했을 때 호출됩니다.
        /// AcceptAsync의 콜백 매소드에서 호출되며 여러 스레드에서 동시에 호출될 수 있기 때문에 공유자원에 접근할 때는 주의해야 합니다.
        /// </summary>
        /// <param name="client_socket"></param>
        /// <param name="token"></param>
        private void OnNewClient(Socket clientSocket, object token)
        {
            // TODO : add peer to list

            Interlocked.Increment(ref m_connectedCount);

            Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}", Thread.CurrentThread.ManagedThreadId, clientSocket.Handle, m_connectedCount));

            SocketAsyncEventArgs receiveArgs = m_receiveEventAragePool.Pop();
            SocketAsyncEventArgs sendArgs = m_sendEventAragePool.Pop();

            UserToken userToken = null;
            if (OnSessionCreatedCallback != null)
            {
                userToken = receiveArgs.UserToken as UserToken;
                OnSessionCreatedCallback(userToken);
            }

            BeginReceive(clientSocket, receiveArgs, sendArgs);   

            // TODO : userToken의 keepalive 처리
        }

        private void BeginReceive(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 됨. 둘 다 동일한 userToken 물고 있음
            UserToken userToken = receiveArgs.UserToken as UserToken;
            userToken.SetEventArgs(receiveArgs, sendArgs);

            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용함
            userToken.Socket = clientSocket;

            // 데이터를 받을 수 있도록 소켓 메소드를 호출해준다.
            // 비동기로 수신할 경우 워커 스레드에서 대기중으로 있다가 Completed에 설정해놓은 메소드가 호출된다.
            // 동기로 완료될 경우에는 직접 완료 메소드를 호출해줘야 한다.
            bool pending = clientSocket.ReceiveAsync(receiveArgs);
            if (pending == false)
            {
                ProcessReceive(receiveArgs);
            }
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
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
                Console.WriteLine(string.Format("[{0}] Receive message. handle {1},  count {2}", Thread.CurrentThread.ManagedThreadId, userToken.Socket.Handle, m_connectedCount));

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
				Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
				CloseClientsocket(userToken);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            UserToken userToken = e.UserToken as UserToken;
            userToken.ProcessSend(e);
        }

        private void CloseClientsocket(UserToken userToken)
        {
            Console.WriteLine(string.Format("Close socket. {0}", userToken.Socket.Handle));

            userToken.OnRemoved();

            if (m_receiveEventAragePool != null)
            {
                m_receiveEventAragePool.Push(userToken.ReceiveEventArgs);
            }

            if (m_sendEventAragePool != null)
            {
                m_sendEventAragePool.Push(userToken.SendEventArgs);
            }
        }

    }
}