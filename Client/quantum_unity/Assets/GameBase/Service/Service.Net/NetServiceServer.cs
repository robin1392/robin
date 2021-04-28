using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Service.Net
{
    public class NetServiceServer : NetServiceBase
    {
        // 인증된 클라이언트 세션
        protected Dictionary<string, ClientSession> _authedClientSessions;

        // 재연결 대기 세션
        protected List<ClientSession> _offlineClientSessions;

        // 만료된 클라이언트 세션
        protected List<string> _expiredClientSessions;

        // 소켓 리스너
        private SocketListener _listener;

        private long _offlineSessionUpdateTime;




        public NetServiceServer()
        {
            _listener = new SocketListener();
            _listener.ConnectedClientCallback += OnConnectedClientCallback;

            _authedClientSessions = new Dictionary<string, ClientSession>();
            _offlineClientSessions = new List<ClientSession>();
            _expiredClientSessions = new List<string>();
            
            _offlineSessionUpdateTime = 0;
        }


        public override void Init(NetServiceConfig config)
        {
            base.Init(config);
        }


        public void Destroy()
        {
            base.Clear();

            lock (_authedClientSessions)
            {
                // foreach (var session in _authedClientSessions.Values)
                // {
                //     if (_receiveEventAragePool != null)
                //     {
                //         _receiveEventAragePool.Push(session.ReceiveEventArgs);
                //     }

                //     if (_sendEventAragePool != null)
                //     {
                //         _sendEventAragePool.Push(session.SendEventArgs);
                //     }
                // }

                _authedClientSessions.Clear();
            }

            lock (_offlineClientSessions)
            {
                _offlineClientSessions.Clear();
            }
            
            lock (_expiredClientSessions)
            {
                _expiredClientSessions.Clear();
            }

            _offlineSessionUpdateTime = 0;
        }


        public bool Start()
        {
            return _listener.Start("0.0.0.0", _config.Port, _config.MaxConnectionNum);
        }


        public override void Update()
        {
            base.Update();

            UpdateDisconnectedClientSession();

            // offline 세션 갱신
            UpdateOfflineClientSession();

            // pause 세션 갱신
            UpdatePauseClientSession();
        }


        private void OnConnectedClientCallback(Socket clientSocket, object token)
        {
            // NetSocketAsyncEventArgs receiveArgs = _receiveEventAragePool.Pop();
            // NetSocketAsyncEventArgs sendArgs = _sendEventAragePool.Pop();
            ClientSession clientSession = new ClientSession(_config.BufferSize);
            clientSession.CompletedMessageCallback += OnCompletedMessageCallback;


            NetSocketAsyncEventArgs receiveArgs = new NetSocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnCompletedReceiveCallback);
            receiveArgs.UserToken = clientSession;
            receiveArgs.SetBuffer(new byte[_config.BufferSize], 0, _config.BufferSize);

            NetSocketAsyncEventArgs sendArgs = new NetSocketAsyncEventArgs();
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnCompletedSendCallback);
            sendArgs.UserToken = clientSession;
            sendArgs.SetBuffer(new byte[_config.BufferSize], 0, _config.BufferSize);


            // 소켓 옵션 설정.
            clientSocket.LingerState = new LingerOption(true, 10);
            clientSocket.NoDelay = true;


            // 패킷 수신 시작
            //ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            BeginReceive(clientSocket, receiveArgs, sendArgs);
        }


        protected override void OnCloseClientSocket(SocketAsyncEventArgs e)
        {
            ClientSession clientSession = e.UserToken as ClientSession;
            RemoveAuthedClientSession(clientSession);

            if (clientSession.SessionState == ESessionState.None
                || clientSession.SessionState == ESessionState.Wait)
            {
                AddOfflineClientSession(clientSession, 15);

                if (clientSession.GameSession != null)
                {
                    clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.OFFLINE_CLIENT, null, 0);
                }
            }
            else
            {
                if (clientSession.NetState != ENetState.Reconnecting)
                {
                    AddExpiredClientSession(clientSession);

                    if (clientSession.GameSession != null)
                    {
                        clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.EXPIRED_CLIENT, null, 0);
                    }
                }
            }

            clientSession.Clear();
            // if (_receiveEventAragePool != null)
            // {
            //     _receiveEventAragePool.Push(clientSession.ReceiveEventArgs);
            // }

            // if (_sendEventAragePool != null)
            // {
            //     _sendEventAragePool.Push(clientSession.SendEventArgs);
            // }
        }


        protected override void OnCompletedMessageCallback(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            if (clientSession == null)
            {
                return;
            }


            // 서비스 내부 메세지을 처리한다.
            if (ProcessNetServiceMessage(clientSession, protocolId, msg, length) == false)
            {
                // 릴레이 메세지를 처리한다.
                if (clientSession.GameSession.PushRelayMessage(clientSession, protocolId, msg, length) == false)
                {
                    // 외부 메세지를 처리한다.
                    clientSession.GameSession.PushExternalMessage(clientSession.Peer, protocolId, msg, length);
                }
            }
        }


        protected override bool ProcessNetServiceMessage(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch((ENetProtocol)protocolId)
            {
                case ENetProtocol.AUTH_SESSION_REQ:
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
                            break;
                        }

                        _logger.Debug(string.Format("[NetService] auth session. clientSessionId: {0}, clientNetState: {1}", clientSessionId, clientNetState));

                        // 세션 기본 정보 설정.
                        clientSession.SessionId = clientSessionId;
                        clientSession.SessionState = ESessionState.None;
                        clientSession.NetState = ENetState.Connected;


                        // 게임 세션 
                        clientSession.GameSession = _gameSession;
                        if (clientSession.GameSession == null)
                        {
                            // 세션 인증 실패
                            SendNetAuthSessionAck(clientSession);
                            break;
                        }


                        // 만료된 클라이언트 세션 체크
                        if (CheckExpiredClientSession(clientSession) == true)
                        {
                            break;
                        }


                        // 중복 클라이언트 세션 체크
                        if (CheckDuplicatedClientSession(clientSession) == true)
                        {
                            break;
                        }


                        ClientSession offlineSession = null;
                        if (GetOfflineClientSession(clientSessionId, out offlineSession) == true)
                        {
                            // 기존 peer에 새로운 세션을 설정한다.
                            offlineSession.Peer.SetClientSession(clientSession);


                            // 게임 세션에 클라이언트 재연결을 알리기 위해 내부 메세지를 푸시한다.
                            if (clientSession.GameSession != null)
                            {
                                clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.RECONNECT_CLIENT, null, 0);
                            }
                            
                        }
                        else
                        {
                            if (clientNetState != ENetState.Connecting)
                            {
                                clientSession.SessionState = ESessionState.Expired;
                                SendNetDisconnectNotify(clientSession);
                                clientSession.Disconnect();
                                break;
                            }


                            // 게임 세션에 클라이언트 접속을 알리기 위해 내부 메세지를 푸시한다.
                            clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.CONNECT_CLIENT, null, 0);
                        }


                        // 인증 클라이언트 세션 추가
                        if (AddAuthedClientSession(clientSession) == false)
                        {
                            break;
                        }

                        SendNetAuthSessionAck(clientSession);
                    }
                    break;
                case ENetProtocol.PAUSE_SESSION_REQ:
                    {
                        if (clientSession.OverPauseCount() == true)
                        {
                            clientSession.SessionState = ESessionState.Blocked;
                            SendNetDisconnectNotify(clientSession);
                            clientSession.Disconnect();
                        }
                        else
                        {
                            clientSession.PauseLimitTimeTick = DateTime.UtcNow.AddSeconds(5).Ticks;

                            if (clientSession.GameSession != null)
                            {
                                clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.PAUSE_CLIENT, null, 0);
                            }
                        }
                    }
                    break;
                case ENetProtocol.RESUME_SESSION_REQ:
                    {
                        clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.RESUME_CLIENT, null, 0);
                    }
                    break;
                default:
                    {
                        return false;
                    }
            }

            return true;
        }


        private bool AddExpiredClientSession(ClientSession clientSession)
        {
            lock (_expiredClientSessions)
            {
                if (_expiredClientSessions.Exists(x => x == clientSession.SessionId) == true)
                {
                    return false;
                }


                _expiredClientSessions.Add(clientSession.SessionId);
            }

            return true;
        }


        private bool CheckExpiredClientSession(ClientSession clientSession)
        {
            return false;
        }


        private bool CheckDuplicatedClientSession(ClientSession clientSession)
        {
            ClientSession duplicatedSession;
            if (GetAuthedClientSession(clientSession.SessionId, out duplicatedSession) == false)
            {
                return false;
            }


            // 중복 세션 접속 종료
            duplicatedSession.SessionState = ESessionState.Duplicated;
            SendNetDisconnectNotify(duplicatedSession);
            duplicatedSession.Disconnect();


            // 기존 peer에 새로운 세션을 설정한다.
            clientSession.SessionState = ESessionState.None;
            duplicatedSession.Peer.SetClientSession(clientSession);
            RemoveAuthedClientSession(clientSession);


            // 게임 세션에 클라이언트 재연결을 알리기 위해 내부 메세지를 푸시한다.
            if (clientSession.GameSession != null)
            {
                clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.RECONNECT_CLIENT, null, 0);
            }

            SendNetAuthSessionAck(clientSession);
            return true;
        }


        private bool AddAuthedClientSession(ClientSession clientSession)
        {
            lock (_authedClientSessions)
            {
                if (_authedClientSessions.ContainsKey(clientSession.SessionId) == true)
                {
                    return false;
                }
                _authedClientSessions.Add(clientSession.SessionId, clientSession);
            }
            return true;
        }


        private void RemoveAuthedClientSession(ClientSession clientSession)
        {
            lock (_authedClientSessions)
            {
                if (_authedClientSessions.ContainsKey(clientSession.SessionId) == false)
                {
                    return;
                }
                _authedClientSessions.Remove(clientSession.SessionId);
            }
        }


        private bool GetAuthedClientSession(string id, out ClientSession value)
        {
            lock (_authedClientSessions)
            {
                if (_authedClientSessions.TryGetValue(id, out value) == false)
                {
                    return false;
                }
                return true;
            }
        }


        private bool AddOfflineClientSession(ClientSession clientSession, int expireTime)
        {
            clientSession.NetState = ENetState.Reconnecting;
            clientSession.AliveTimeTick = DateTime.UtcNow.AddSeconds(expireTime).Ticks;

            if (_offlineSessionUpdateTime == 0)
            {
                _offlineSessionUpdateTime = DateTime.UtcNow.AddSeconds(10).Ticks;
            }

            lock (_offlineClientSessions)
            {
                if (_offlineClientSessions.Contains(clientSession) == true)
                {
                    return false;
                }

                _offlineClientSessions.Insert(0, clientSession);
            }

            return true;
        }


        private bool GetOfflineClientSession(string id, out ClientSession value)
        {
            lock (_offlineClientSessions)
            {
                value = _offlineClientSessions.Find(x => x.SessionId ==  id);
                if (value == null)
                {
                    return false;
                }

                _offlineClientSessions.Remove(value);
                return true;
            }
        }


        void UpdateDisconnectedClientSession()
        {
            lock (_authedClientSessions)
            {
                List<string> removeSession = new List<string>();
                foreach (var elem in _authedClientSessions)
                {
                    if (elem.Value.SessionState == ESessionState.Blocked)
                    {
                        SendNetDisconnectNotify(elem.Value);
                        elem.Value.Disconnect();

                        if (elem.Value.GameSession != null)
                        {
                            elem.Value.GameSession.PushInternalMessage(elem.Value, EInternalProtocol. DISCONNECT_CLIENT, null, 0);
                        }

                        removeSession.Add(elem.Value.SessionId);
                    }
                }


                foreach (var id in removeSession)
                {
                    _authedClientSessions.Remove(id);
                }
            }
        }


        // offline 클라이언트 세션 갱신처리
        private void UpdateOfflineClientSession()
        {
            long nowTick = DateTime.UtcNow.Ticks;
            if (_offlineSessionUpdateTime == 0 || nowTick < _offlineSessionUpdateTime)
            {
                return;
            }
            _offlineSessionUpdateTime = DateTime.UtcNow.AddSeconds(1).Ticks;


            lock (_offlineClientSessions)
            {
                if (_offlineClientSessions.Count == 0)
                {
                    return;
                }


                ClientSession clientSession = _offlineClientSessions[_offlineClientSessions.Count - 1];
                while (clientSession != null && clientSession.AliveTimeTick < nowTick)
                {
                    clientSession.NetState = ENetState.Disconnected;
                    clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.EXPIRED_CLIENT, null, 0);


                    _offlineClientSessions.RemoveAt(_offlineClientSessions.Count - 1);
                    if (_offlineClientSessions.Count == 0)
                    {
                        break;
                    }
                    clientSession = _offlineClientSessions[_offlineClientSessions.Count - 1];
                }
            }
        }

        private void UpdatePauseClientSession()
        {

        }     

   }
}