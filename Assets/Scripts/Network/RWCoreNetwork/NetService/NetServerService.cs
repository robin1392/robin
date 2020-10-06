using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using RWCoreLib.Log;
using RWCoreNetwork.NetPacket;
using System.Linq;
using RWCoreLib;

namespace RWCoreNetwork.NetService
{
    public class NetServerService : NetBaseService
    {
        private Listener _listener;

        private bool _onRelayQueue;

        // 메시지 송수신시 필요한 오브젝트
        private SocketAsyncEventArgsPool _receiveEventAragePool;
        private SocketAsyncEventArgsPool _sendEventAragePool;


        // 메시지 수신, 전송시 비동기 소켓에서 사용할 버퍼를 관리하는 객체
        private BufferManager _bufferManager;


        // 연결된 클라이언트 세션 
        protected Dictionary<int, ClientSession> _connectedClientSessions;


        // 연결이 끊긴 클라이언트 세션
        // 지정 시간 경과시 제거한다.
        protected List<ClientSession> _disconnectedClientSessions;
        private List<ClientSession> _removeClientSession;


        // 인증된 클라이언트 세션
        protected Dictionary<string, ClientSession> _authedClientSessions;


        private long _removeClientSessionTick;
        private object _lockClientSessionContainer;

        private readonly int preAllocCount = 2;


        public NetServerService(PacketHandler packetHandler, ILog logger, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring, bool onRelayQueue)
            : base(packetHandler, logger, bufferSize, keepAliveTime, keepAliveInterval, onMonitoring)
        {
            _listener = new Listener();
            _listener.OnNewClientCallback += OnConnectedClient;
            _onRelayQueue = onRelayQueue;


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
                ClientSession clientSession = new ClientSession(_logger, bufferSize, i + 1);
                clientSession.CompletedMessageCallback += OnMessageCompleted;


                // receive pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                    args.UserToken = clientSession;

                    _bufferManager.SetBuffer(args);
                    _receiveEventAragePool.Push(args);
                }

                // send pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                    args.UserToken = clientSession;

                    _bufferManager.SetBuffer(args);
                    _sendEventAragePool.Push(args);
                }
            }


            _connectedClientSessions = new Dictionary<int, ClientSession>();
            _authedClientSessions = new Dictionary<string, ClientSession>();
            _disconnectedClientSessions = new List<ClientSession>();
            _removeClientSession = new List<ClientSession>();

            _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;
            _lockClientSessionContainer = new object();
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
            SocketAsyncEventArgs receiveArgs = _receiveEventAragePool.Pop();
            SocketAsyncEventArgs sendArgs = _sendEventAragePool.Pop();


            // 소켓 옵션 설정.
            clientSocket.LingerState = new LingerOption(true, 10);
            clientSocket.NoDelay = true;


            // 패킷 수신 시작
            ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            BeginReceive(clientSocket, receiveArgs, sendArgs);


            // 연결된 클라이언트 세션 목록에 추가
            lock (_lockClientSessionContainer)
            {
                _connectedClientSessions.Add(clientSession.Id, clientSession);
            }
        }

        public override void Update()
        {
            if (_onMonitoring == true)
            {
                _netMonitorHandler.Print();
            }


            // 패킷을 처리한다.
            _packetHandler.Update();


            // 접속 해제 클라이언트 세션을 제거한다.
            long nowTick = DateTime.UtcNow.Ticks;
            if (_removeClientSessionTick < nowTick)
            {
                _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;

                if (_disconnectedClientSessions.Count > 0)
                {
                    lock (_lockClientSessionContainer)
                    {
                        ClientSession clientSession = _disconnectedClientSessions.Last();
                        while (clientSession != null && clientSession.AliveTimeTick < nowTick)
                        {
                            _disconnectedClientSessions.RemoveAt(_disconnectedClientSessions.Count - 1);
                            _removeClientSession.Add(clientSession);

                            if (_disconnectedClientSessions.Count == 0)
                            {
                                break;
                            }
                            clientSession = _disconnectedClientSessions.Last();
                        }
                    }


                    foreach (var clientSession in _removeClientSession)
                    {
                        if (ClientDisconnectedCallback != null)
                        {
                            ClientDisconnectedCallback(clientSession);
                        }

                        _logger.Info(string.Format("Remove client session. sessionId: {0}", clientSession.ClientSessionId));
                    }
                    _removeClientSession.Clear();
                }
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


            if (_onMonitoring == true)
            {
                _netMonitorHandler.SetReceivePacket(clientSession.Id, msg);
            }


            int protocolId = BitConverter.ToInt32(msg, 0);
            int length = BitConverter.ToInt32(msg, Defines.PROTOCOL_ID);
            byte[] buffer = new byte[_bufferSize];
            Array.Copy(msg, Defines.HEADER_SIZE, buffer, 0, length);


            if (protocolId == (int)EInternalProtocol.CHECK_SESSION_REQ)
            {
                bool isReconnect = false;
                {
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream(buffer))
                    {
                        lock(_authedClientSessions)
                        {
                            string clientSessionId = (string)bf.Deserialize(ms);
                            clientSession.ClientSessionId = clientSessionId;


                            ClientSession oldClientSession = null;
                            if (_authedClientSessions.TryGetValue(clientSessionId, out oldClientSession) == true)
                            {
                                isReconnect = true;

                                if (ClientOnlineCallback != null)
                                {
                                    ClientOnlineCallback(clientSession, oldClientSession.GetPeer());
                                }
                            }
                            else
                            {
                                _authedClientSessions.Add(clientSessionId, clientSession);

                                if (ClientConnectedCallback != null)
                                {
                                    ClientConnectedCallback(clientSession);
                                }
                            }
                        }

                        lock(_lockClientSessionContainer)
                        {
                            _connectedClientSessions.Remove(clientSession.Id);
                        }
                    }
                }

                {
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream())
                    {
                        bf.Serialize(ms, isReconnect);
                        clientSession.Send((int)EInternalProtocol.CHECK_SESSION_ACK, ms.ToArray(), ms.ToArray().Length);
                    }
                }
            }
            else
            {
                if (_onRelayQueue == false)
                {
                    if (_packetHandler.InterceptProtocol != null)
                    {
                        if (_packetHandler.InterceptProtocol(clientSession.GetPeer(), protocolId, buffer, length) == true)
                        {
                            return;
                        }
                    }
                }


                // 패킷처리 큐에 추가한다.
                _packetHandler.EnqueuePacket(clientSession.GetPeer(), protocolId, buffer, length);
            }
        }

        protected override void CloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();
            clientSession.NetState = ENetState.Disconnected;


            if (ClientOfflineCallback != null)
            {
                ClientOfflineCallback(clientSession);
            }


            if (_receiveEventAragePool != null)
            {
                _receiveEventAragePool.Push(clientSession.ReceiveEventArgs);
            }


            if (_sendEventAragePool != null)
            {
                _sendEventAragePool.Push(clientSession.SendEventArgs);
            }


            lock (_lockClientSessionContainer)
            {
                _authedClientSessions.Remove(clientSession.ClientSessionId);

                clientSession.AliveTimeTick = DateTime.UtcNow.AddSeconds(30).Ticks;
                _disconnectedClientSessions.Insert(0, clientSession);

            }
        }
    }
}
