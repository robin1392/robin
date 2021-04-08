using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Service.Core;


namespace Service.Net
{
    public class NetServiceServer : NetServiceBase
    {
        // 인증된 클라이언트 세션
        protected Dictionary<string, UserToken> _autheduserTokens;

        // 재연결 대기 세션
        protected List<UserToken> _offlineuserTokens;

        // 만료된 클라이언트 세션
        protected List<string> _expireduserTokens;

        // 소켓 리스너
        private SocketListener _listener;

        private long _offlineSessionUpdateTime;




        public NetServiceServer()
        {
            _listener = new SocketListener();
            _listener.ConnectedClientCallback += OnConnectedClientCallback;

            _autheduserTokens = new Dictionary<string, UserToken>();
            _offlineuserTokens = new List<UserToken>();
            _expireduserTokens = new List<string>();
            
            _offlineSessionUpdateTime = 0;
        }


        public override void Init(NetServiceConfig config)
        {
            base.Init(config);
        }


        public void Destroy()
        {
            base.Clear();

            lock (_autheduserTokens)
            {
                // foreach (var session in _autheduserTokens.Values)
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

                _autheduserTokens.Clear();
            }

            lock (_offlineuserTokens)
            {
                _offlineuserTokens.Clear();
            }
            
            lock (_expireduserTokens)
            {
                _expireduserTokens.Clear();
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

            UpdateDisconnecteduserToken();

            // offline 세션 갱신
            UpdateOfflineuserToken();

            // pause 세션 갱신
            UpdatePauseuserToken();
        }


        private void OnConnectedClientCallback(Socket clientSocket, object token)
        {
            // NetSocketAsyncEventArgs receiveArgs = _receiveEventAragePool.Pop();
            // NetSocketAsyncEventArgs sendArgs = _sendEventAragePool.Pop();
            UserToken userToken = new UserToken(_config.BufferSize);
            userToken.CompletedMessageCallback += OnCompletedMessageCallback;


            NetSocketAsyncEventArgs receiveArgs = new NetSocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnCompletedReceiveCallback);
            receiveArgs.UserToken = userToken;
            receiveArgs.SetBuffer(new byte[_config.BufferSize], 0, _config.BufferSize);

            NetSocketAsyncEventArgs sendArgs = new NetSocketAsyncEventArgs();
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnCompletedSendCallback);
            sendArgs.UserToken = userToken;
            sendArgs.SetBuffer(new byte[_config.BufferSize], 0, _config.BufferSize);


            // 소켓 옵션 설정.
            clientSocket.LingerState = new LingerOption(true, 10);
            clientSocket.NoDelay = true;


            // 패킷 수신 시작
            //userToken userToken = receiveArgs.UserToken as userToken;
            BeginReceive(clientSocket, receiveArgs, sendArgs);
        }


        protected override void OnCloseClientSocket(SocketAsyncEventArgs e)
        {
            UserToken userToken = e.UserToken as UserToken;
            RemoveAutheduserToken(userToken);

            if (userToken.SessionState == ESessionState.None
                || userToken.SessionState == ESessionState.Wait)
            {
                AddOfflineuserToken(userToken, 15);

                if (userToken.GameSession != null)
                {
                    userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.OFFLINE_CLIENT, null, 0);
                }
            }
            else
            {
                if (userToken.NetState != ENetState.Reconnecting)
                {
                    AddExpireduserToken(userToken);

                    if (userToken.GameSession != null)
                    {
                        userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.EXPIRED_CLIENT, null, 0);
                    }
                }
            }

            userToken.Clear();
            // if (_receiveEventAragePool != null)
            // {
            //     _receiveEventAragePool.Push(userToken.ReceiveEventArgs);
            // }

            // if (_sendEventAragePool != null)
            // {
            //     _sendEventAragePool.Push(userToken.SendEventArgs);
            // }
        }


        protected override void OnCompletedMessageCallback(UserToken userToken, int protocolId, byte[] msg, int length)
        {
            if (userToken == null)
            {
                return;
            }


            // 서비스 내부 메세지을 처리한다.
            if (ProcessNetServiceMessage(userToken, protocolId, msg, length) == false)
            {
                // 릴레이 메세지를 처리한다.
                if (userToken.GameSession.PushRelayMessage(userToken.Peer, protocolId, msg, length) == false)
                {
                    // 외부 메세지를 처리한다.
                    userToken.GameSession.PushExternalMessage(userToken.Peer, protocolId, msg, length);
                }
            }
        }


        protected override bool ProcessNetServiceMessage(UserToken userToken, int protocolId, byte[] msg, int length)
        {
            switch((ENetProtocol)protocolId)
            {
                case ENetProtocol.AUTH_SESSION_REQ:
                    {
                        string userTokenId = string.Empty;
                        ENetState clientNetState = ENetState.End;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            userTokenId = (string)bf.Deserialize(ms);
                            clientNetState = (ENetState)(byte)bf.Deserialize(ms);
                        }

                        if (userTokenId == string.Empty)
                        {
                            break;
                        }

                        Logger.Debug(string.Format("[NetService] auth session. userTokenId: {0}, clientNetState: {1}", userTokenId, clientNetState));

                        // 세션 기본 정보 설정.
                        userToken.SessionId = userTokenId;
                        userToken.SessionState = ESessionState.None;
                        userToken.NetState = ENetState.Connected;


                        // 게임 세션 
                        userToken.GameSession = _gameSession;
                        if (userToken.GameSession == null)
                        {
                            // 세션 인증 실패
                            SendNetAuthSessionAck(userToken);
                            break;
                        }


                        // 만료된 클라이언트 세션 체크
                        if (CheckExpireduserToken(userToken) == true)
                        {
                            break;
                        }


                        // 중복 클라이언트 세션 체크
                        if (CheckDuplicateduserToken(userToken) == true)
                        {
                            break;
                        }


                        UserToken offlineSession = null;
                        if (GetOfflineuserToken(userTokenId, out offlineSession) == true)
                        {
                            // 기존 peer에 새로운 세션을 설정한다.
                            offlineSession.Peer.SetUserToken(userToken);


                            // 게임 세션에 클라이언트 재연결을 알리기 위해 내부 메세지를 푸시한다.
                            if (userToken.GameSession != null)
                            {
                                userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.RECONNECT_CLIENT, null, 0);
                            }
                            
                        }
                        else
                        {
                            if (clientNetState != ENetState.Connecting)
                            {
                                userToken.SessionState = ESessionState.Expired;
                                SendNetDisconnectNotify(userToken);
                                userToken.Disconnect();
                                break;
                            }


                            // 게임 세션에 클라이언트 접속을 알리기 위해 내부 메세지를 푸시한다.
                            userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.CONNECT_CLIENT, null, 0);
                        }


                        // 인증 클라이언트 세션 추가
                        if (AddAutheduserToken(userToken) == false)
                        {
                            break;
                        }

                        SendNetAuthSessionAck(userToken);
                    }
                    break;
                case ENetProtocol.PAUSE_SESSION_REQ:
                    {
                        if (userToken.OverPauseCount() == true)
                        {
                            userToken.SessionState = ESessionState.Blocked;
                            SendNetDisconnectNotify(userToken);
                            userToken.Disconnect();
                        }
                        else
                        {
                            userToken.PauseLimitTimeTick = DateTime.UtcNow.AddSeconds(5).Ticks;

                            if (userToken.GameSession != null)
                            {
                                userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.PAUSE_CLIENT, null, 0);
                            }
                        }
                    }
                    break;
                case ENetProtocol.RESUME_SESSION_REQ:
                    {
                        userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.RESUME_CLIENT, null, 0);
                    }
                    break;
                default:
                    {
                        return false;
                    }
            }

            return true;
        }


        private bool AddExpireduserToken(UserToken userToken)
        {
            lock (_expireduserTokens)
            {
                if (_expireduserTokens.Exists(x => x == userToken.SessionId) == true)
                {
                    return false;
                }


                _expireduserTokens.Add(userToken.SessionId);
            }

            return true;
        }


        private bool CheckExpireduserToken(UserToken userToken)
        {
            return false;
        }


        private bool CheckDuplicateduserToken(UserToken userToken)
        {
            UserToken duplicatedSession;
            if (GetAutheduserToken(userToken.SessionId, out duplicatedSession) == false)
            {
                return false;
            }


            // 중복 세션 접속 종료
            duplicatedSession.SessionState = ESessionState.Duplicated;
            SendNetDisconnectNotify(duplicatedSession);
            duplicatedSession.Disconnect();


            // 기존 peer에 새로운 세션을 설정한다.
            userToken.SessionState = ESessionState.None;
            duplicatedSession.Peer.SetUserToken(userToken);
            RemoveAutheduserToken(userToken);


            // 게임 세션에 클라이언트 재연결을 알리기 위해 내부 메세지를 푸시한다.
            if (userToken.GameSession != null)
            {
                userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.RECONNECT_CLIENT, null, 0);
            }

            SendNetAuthSessionAck(userToken);
            return true;
        }


        private bool AddAutheduserToken(UserToken userToken)
        {
            lock (_autheduserTokens)
            {
                if (_autheduserTokens.ContainsKey(userToken.SessionId) == true)
                {
                    return false;
                }
                _autheduserTokens.Add(userToken.SessionId, userToken);
            }
            return true;
        }


        private void RemoveAutheduserToken(UserToken userToken)
        {
            lock (_autheduserTokens)
            {
                if (_autheduserTokens.ContainsKey(userToken.SessionId) == false)
                {
                    return;
                }
                _autheduserTokens.Remove(userToken.SessionId);
            }
        }


        private bool GetAutheduserToken(string id, out UserToken value)
        {
            lock (_autheduserTokens)
            {
                if (_autheduserTokens.TryGetValue(id, out value) == false)
                {
                    return false;
                }
                return true;
            }
        }


        private bool AddOfflineuserToken(UserToken userToken, int expireTime)
        {
            userToken.NetState = ENetState.Reconnecting;
            userToken.AliveTimeTick = DateTime.UtcNow.AddSeconds(expireTime).Ticks;

            if (_offlineSessionUpdateTime == 0)
            {
                _offlineSessionUpdateTime = DateTime.UtcNow.AddSeconds(10).Ticks;
            }

            lock (_offlineuserTokens)
            {
                if (_offlineuserTokens.Contains(userToken) == true)
                {
                    return false;
                }

                _offlineuserTokens.Insert(0, userToken);
            }

            return true;
        }


        private bool GetOfflineuserToken(string id, out UserToken value)
        {
            lock (_offlineuserTokens)
            {
                value = _offlineuserTokens.Find(x => x.SessionId ==  id);
                if (value == null)
                {
                    return false;
                }

                _offlineuserTokens.Remove(value);
                return true;
            }
        }


        void UpdateDisconnecteduserToken()
        {
            lock (_autheduserTokens)
            {
                List<string> removeSession = new List<string>();
                foreach (var elem in _autheduserTokens)
                {
                    if (elem.Value.SessionState == ESessionState.Blocked)
                    {
                        SendNetDisconnectNotify(elem.Value);
                        elem.Value.Disconnect();

                        if (elem.Value.GameSession != null)
                        {
                            elem.Value.GameSession.PushInternalMessage(elem.Value.Peer, EInternalProtocol. DISCONNECT_CLIENT, null, 0);
                        }

                        removeSession.Add(elem.Value.SessionId);
                    }
                }


                foreach (var id in removeSession)
                {
                    _autheduserTokens.Remove(id);
                }
            }
        }


        // offline 클라이언트 세션 갱신처리
        private void UpdateOfflineuserToken()
        {
            long nowTick = DateTime.UtcNow.Ticks;
            if (_offlineSessionUpdateTime == 0 || nowTick < _offlineSessionUpdateTime)
            {
                return;
            }
            _offlineSessionUpdateTime = DateTime.UtcNow.AddSeconds(1).Ticks;


            lock (_offlineuserTokens)
            {
                if (_offlineuserTokens.Count == 0)
                {
                    return;
                }


                UserToken userToken = _offlineuserTokens[_offlineuserTokens.Count - 1];
                while (userToken != null && userToken.AliveTimeTick < nowTick)
                {
                    userToken.NetState = ENetState.Disconnected;
                    userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.EXPIRED_CLIENT, null, 0);


                    _offlineuserTokens.RemoveAt(_offlineuserTokens.Count - 1);
                    if (_offlineuserTokens.Count == 0)
                    {
                        break;
                    }
                    userToken = _offlineuserTokens[_offlineuserTokens.Count - 1];
                }
            }
        }

        private void UpdatePauseuserToken()
        {

        }     

   }
}