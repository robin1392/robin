﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using RandomWarsService.Core;
using RandomWarsService.Network.Socket.NetPacket;
using RandomWarsService.Network.Socket.NetSession;

namespace RandomWarsService.Network.Socket.NetService
{
    public class NetClientService : NetBaseService
    {
        public delegate void ClientReconnectingDelegate();
        public ClientReconnectingDelegate ClientReconnectingCallback { get; set; }


        private ClientSession _clientSession;

        // 네트워크 상태 큐
        private Queue<ClientSession> _netEventQueue;


        private string _binarySerializePath;
        private string _gameSessionId;
        private string _playerSessionId;
        private string _serverAddr;
        private int _port;
        private DateTime _reconnectCheckTime;
        private readonly int _reconnectCheckInterval = 5;
        private readonly int _reconnectCheckLimit = 10;



        public NetClientService(PacketHandler packetHandler, ILog logger, int maxConnection, int bufferSize, int keepAliveTime, int keepAliveInterval, bool onMonitoring, string binarySerializePath)
            : base(packetHandler, logger, bufferSize, keepAliveTime, keepAliveInterval, onMonitoring)
        {
            _netEventQueue = new Queue<ClientSession>();

            _clientSession = new ClientSession(_logger, _bufferSize);
            _clientSession.CompletedMessageCallback += OnMessageCompleted;


            _binarySerializePath = binarySerializePath;
            _serverAddr = string.Empty;
            _gameSessionId = string.Empty;
            _playerSessionId = string.Empty;
            _port = 0;
            _reconnectCheckTime = DateTime.UtcNow.AddSeconds(_reconnectCheckInterval);
        }

        public void Clear()
        {

        }


        public override void Update()
        {
            // 연결 이벤트 처리
            ProcessConnectionEvent();

            if (IsConnected() == true)
            {
                // 패킷을 처리한다.
                _packetHandler.Update();


                DateTime now = DateTime.UtcNow;
                if (_reconnectCheckTime < now)
                {
                    _reconnectCheckTime = now.AddSeconds(_reconnectCheckInterval);
                    BinarySerialize();
                }
            }
        }


        private void BinarySerialize()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "serverAddr", _serverAddr },
                { "port", _port.ToString()},
                { "gameSessionId", _gameSessionId },
                { "playerSessionId", _playerSessionId },
                { "reconnectCheckTime", _reconnectCheckTime.ToString() },
            };


            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_binarySerializePath, FileMode.OpenOrCreate);
            formatter.Serialize(stream, values);
            stream.Close();
        }


        private bool BinaryDeserialize()
        {
            if (File.Exists(_binarySerializePath) == false)
            {
                _logger.Debug(string.Format("[NetClient] BinaryDeserialize. Not exist file. path: {0}", _binarySerializePath));
                return false;
            }


            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_binarySerializePath, FileMode.Open);
            Dictionary<string, string> values = (Dictionary<string, string>)formatter.Deserialize(stream);

            _serverAddr = values["serverAddr"];
            _port = int.Parse(values["port"]);
            _gameSessionId = values["gameSessionId"];
            _playerSessionId = values["playerSessionId"];
            _reconnectCheckTime = DateTime.Parse(values["reconnectCheckTime"]);

            stream.Close();
            return true;
        }


        public bool IsConnected()
        {
            if (_clientSession == null || _clientSession.Socket == null)
            {
                return false;
            }

            return _clientSession.Socket.Connected;
        }


        public bool CheckReconnection()
        {
            _logger.Debug(string.Format("[NetClient] CheckReconnection. sessionState: {0}", _clientSession.DisconnectState));

            if (BinaryDeserialize() == false)
            {
                return false;
            }


            if (_reconnectCheckTime.AddSeconds(_reconnectCheckLimit) < DateTime.UtcNow)
            {
                _logger.Debug(string.Format("[NetClient] check time. {0}", _reconnectCheckTime.AddSeconds(_reconnectCheckLimit).ToString()));
                return false;
            }


            Connect(_serverAddr, _port, _playerSessionId);

            if (ClientReconnectingCallback != null)
            {
                ClientReconnectingCallback();
            }
            return true;
        }


        /// <summary>
        /// N개의 클라이언트를 서버에 연결한다.
        /// </summary>
        /// <param name="serverAddr"></param>
        /// <param name="port"></param>
        public void Connect(string serverAddr, int port, string playerSessionId)
        {
            if (_clientSession.Socket != null)
            {
                _logger.Debug(string.Format("[NetClient] Close socket. socketHandle. {0}", _clientSession.Socket.Handle));
                _clientSession.Socket.Close();
                _clientSession.Socket = null;
            }

            System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.LingerState = new LingerOption(true, 10);
            socket.NoDelay = true;
            socket.SendTimeout = 2000;
            socket.ReceiveTimeout = 2000;


            _clientSession.SessionId = playerSessionId;
            _serverAddr = serverAddr;
            _gameSessionId = string.Empty;
            _playerSessionId = playerSessionId;
            _port = port;
            _reconnectCheckTime = DateTime.UtcNow;


            _logger.Debug(string.Format("[NetClient] Connect to server. addr: {0}:{1}, playerSessionId: {2}, gameSessionId: {3}", _serverAddr, _port, _playerSessionId, _gameSessionId));


            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), port);

            bool pendiing = socket.ConnectAsync(args);
            if (pendiing == false)
            {
                OnConnectCompleted(socket, args);
            }
        }


        /// <summary>
        /// 서버와 연결 성공했다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                System.Net.Sockets.Socket socket = sender as System.Net.Sockets.Socket;

                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                receiveArgs.SetBuffer(new byte[_bufferSize], 0, _bufferSize);
                receiveArgs.UserToken = _clientSession;

                SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                sendArgs.SetBuffer(new byte[_bufferSize], 0, _bufferSize);
                sendArgs.UserToken = _clientSession;


                // 패킷 수신 시작
                BeginReceive(socket, receiveArgs, sendArgs);


                // 서버와의 연결이 성공하면 서버로 세션 상태를 요청한다.
                // 응답으로 신규연결/재연결 여부를 전달 받을 수 있다.
                SendInternalAuthSessionReq(_clientSession);

                _logger.Debug(string.Format("[NetClient] Complete to connect server. addr: {0}:{1}, playerSessionId: {2}, gameSessionId: {3}, socketHandle: {4}", _serverAddr, _port, _playerSessionId, _gameSessionId, socket.Handle));
            }
            else
            {
                _clientSession.DisconnectState = ESessionState.TimeOut;
                lock (_netEventQueue)
                {
                    _netEventQueue.Enqueue(_clientSession);
                }


                IPEndPoint remoteIpEndPoint = e.RemoteEndPoint as IPEndPoint;
                //Connect(remoteIpEndPoint.Address.ToString(), remoteIpEndPoint.Port);
                _logger.Error(string.Format("[NetClient] Failed to connect server(error: {0}), Retry to connect. address: {1}", e.SocketError, remoteIpEndPoint.Address + ":" + remoteIpEndPoint.Port));
            }
        }


        /// <summary>
        /// 서버와의 연결을 끊는다.
        /// </summary>
        public void Disconnect(ESessionState sessionState)
        {
            if (_clientSession.NetState == ENetState.Disconnected)
            {
                return;
            }

            _clientSession.DisconnectState = sessionState;
            _clientSession.Disconnect();

            _logger.Debug(string.Format("[NetClient] Disconnect server. sessionState: {0}", sessionState));
        }


        /// <summary>
        /// 연결 이벤트 처리
        /// </summary>
        protected void ProcessConnectionEvent()
        {
            // 서버 연결/해제 이벤트를 처리한다.
            ClientSession clientSession = null;
            lock (_netEventQueue)
            {
                if (_netEventQueue.Count == 0)
                {
                    return;
                }

                clientSession = _netEventQueue.Dequeue();
            }


            switch (clientSession.NetState)
            {
                case ENetState.Connected:
                    {
                        if (ClientConnectedCallback != null)
                        {
                            ClientConnectedCallback(clientSession, clientSession.DisconnectState);
                        }

                        _logger.Debug(string.Format("[NetClient] ProcessConnectionEvent - Connected. sessionState: {0}", clientSession.DisconnectState));
                    }
                    break;
                case ENetState.Reconnected:
                    {
                        clientSession.NetState = ENetState.Connected;

                        if (ClientReconnectedCallback != null)
                        {
                            ClientReconnectedCallback(clientSession, clientSession.DisconnectState);
                        }

                        _logger.Debug(string.Format("[NetClient] ProcessConnectionEvent - Reconnected. sessionState: {0}", clientSession.DisconnectState));
                    }
                    break;
                case ENetState.Disconnected:
                    {
                        if (clientSession.DisconnectState != ESessionState.None
                            && clientSession.DisconnectState != ESessionState.Wait)
                        {
                            if (_binarySerializePath.Length != 0)
                            {
                                File.Delete(_binarySerializePath);
                            }
                        }


                        if (ClientDisconnectedCallback != null)
                        {
                            ClientDisconnectedCallback(clientSession, clientSession.DisconnectState);
                        }

                        _logger.Debug(string.Format("[NetClient] ProcessConnectionEvent - Disconnected. sessionState: {0}", clientSession.DisconnectState));
                    }
                    break;
            }
        }


        protected override void OnCloseClientsocket(ClientSession clientSession, SocketError error)
        {
            clientSession.OnRemoved();
            clientSession.NetState = ENetState.Disconnected;


            lock (_netEventQueue)
            {
                _netEventQueue.Enqueue(clientSession);
            }

            _logger.Debug(string.Format("[NetClient] OnCloseClientsocket. sessionState: {0}", clientSession.DisconnectState));
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


            if (ProcessInternalPacket(clientSession, protocolId, msg, length) == false)
            {
                // 패킷처리 큐에 추가한다.
                _packetHandler.EnqueuePacket(clientSession.GetPeer(), protocolId, msg, length);
            }
        }


        protected override bool ProcessInternalPacket(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch ((EInternalProtocol)protocolId)
            {
                case EInternalProtocol.AUTH_SESSION_ACK:
                    {
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            clientSession.NetState = (ENetState)(byte)bf.Deserialize(ms);
                            clientSession.DisconnectState = (ESessionState)(short)bf.Deserialize(ms);
                        }


                        lock (_netEventQueue)
                        {
                            _netEventQueue.Enqueue(clientSession);
                        }
                    }
                    break;
                case EInternalProtocol.DISCONNECT_SESSION_NOTIFY:
                    {
                        ESessionState sessionState;
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            sessionState = (ESessionState)(short)bf.Deserialize(ms);
                        }

                        clientSession.DisconnectState = sessionState;
                    }
                    break;
                case EInternalProtocol.PAUSE_SESSION_ACK:
                    {
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(msg))
                        {
                            clientSession.PauseLimitTimeTick = (long)bf.Deserialize(ms);
                        }
                    }
                    break;
                case EInternalProtocol.RESUME_SESSION_ACK:
                    {
                    }
                    break;
                default:
                    {
                        return false;
                    }
            }

            return true;
        }


        public void PauseSession()
        {
            SendInternalPauseSessionReq(_clientSession);
        }


        public void ResumeSession()
        {
            if (_clientSession.PauseLimitTimeTick == 0)
            {
                return;
            }

            if (_clientSession.PauseLimitTimeTick < DateTime.UtcNow.Ticks)
            {
                _clientSession.GetPeer().Disconnect(ESessionState.Expired);
                return;
            }

            _clientSession.NetState = ENetState.Connected;
            SendInternalResumeSessionReq(_clientSession);
        }
    }
}
