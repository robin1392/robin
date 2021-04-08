using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;

namespace Service.Net
{
    public class NetServiceClient : NetServiceBase, IDisposable
    {
        private DateTime _nowTime;
        private string _binarySerializePath;
        private long _playTimeStampTick;
        private long _reconnectTryTimeTick;
        private readonly int _reconnectCheckInterval = 5;
        private readonly int _reconnectCheckLimit = 10;


        public override void Init(NetServiceConfig config)
        {
            base.Init(config);

            _playTimeStampTick = 0;
            _reconnectTryTimeTick = 0;
        }


        public void Dispose()
        {
            
        }   


        public override void Update() 
        {

            base.Update();
            _nowTime = DateTime.UtcNow;

            // if (_peer != null
            //     && _peer.userToken.NetState == ENetState.Reconnecting
            //     && _reconnectTryTimeTick != 0
            //     && _reconnectTryTimeTick < _nowTime.Ticks)
            // {

            //     _reconnectTryTimeTick = _nowTime.AddSeconds(3).Ticks;
            //     Connect(_gameSession.ServerAddr, 
            //         _gameSession.Port,
            //         _gameSession.Id, 
            //         _playerSessionId, 
            //         ENetState.Reconnecting);
            // }


            // if (IsConnected() == true)
            // {
            //     if (_playTimeStampTick < _nowTime.Ticks)
            //     {
            //         _playTimeStampTick = _nowTime.AddSeconds(_reconnectCheckInterval).Ticks;
            //         BinarySerialize();
            //     }
            // }

        }

        //public bool IsConnected()
        //{
        //    return _peer.IsConnected();
        //}


        //public bool CheckReconnection()
        //{
        //    if (BinaryDeserialize() == false)
        //    {
        //        return false;
        //    }


        //    if (_playTimeStampTick + (_reconnectCheckLimit * TimeSpan.TicksPerSecond) < DateTime.UtcNow.Ticks)
        //    {
        //        return false;
        //    }


        //    if (_gameSession == null)
        //    {

        //    }

        //    _reconnectTryTimeTick = _nowTime.AddSeconds(3).Ticks;
        //    Connect(_serverAddr, _port, _gameSessionId, _playerSessionId, ENetState.Reconnecting);


        //    // if (ClientReconnectingCallback != null)
        //    // {
        //    //     ClientReconnectingCallback();
        //    // }

        //    return true;
        //}


        public void Connect(string serverAddr, int port, string playerSessionId, ENetState netState)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.LingerState = new LingerOption(true, 10);
            socket.NoDelay = true;


            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompletedCallback;
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), port);
            args.UserToken = new NetClientToken 
            { 
                ServerAddr = serverAddr,
                Port = port,
                PlayerSessionId = playerSessionId,
                NetState = netState,
                PlayTimeStampTick = DateTime.UtcNow.Ticks,
                GameSession = _gameSession,
            };

            bool pending = socket.ConnectAsync(args);
            if (pending == false)
            {
                OnConnectCompletedCallback(socket, args);
            }
        }


        void OnConnectCompletedCallback(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                // 연결 에러 처리
                IPEndPoint remoteIpEndPoint = e.RemoteEndPoint as IPEndPoint;
                return;
            }


            Socket socket = sender as Socket;
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


            NetClientToken token = e.UserToken as NetClientToken;
            if (token == null)
            {
                return;
            }


            // 패킷 수신 시작
            BeginReceive(socket, receiveArgs, sendArgs);


            // 서버와의 연결이 성공하면 서버로 세션 상태를 요청한다.
            userToken.SessionId = token.PlayerSessionId;
            userToken.NetState = token.NetState;
            userToken.GameSession = token.GameSession;
            SendNetAuthSessionReq(userToken);
        }


        public void Disconnect(Peer peer, ESessionState sessionState)
        {
            if (peer.UserToken.NetState == ENetState.Disconnected)
            {
                return;
            }

            peer.UserToken.SessionState = sessionState;
            peer.UserToken.Disconnect();
        }


        protected override void OnCloseClientSocket(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.NotConnected)
            {
                // try reconnect
                NetClientToken token = e.UserToken as NetClientToken;
                if (token == null)
                {
                    return;
                }

                Connect(token.ServerAddr, token.Port, token.PlayerSessionId, ENetState.Reconnecting);
                return;
            }


            UserToken userToken = e.UserToken as UserToken;
            userToken.NetState = ENetState.Disconnected; 
            if (userToken.SessionState == ESessionState.None)
            {
                userToken.SessionState = ESessionState.Wait;
            }
            

            if (userToken.GameSession != null)
            {
                userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.OFFLINE_CLIENT, null, 0);
            }
        }


        protected override void OnCompletedMessageCallback(UserToken userToken, int protocolId, byte[] msg, int length)
        {
            if (userToken == null)
            {
                return;
            }


            if (userToken.GameSession == null)
            {
                return;
            }


            // 서비스 내부 메세지을 처리한다.
            bool f = ProcessNetServiceMessage(userToken, protocolId, msg, length);
            if (f == false)
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
                case ENetProtocol.AUTH_SESSION_ACK:
                {
                    string userTokenId;
                    ENetState netState;
                    ESessionState sessionState;
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream(msg))
                    {
                        userTokenId = (string)bf.Deserialize(ms);
                        netState = (ENetState)(byte)bf.Deserialize(ms);
                        sessionState = (ESessionState)(short)bf.Deserialize(ms);
                    }


                    userToken.GameSession = _gameSession;
                    userToken.SessionState = sessionState;
                    if (netState == ENetState.Connected)
                    {
                        if (userToken.NetState == ENetState.Connecting)
                        {
                            userToken.NetState = ENetState.Connected;
                        }
                        else
                        {
                            userToken.NetState = ENetState.Reconnected;
                        }
                    }                        


                    userToken.GameSession.PushInternalMessage(userToken.Peer, EInternalProtocol.CONNECT_CLIENT, null, 0);
                }
                break;
                default:
                {
                    return false;
                }
            }


            return true;
        }


        public void PauseSession(Peer peer)
        {
            SendNetPauseSessionReq(peer.UserToken);
        }


        public void ResumeSession(Peer peer)
        {
            if (peer.UserToken.PauseLimitTimeTick == 0)
            {
                return;
            }

            if (peer.UserToken.PauseLimitTimeTick < DateTime.UtcNow.Ticks)
            {
                peer.UserToken.Peer.Disconnect(ESessionState.Expired);
                return;
            }

            peer.UserToken.NetState = ENetState.Connected;
            SendNetResumeSessionReq(peer.UserToken);
        }
    }
}