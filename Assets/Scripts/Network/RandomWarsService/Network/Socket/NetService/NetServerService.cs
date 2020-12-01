using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using RandomWarsService.Core;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsService.Network.Socket.NetSession;


namespace RandomWarsService.Network.Socket.NetService
{
    public class NetServerService : NetBaseService
    {
        public ClientConnectDelegate ClientPauseCallback { get; set; }
        public ClientConnectDelegate ClientResumeCallback { get; set; }


        // 네트워크 상태 모니터링 핸들러
        protected MonitorHandler _netMonitorHandler;


        // 인증된 클라이언트 세션
        protected Dictionary<string, ClientSession> _authedClientSessions;


        // 재연결 대기 세션
        protected List<ClientSession> _reconnectClientSessions;


        // 만료된 클라이언트 세션
        protected List<string> _expiredClientSessions;


        private Listener _listener;


        // 메시지 송수신시 필요한 오브젝트
        private SocketAsyncEventArgsPool _receiveEventAragePool;
        private SocketAsyncEventArgsPool _sendEventAragePool;


        // 메시지 수신, 전송시 비동기 소켓에서 사용할 버퍼를 관리하는 객체
        private BufferHandler _bufferHandler;


        private readonly object _lockSessionContainer = new object();
        private readonly int preAllocCount = 2;
        private long _removeClientSessionTick;



        public NetServerService(PacketHandler packetHandler, ILog logger, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring)
            : base(packetHandler, logger, bufferSize, keepAliveTime, keepAliveInterval, onMonitoring)
        {
            _listener = new Listener();
            _listener.OnNewClientCallback += OnConnectedClient;


            // SocketAsyncEventArgs object pool 생성
            _receiveEventAragePool = new SocketAsyncEventArgsPool(maxConnection);
            _sendEventAragePool = new SocketAsyncEventArgsPool(maxConnection);



            // 버퍼 할당
            _bufferHandler = new BufferHandler(maxConnection * bufferSize * preAllocCount, bufferSize);
            _bufferHandler.InitBuffer();


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

                    _bufferHandler.SetBuffer(args);
                    _receiveEventAragePool.Push(args);
                }

                // send pool
                {
                    args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                    args.UserToken = clientSession;

                    _bufferHandler.SetBuffer(args);
                    _sendEventAragePool.Push(args);
                }
            }


            _authedClientSessions = new Dictionary<string, ClientSession>();
            _reconnectClientSessions = new List<ClientSession>();
            _expiredClientSessions = new List<string>();

            _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;

            _netMonitorHandler = new MonitorHandler(logger, 10);

        }


        public void Destroy()
        {
            Clear();
            _listener.Close();
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
                        if (clientSession.DisconnectState != ESessionState.None && clientSession.Socket.Connected == true)
                        {
                            // 접속 해제 상태이면 접속 해제를 먼저 알리고 세션 연결을 끊는다.
                            SendInternalDisconnectSessionNotify(clientSession);
                            clientSession.Disconnect();
                        }
                        else if (clientSession.ExpiredPauseTime(nowTick) == true)
                        {
                            // Pause 허용 시간 초과시 세션 연결을 끊는다.
                            clientSession.PauseLimitTimeTick = 0;
                            clientSession.Disconnect();
                        }
                    }
                }
            }
        }


        public void StartGameSession()
        {
            _netMonitorHandler.Start();
            _logger.Info(string.Format("[NetServerService] StartGameSession."));
        }


        public void EndGameSession()
        {
            _netMonitorHandler.End();
            _netMonitorHandler.ShowResult();
            _logger.Info(string.Format("[NetServerService] EndGameSession."));
        }

        /// <summary>
        /// 새로운 클라이언트가 접속 성공 했을 때 호출됩니다.
        /// AcceptAsync의 콜백 매소드에서 호출되며 여러 스레드에서 동시에 호출될 수 있기 때문에 공유자원에 접근할 때는 주의해야 합니다.
        /// </summary>
        /// <param name="client_socket"></param>
        /// <param name="token"></param>
        private void OnConnectedClient(System.Net.Sockets.Socket clientSocket, object token)
        {
            SocketAsyncEventArgs receiveArgs = _receiveEventAragePool.Pop();
            SocketAsyncEventArgs sendArgs = _sendEventAragePool.Pop();

            _logger.Info(string.Format("[OnConnectedClient] _receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));


            // 소켓 옵션 설정.
            clientSocket.LingerState = new LingerOption(true, 10);
            clientSocket.NoDelay = true;


            if (_authedClientSessions.Count == 0)
            {
                StartGameSession();
            }


            // 패킷 수신 시작
            ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            BeginReceive(clientSocket, receiveArgs, sendArgs);
        }


        protected override void OnCloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();


            lock (_lockSessionContainer)
            {
                _authedClientSessions.Remove(clientSession.SessionId);


                // 재연결 대기 목록에 추가한다.
                if (clientSession.DisconnectState == ESessionState.None
                    || clientSession.DisconnectState == ESessionState.Wait)
                {
                    clientSession.NetState = ENetState.Reconnecting;
                    clientSession.AliveTimeTick = DateTime.UtcNow.AddSeconds(15).Ticks;

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


                // 
                if (_authedClientSessions.Count == 0)
                {
                    EndGameSession();
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


        protected override void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            ClientSession clientSession = e.UserToken as ClientSession;
            byte[] msg = clientSession.ProcessSend(e);

            if (_onMonitoring == true)
            {
                _netMonitorHandler.SetSendPacket(clientSession.SessionId, msg);
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


            // 서비스 내부 패킷을 우선 처리한다.
            if (ProcessInternalPacket(clientSession, protocolId, msg, length) == false)
            {
                if (_packetHandler.InterceptProtocol != null)
                {
                    if (_packetHandler.InterceptProtocol(clientSession.GetPeer(), protocolId, msg, length) == true)
                    {
                        if (_onMonitoring == true)
                        {
                            _netMonitorHandler.SetReceivePacket(clientSession.SessionId, protocolId, msg, length);
                        }
                        return;
                    }
                }


                // 패킷 프로세서 큐에 추가한다.
                _packetHandler.EnqueuePacket(clientSession.GetPeer(), protocolId, msg, length);
            }


            if (_onMonitoring == true)
            {
                _netMonitorHandler.SetReceivePacket(clientSession.SessionId, protocolId, msg, length);
            }
        }


        protected override bool ProcessInternalPacket(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch ((EInternalProtocol)protocolId)
            {
                case EInternalProtocol.AUTH_SESSION_REQ:
                    {
                        string clientSessionId = string.Empty;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            clientSessionId = (string)bf.Deserialize(ms);
                        }


                        if (clientSessionId == string.Empty)
                        {
                            _logger.Fatal("OnMessageCompleted. clientSessionId is empty!!!");
                            return true;
                        }


                        // 세션 기본 정보 설정.
                        clientSession.SessionId = clientSessionId;
                        clientSession.DisconnectState = ESessionState.None;
                        clientSession.NetState = ENetState.Connected;


                        lock (_lockSessionContainer)
                        {
                            _logger.Info(string.Format("[OnMessageCompleted] connect session. clientSessionId: {0}", clientSessionId));


                            // 만료된 세션은 상태를 클라에게 전달하고 접속을 끊는다.
                            if (_expiredClientSessions.Contains(clientSessionId) == true)
                            {
                                _logger.Info(string.Format("[OnMessageCompleted] expired session. clientSessionId: {0}", clientSessionId));

                                clientSession.DisconnectState = ESessionState.Expired;
                                SendInternalDisconnectSessionNotify(clientSession);
                                _authedClientSessions.Add(clientSession.SessionId, clientSession);
                                return true;
                            }


                            // 중복 세션 접속시 기존 세션 접속을 종료시킨다.
                            ClientSession duplicatedSession = null;
                            if (_authedClientSessions.TryGetValue(clientSessionId, out duplicatedSession) == true)
                            {
                                _logger.Info(string.Format("[OnMessageCompleted] duplicated session. clientSessionId: {0}", clientSessionId));

                                // 중복 세션 접속 종료
                                _authedClientSessions.Remove(clientSessionId);
                                duplicatedSession.DisconnectState = ESessionState.Duplicated;
                                SendInternalDisconnectSessionNotify(duplicatedSession);
                                duplicatedSession.Disconnect();


                                // 기존 peer에 새로운 세션을 설정한다.
                                duplicatedSession.GetPeer().SetClientSession(clientSession);


                                if (ClientReconnectedCallback != null)
                                {
                                    ClientReconnectedCallback(clientSession, clientSession.DisconnectState);
                                }


                                // 세션 인증 응답 패킷 전송
                                SendInternalAuthSessionAck(clientSession, ENetState.Reconnected);
                            }
                            else
                            {
                                // 재접속이면 대기 목록에서 제외하고 인증된 세션목록에 추가한다.
                                ClientSession offlineSession = _reconnectClientSessions.Find(x => x.SessionId == clientSessionId);
                                if (offlineSession != null)
                                {
                                    // 대기 목록에서 제거한다.
                                    _reconnectClientSessions.Remove(offlineSession);


                                    // 기존 peer에 새로운 세션을 설정한다.
                                    offlineSession.GetPeer().SetClientSession(clientSession);


                                    if (ClientReconnectedCallback != null)
                                    {
                                        ClientReconnectedCallback(clientSession, clientSession.DisconnectState);
                                    }

                                    // 세션 인증 응답 패킷 전송
                                    SendInternalAuthSessionAck(clientSession, ENetState.Reconnected);
                                }
                                else
                                {
                                    // 신규 접속
                                    if (ClientConnectedCallback != null)
                                    {
                                        ClientConnectedCallback(clientSession, clientSession.DisconnectState);
                                    }

                                    // 세션 인증 응답 패킷 전송
                                    SendInternalAuthSessionAck(clientSession, ENetState.Connected);
                                }


                                if (_authedClientSessions.ContainsKey(clientSessionId) == true)
                                {
                                    _logger.Fatal(string.Format("[OnMessageCompleted] already exist authed session. clientSessionId: {0}", clientSessionId));
                                    return true;
                                }


                                // 인증 세션 목록에 추가
                                _authedClientSessions.Add(clientSessionId, clientSession);
                            }
                        }
                    }
                    break;
                case EInternalProtocol.PAUSE_SESSION_REQ:
                    {
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                        }


                        if (clientSession.OverPauseCount() == true)
                        {
                            clientSession.DisconnectState = ESessionState.Blocked;
                        }
                        else
                        {
                            clientSession.PauseLimitTimeTick = DateTime.UtcNow.AddSeconds(5).Ticks;

                            if (ClientPauseCallback != null)
                            {
                                ClientPauseCallback(clientSession, clientSession.DisconnectState);
                            }
                        }

                        SendInternalPauseSessionAck(clientSession);
                    }
                    break;
                case EInternalProtocol.RESUME_SESSION_REQ:
                    {
                        clientSession.PauseLimitTimeTick = 0;

                        if (ClientResumeCallback != null)
                        {
                            ClientResumeCallback(clientSession, clientSession.DisconnectState);
                        }

                        SendInternalResumeSessionAck(clientSession);
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
