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
        public ClientConnectDelegate ClientPauseCallback { get; set; }
        public ClientConnectDelegate ClientResumeCallback { get; set; }


        private Listener _listener;

        private bool _onRelayQueue;

        // 메시지 송수신시 필요한 오브젝트
        private SocketAsyncEventArgsPool _receiveEventAragePool;
        private SocketAsyncEventArgsPool _sendEventAragePool;


        // 메시지 수신, 전송시 비동기 소켓에서 사용할 버퍼를 관리하는 객체
        private BufferHandler _bufferManager;


        // 인증된 클라이언트 세션
        protected Dictionary<string, ClientSession> _authedClientSessions;


        // 재연결 대기 세션
        protected List<ClientSession> _reconnectClientSessions;


        // 만료된 클라이언트 세션
        protected List<string> _expiredClientSessions;


        object _lockSessionContainer = new object();

        private long _removeClientSessionTick;
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
            _bufferManager = new BufferHandler(maxConnection * bufferSize * preAllocCount, bufferSize);
            _bufferManager.InitBuffer();


            // SocketAsyncEventArgs object pool 할당
            SocketAsyncEventArgs args;
            for (int i = 0; i < maxConnection; i++)
            {
                ClientSession clientSession = new ClientSession(_logger, bufferSize);
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


            _authedClientSessions = new Dictionary<string, ClientSession>();
            _reconnectClientSessions = new List<ClientSession>();
            _expiredClientSessions = new List<string>();

            _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;
        }


        public override void Clear()
        {
            base.Clear();

            lock (_lockSessionContainer)
            {
                foreach (var session in _authedClientSessions.Values)
                {
                    if (_receiveEventAragePool != null)
                    {
                        _receiveEventAragePool.Push(session.ReceiveEventArgs);
                    }

                    if (_sendEventAragePool != null)
                    {
                        _sendEventAragePool.Push(session.SendEventArgs);
                    }
                }
                _authedClientSessions.Clear();
                _reconnectClientSessions.Clear();
                _expiredClientSessions.Clear();
            }


            _logger.Info(string.Format("[Clear] _receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));
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

            _logger.Info(string.Format("[OnConnectedClient] _receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));


            // 소켓 옵션 설정.
            clientSocket.LingerState = new LingerOption(true, 10);
            clientSocket.NoDelay = true;


            // 패킷 수신 시작
            ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            BeginReceive(clientSocket, receiveArgs, sendArgs);
        }


        public override void Update()
        {
            // 패킷을 처리한다.
            _packetHandler.Update();


            // 접속 해제 클라이언트 세션을 제거한다.
            long nowTick = DateTime.UtcNow.Ticks;
            lock (_lockSessionContainer)
            {
                if (_removeClientSessionTick < nowTick)
                {
                    _removeClientSessionTick = DateTime.UtcNow.AddSeconds(1).Ticks;


                    if (_reconnectClientSessions.Count > 0)
                    {
                        ClientSession clientSession = _reconnectClientSessions.Last();
                        while (clientSession != null && clientSession.AliveTimeTick < nowTick)
                        {
                            _reconnectClientSessions.RemoveAt(_reconnectClientSessions.Count - 1);
                            _expiredClientSessions.Add(clientSession.SessionId);


                            clientSession.NetState = ENetState.Disconnected;
                            if (ClientDisconnectedCallback != null)
                            {
                                ClientDisconnectedCallback(clientSession, clientSession.DisconnectState);
                            }


                            if (_reconnectClientSessions.Count == 0)
                            {
                                break;
                            }
                            clientSession = _reconnectClientSessions.Last();
                        }
                    }


                    foreach (var clientSession in _authedClientSessions.Values)
                    {
                        if (clientSession.DisconnectState != EDisconnectState.None && clientSession.Socket.Connected == true)
                        {
                            // 접속 해제 상태이면 접속 해제를 먼저 알리고 세션 연결을 끊는다.
                            SendInternalDisconnectSessionNotify(clientSession);
                            clientSession.Disconnect();
                        }
                        else if (clientSession.ExpiredPauseTime() == true)
                        {
                            // Pause 허용 시간 초과시 세션 연결을 끊는다.
                            clientSession.Disconnect();
                        }
                    }
                }
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


            if (_onMonitoring == true)
            {
                _netMonitorHandler.SetReceivePacket(clientSession.SessionId, protocolId, msg, length);
                _logger.Info(string.Format("OnMessageCompleted. protocolId: {0}, length: {1}", protocolId, length));
            }


            // 네트워크 서비스 내부 패킷을 처리한다.
            if (ProcessInternalPacket(clientSession, protocolId, msg, length) == false)
            {
                if (_onRelayQueue == false && _packetHandler.InterceptProtocol != null)
                {
                    if (_packetHandler.InterceptProtocol(clientSession.GetPeer(), protocolId, msg, length) == true)
                    {
                        return;
                    }
                }


                // 패킷처리 큐에 추가한다.
                _packetHandler.EnqueuePacket(clientSession.GetPeer(), protocolId, msg, length);
            }
        }


        protected override void CloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();


            lock (_lockSessionContainer)
            {
                _authedClientSessions.Remove(clientSession.SessionId);


                // 재연결 대기 목록에 추가한다.
                if (clientSession.DisconnectState == EDisconnectState.None || clientSession.DisconnectState == EDisconnectState.Wait)
                {
                    clientSession.NetState = ENetState.Reconnecting;
                    clientSession.AliveTimeTick = DateTime.UtcNow.AddSeconds(30).Ticks;

                    if (_reconnectClientSessions.Contains(clientSession) == true)
                    {
                        _logger.Fatal(string.Format("[CloseClientsocket] already exist reconnect session. clientSessionId: {0}", clientSession.SessionId));
                        return;
                    }

                    _reconnectClientSessions.Insert(0, clientSession);


                    _logger.Info(string.Format("[CloseClientsocket] reconnect session. clientSessionId: {0}", clientSession.SessionId));
                }
                else
                {
                    if (_expiredClientSessions.Exists(x => x == clientSession.SessionId) == false)
                    {
                        if (clientSession.NetState != ENetState.Reconnecting)
                        {
                            _expiredClientSessions.Add(clientSession.SessionId);
                        }
                    }
                    

                    _logger.Info(string.Format("[CloseClientsocket] expired session. clientSessionId: {0}", clientSession.SessionId));
                }
            }


            clientSession.NetState = ENetState.Disconnected;
            if (ClientOfflineCallback != null && clientSession.GetPeer() != null)
            {
                ClientOfflineCallback(clientSession, clientSession.DisconnectState);
            }


            if (_receiveEventAragePool != null)
            {
                _receiveEventAragePool.Push(clientSession.ReceiveEventArgs);
            }

            if (_sendEventAragePool != null)
            {
                _sendEventAragePool.Push(clientSession.SendEventArgs);
            }

            _logger.Info(string.Format("[CloseClientsocket] _receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));

        }


        protected override bool ProcessInternalPacket(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch((EInternalProtocol)protocolId)
            {
                case EInternalProtocol.AUTH_SESSION_REQ:
                    {
                        string clientSessionId = string.Empty;
                        ENetState clientNetState = ENetState.End;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            clientSessionId = (string)bf.Deserialize(ms);
                            clientNetState = (ENetState)(byte)bf.Deserialize(ms);
                        }


                        if (clientSessionId == string.Empty)
                        {
                            _logger.Fatal("OnMessageCompleted. clientSessionId is empty!!!");
                            return true;
                        }


                        clientSession.SessionId = clientSessionId;
                        clientSession.DisconnectState = EDisconnectState.None;
                        clientSession.NetState = ENetState.Connected;


                        lock (_lockSessionContainer)
                        {
                            // 중복 세션 접속시 기존 세션 접속을 종료시킨다.
                            ClientSession duplicatedSession = null;
                            if (_authedClientSessions.TryGetValue(clientSessionId, out duplicatedSession) == true)
                            {
                                _logger.Info(string.Format("[OnMessageCompleted] duplicated session. clientSessionId: {0}", clientSessionId));


                                // 인증 세션 목록에서 제거한다.
                                _authedClientSessions.Remove(duplicatedSession.SessionId);


                                // 중복 세션 접속 종료
                                //duplicatedSession.GetPeer().Disconnect(EDisconnectState.Duplicated);
                                clientSession.DisconnectState = EDisconnectState.Expired;
                                SendInternalDisconnectSessionNotify(clientSession);
                                _authedClientSessions.Add(clientSession.SessionId, clientSession);

                            }


                            // 신규 접속 요청 처리
                            if (clientNetState == ENetState.Connecting)
                            {
                                _logger.Info(string.Format("[OnMessageCompleted] connect session. clientSessionId: {0}", clientSessionId));


                                if (_expiredClientSessions.Contains(clientSessionId) == true)
                                {
                                    _logger.Info(string.Format("[OnMessageCompleted] expired session. clientSessionId: {0}", clientSessionId));

                                    clientSession.DisconnectState = EDisconnectState.Expired;
                                    SendInternalDisconnectSessionNotify(clientSession);
                                    _authedClientSessions.Add(clientSession.SessionId, clientSession);
                                    return true;
                                }



                                if (_authedClientSessions.ContainsKey(clientSessionId) == true)
                                {
                                    _logger.Fatal(string.Format("[OnMessageCompleted] already exist authed session. clientSessionId: {0}", clientSessionId));
                                    return true;
                                }


                                // 인증 세션 목록에 추가
                                _authedClientSessions.Add(clientSessionId, clientSession);


                                if (ClientConnectedCallback != null)
                                {
                                    ClientConnectedCallback(clientSession, clientSession.DisconnectState);
                                }


                                // 세션 인증 응답 패킷 전송
                                SendInternalAuthSessionAck(clientSession);
                                return true;
                            }


                            // 재접속 요청시 재접속 대상 세션이 존재하지 않으므로 실패를 알린다.
                            if (clientNetState == ENetState.Reconnecting)
                            {
                                _logger.Info(string.Format("[OnMessageCompleted] reconnect session. clientSessionId: {0}", clientSessionId));


                                // 재접속이면 대기 목록에서 제외하고 인증된 세션목록에 추가한다.
                                ClientSession reconnectSession = _reconnectClientSessions.Find(x => x.SessionId == clientSessionId);
                                if (reconnectSession == null)
                                {
                                    _logger.Info(string.Format("[OnMessageCompleted] not found reconnect session. clientSessionId: {0}", clientSessionId));

                                    clientSession.NetState = ENetState.Reconnecting;
                                    clientSession.DisconnectState = EDisconnectState.Expired;
                                    SendInternalDisconnectSessionNotify(clientSession);
                                    _authedClientSessions.Add(clientSession.SessionId, clientSession);
                                    return true;
                                }


                                // 대기 목록에서 제거한다.
                                _reconnectClientSessions.Remove(reconnectSession);


                                // 기존 peer에 새로운 세션을 설정한다.
                                reconnectSession.GetPeer().SetClientSession(clientSession);


                                // 인증 세션 목록에 추가
                                _authedClientSessions.Add(clientSessionId, clientSession);


                                if (ClientReconnectedCallback != null)
                                {
                                    ClientReconnectedCallback(clientSession, clientSession.DisconnectState);
                                }


                                // 세션 인증 응답 패킷 전송
                                SendInternalAuthSessionAck(clientSession);
                            }
                        }
                    }
                    break;
                case EInternalProtocol.PAUSE_SESSION_REQ:
                    {
                        long timeTick = 0;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            timeTick = (long)bf.Deserialize(ms);
                        }


                        if (timeTick > DateTime.UtcNow.Ticks)
                        {
                            _logger.Error(string.Format("Invalid pause start time tick. time: {0}", timeTick));
                            return true;
                        }


                        clientSession.PauseStartTimeTick = timeTick;
                        
                        if (ClientPauseCallback != null)
                        {
                            ClientPauseCallback(clientSession, clientSession.DisconnectState);
                        }
                    }
                    break;
                case EInternalProtocol.RESUME_SESSION_REQ:
                    {
                        clientSession.PauseStartTimeTick = 0;

                        if (ClientResumeCallback != null)
                        {
                            ClientResumeCallback(clientSession, clientSession.DisconnectState);
                        }
                    }
                    break;
                default:
                    {
                        return false;
                    }
            }

            return true;
        }
    }
}
