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
            //     && _peer.ClientSession.NetState == ENetState.Reconnecting
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
                // ?????? ?????? ??????
                IPEndPoint remoteIpEndPoint = e.RemoteEndPoint as IPEndPoint;
                return;
            }


            Socket socket = sender as Socket;
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


            NetClientToken token = e.UserToken as NetClientToken;
            if (token == null)
            {
                return;
            }


            // ?????? ?????? ??????
            BeginReceive(socket, receiveArgs, sendArgs);


            // ???????????? ????????? ???????????? ????????? ?????? ????????? ????????????.
            clientSession.SessionId = token.PlayerSessionId;
            clientSession.NetState = token.NetState;
            clientSession.GameSession = token.GameSession;
            SendNetAuthSessionReq(clientSession);
        }


        public void Disconnect(Peer peer, ESessionState sessionState)
        {
            if (peer.ClientSession.NetState == ENetState.Disconnected)
            {
                return;
            }

            peer.ClientSession.SessionState = sessionState;
            peer.ClientSession.Disconnect();
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


            ClientSession clientSession = e.UserToken as ClientSession;
            clientSession.NetState = ENetState.Disconnected; 
            if (clientSession.SessionState == ESessionState.None)
            {
                clientSession.SessionState = ESessionState.Wait;
            }
            

            if (clientSession.GameSession != null)
            {
                clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.OFFLINE_CLIENT, null, 0);
            }
        }


        protected override void OnCompletedMessageCallback(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            if (clientSession == null)
            {
                return;
            }


            if (clientSession.GameSession == null)
            {
                return;
            }


            // ????????? ?????? ???????????? ????????????.
            bool f = ProcessNetServiceMessage(clientSession, protocolId, msg, length);
            if (f == false)
            {
                // ????????? ???????????? ????????????.
                if (clientSession.GameSession.PushRelayMessage(clientSession, protocolId, msg, length) == false)
                {
                    // ?????? ???????????? ????????????.
                    clientSession.GameSession.PushExternalMessage(clientSession.Peer, protocolId, msg, length);
                }
            }
        }


        protected override bool ProcessNetServiceMessage(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch((ENetProtocol)protocolId)
            {
                case ENetProtocol.AUTH_SESSION_ACK:
                {
                    string clientSessionId;
                    ENetState netState;
                    ESessionState sessionState;
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream(msg))
                    {
                        clientSessionId = (string)bf.Deserialize(ms);
                        netState = (ENetState)(byte)bf.Deserialize(ms);
                        sessionState = (ESessionState)(short)bf.Deserialize(ms);
                    }


                    clientSession.GameSession = _gameSession;
                    clientSession.SessionState = sessionState;
                    if (netState == ENetState.Connected)
                    {
                        if (clientSession.NetState == ENetState.Connecting)
                        {
                            clientSession.NetState = ENetState.Connected;
                        }
                        else
                        {
                            clientSession.NetState = ENetState.Reconnected;
                        }
                    }                        


                    clientSession.GameSession.PushInternalMessage(clientSession, EInternalProtocol.CONNECT_CLIENT, null, 0);
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
            SendNetPauseSessionReq(peer.ClientSession);
        }


        public void ResumeSession(Peer peer)
        {
            if (peer.ClientSession.PauseLimitTimeTick == 0)
            {
                return;
            }

            if (peer.ClientSession.PauseLimitTimeTick < DateTime.UtcNow.Ticks)
            {
                peer.ClientSession.Peer.Disconnect(ESessionState.Expired);
                return;
            }

            peer.ClientSession.NetState = ENetState.Connected;
            SendNetResumeSessionReq(peer.ClientSession);
        }
    }
}