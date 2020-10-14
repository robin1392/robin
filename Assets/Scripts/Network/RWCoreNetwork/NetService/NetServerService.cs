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


        // 대기 세션
        protected List<ClientSession> _reconnectClientSessions;


        // 만료된 세션
        protected Dictionary<string, ClientSession> _expiredClientSessions;


        // 인증된 클라이언트 세션
        protected Dictionary<string, ClientSession> _authedClientSessions;


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
            _bufferManager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
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
            _expiredClientSessions = new Dictionary<string, ClientSession>();

            _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;
        }


        public override void Clear()
        {
            base.Clear();

            lock (_authedClientSessions)
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
            }


            lock (_reconnectClientSessions)
            {

                foreach (var session in _reconnectClientSessions)
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
                _reconnectClientSessions.Clear();
            }


            lock (_expiredClientSessions)
            {
              foreach (var session in _expiredClientSessions.Values)
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
                _expiredClientSessions.Clear();
            }

            _logger.Info(string.Format("_receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));
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

            _logger.Info(string.Format("_receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));

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
            if (_removeClientSessionTick < nowTick)
            {
                _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;

                lock (_reconnectClientSessions)
                {
                    List<ClientSession> removeSessions = new List<ClientSession>();
                    if (_reconnectClientSessions.Count > 0)
                    {
                        ClientSession clientSession = _reconnectClientSessions.Last();
                        while (clientSession != null && clientSession.AliveTimeTick < nowTick)
                        {
                            _reconnectClientSessions.RemoveAt(_reconnectClientSessions.Count - 1);


                            clientSession.NetState = ENetState.Disconnected;
                            if (ClientDisconnectedCallback != null)
                            {
                                ClientDisconnectedCallback(clientSession, clientSession.SessionState);
                            }


                            removeSessions.Add(clientSession);
                            if (_reconnectClientSessions.Count == 0)
                            {
                                break;
                            }
                            clientSession = _reconnectClientSessions.Last();
                        }
                    }


                    if (removeSessions.Count > 0)
                    {
                        foreach (var elem in removeSessions)
                        {
                            lock (_expiredClientSessions)
                            {
                                _expiredClientSessions.Add(elem.SessionId, elem);
                            }
                            

                            if (elem.ReceiveEventArgs != null)
                            {
                                _receiveEventAragePool.Push(elem.ReceiveEventArgs);
                            }

                            if (elem.SendEventArgs != null)
                            {
                                _sendEventAragePool.Push(elem.SendEventArgs);
                            }
                        }

                        _logger.Info(string.Format("_receiveEventAragePool: {0}, _sendEventAragePool: {1}", _receiveEventAragePool.Count, _sendEventAragePool.Count));
                    }
                }
            }


            lock (_authedClientSessions)
            {
                foreach (var elem in _authedClientSessions.Values)
                {
                    if (elem.SessionState != ESessionState.None && elem.Socket.Connected == true)
                    {
                        elem.Disconnect();
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


            if (protocolId == (int)EInternalProtocol.AUTH_CLIENT_SESSION_REQ)
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
                    return; 
                }


                clientSession.SessionId = clientSessionId;
                clientSession.SessionState = ESessionState.None;
                clientSession.NetState = ENetState.Connected;


                // 만료된 세션은 접속을 허용하지 않는다.
                lock (_expiredClientSessions)
                {
                    if (_expiredClientSessions.ContainsKey(clientSessionId) == true)
                    {
                        _logger.Info(string.Format("[OnMessageCompleted] expired session. clientSessionId: {0}", clientSessionId));
                        clientSession.GetPeer().Disconnect(ESessionState.Expired);
                        return;
                    }
                }


                // 중복 세션 접속시 기존 세션 접속을 종료시킨다.
                ClientSession duplicatedSession = null;
                if (_authedClientSessions.TryGetValue(clientSessionId, out duplicatedSession) == true)
                {
                    _logger.Info(string.Format("[OnMessageCompleted] duplicated session. clientSessionId: {0}", clientSessionId));


                    lock (_authedClientSessions)
                    {
                        // 인증 세션 목록에서 제거한다.
                        _authedClientSessions.Remove(duplicatedSession.SessionId);
                    }

                    
                    // 중복 세션 접속 종료
                    duplicatedSession.GetPeer().Disconnect(ESessionState.Duplicated);
                }


                // 신규 접속 요청 처리
                if (clientNetState == ENetState.Connecting)
                {
                    _logger.Info(string.Format("[OnMessageCompleted] connect session. clientSessionId: {0}", clientSessionId));


                    lock (_authedClientSessions)
                    {
                        if (_authedClientSessions.ContainsKey(clientSessionId) == true)
                        {
                            _logger.Fatal(string.Format("[OnMessageCompleted] already exist authed session. clientSessionId: {0}", clientSessionId));
                            return;
                        }


                       // 인증 세션 목록에 추가
                        _authedClientSessions.Add(clientSessionId, clientSession);
                    }
         

                    if (ClientConnectedCallback != null)
                    {
                        ClientConnectedCallback(clientSession, clientSession.SessionState);
                    }


                    // 세션 인증 응답 패킷 전송
                    clientSession.SendInternalAuthSessionAck(clientSession.NetState);
                    return;
                }



                // 재접속 요청시 재접속 대상 세션이 존재하지 않으므로 실패를 알린다.
                if (clientNetState == ENetState.Reconnecting)
                {
                    _logger.Info(string.Format("[OnMessageCompleted] reconnect session. clientSessionId: {0}", clientSessionId));

                    ClientSession reconnectSession = null;
                    lock (_reconnectClientSessions)
                    {
                        // 재접속이면 대기 목록에서 제외하고 인증된 세션목록에 추가한다.
                        reconnectSession = _reconnectClientSessions.Find(x => x.SessionId == clientSessionId);
                        if (reconnectSession == null)
                        {
                            clientSession.GetPeer().Disconnect(ESessionState.Expired);
                            return;
                        }


                        // 대기 목록에서 제거한다.
                        _reconnectClientSessions.Remove(reconnectSession);
                    }



                    // 기존 peer에 새로운 세션을 설정한다.
                    reconnectSession.GetPeer().SetClientSession(clientSession);


                    lock (_authedClientSessions)
                    {
                        // 인증 세션 목록에 추가
                        _authedClientSessions.Add(clientSessionId, clientSession);
                    }
                    

                    if (ClientReconnectedCallback != null)
                    {
                        ClientReconnectedCallback(clientSession, clientSession.SessionState);
                    }


                    // 기존 세션 사용 리소스는 pool에 반환한다.
                    if (_receiveEventAragePool != null)
                    {
                        _receiveEventAragePool.Push(reconnectSession.ReceiveEventArgs);
                    }

                    if (_sendEventAragePool != null)
                    {
                        _sendEventAragePool.Push(reconnectSession.SendEventArgs);
                    }


                    // 세션 인증 응답 패킷 전송
                    clientSession.SendInternalAuthSessionAck(clientSession.NetState);
                }

                return;
            }

            if (protocolId == (int)EInternalProtocol.DISCONNECT_SESSION_CONFIRM)
            {
                ESessionState clientSessionState;
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(msg))
                {
                    clientSessionState = (ESessionState)(short)bf.Deserialize(ms);
                }

                //clientSession.Disconnect();
                return;
            }


            if (_onRelayQueue == false)
            {
                if (_packetHandler.InterceptProtocol != null)
                {
                    if (_packetHandler.InterceptProtocol(clientSession.GetPeer(), protocolId, msg, length) == true)
                    {
                        return;
                    }
                }
            }


            // 패킷처리 큐에 추가한다.
            _packetHandler.EnqueuePacket(clientSession.GetPeer(), protocolId, msg, length);
        }


        protected override void CloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();


            lock (_authedClientSessions)
            {
                _authedClientSessions.Remove(clientSession.SessionId);
            }


            if (clientSession.SessionState == ESessionState.None || clientSession.SessionState == ESessionState.Wait)
            {
                clientSession.NetState = ENetState.Reconnecting;
                clientSession.AliveTimeTick = DateTime.UtcNow.AddSeconds(30).Ticks;

                lock (_reconnectClientSessions)
                {
                    _reconnectClientSessions.Insert(0, clientSession);
                }

                _logger.Info(string.Format("[CloseClientsocket] reconnect session. clientSessionId: {0}", clientSession.SessionId));
            }
            else
            {
                clientSession.NetState = ENetState.Disconnected;

                lock (_expiredClientSessions)
                {
                    _expiredClientSessions.Add(clientSession.SessionId, clientSession);
                }

                _logger.Info(string.Format("[CloseClientsocket] disconnect session. clientSessionId: {0}", clientSession.SessionId));
            }


            if (ClientOfflineCallback != null && clientSession.GetPeer() != null)
            {
                ClientOfflineCallback(clientSession, clientSession.SessionState);
            }
        }


    }
}
