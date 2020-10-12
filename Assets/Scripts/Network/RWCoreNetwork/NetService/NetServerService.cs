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


        // 재연결 대상 세션
        protected List<ClientSession> _reconnectClientSessions;


        // 접속이 막힌 세션
        protected HashSet<string> _expiredClientSessions;


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
            _expiredClientSessions = new HashSet<string>();

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
        }


        public override void Update()
        {
            //if (_onMonitoring == true)
            //{
            //    _netMonitorHandler.Print();
            //}


            // 패킷을 처리한다.
            _packetHandler.Update();


            // 접속 해제 클라이언트 세션을 제거한다.
            long nowTick = DateTime.UtcNow.Ticks;
            if (_removeClientSessionTick < nowTick)
            {
                _removeClientSessionTick = DateTime.UtcNow.AddSeconds(10).Ticks;

                if (_reconnectClientSessions.Count > 0)
                {
                    List<ClientSession> tempRemoveSession = new List<ClientSession>();
                    lock (_lockClientSessionContainer)
                    {
                        ClientSession clientSession = _reconnectClientSessions.Last();
                        while (clientSession != null && clientSession.AliveTimeTick < nowTick)
                        {
                            _reconnectClientSessions.RemoveAt(_reconnectClientSessions.Count - 1);
                            tempRemoveSession.Add(clientSession);

                            if (_reconnectClientSessions.Count == 0)
                            {
                                break;
                            }
                            clientSession = _reconnectClientSessions.Last();
                        }
                    }


                    foreach (var clientSession in tempRemoveSession)
                    {
                        clientSession.NetState = ENetState.Disconnected;

                        if (_receiveEventAragePool != null)
                        {
                            _receiveEventAragePool.Push(clientSession.ReceiveEventArgs);
                        }

                        if (_sendEventAragePool != null)
                        {
                            _sendEventAragePool.Push(clientSession.SendEventArgs);
                        }

                        if (ClientDisconnectedCallback != null)
                        {
                            ClientDisconnectedCallback(clientSession, clientSession.SessionState);
                        }

                        _logger.Info(string.Format("Remove client session. sessionId: {0}", clientSession.SessionId));
                    }
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


            int protocolId = BitConverter.ToInt32(msg, 0);
            int length = BitConverter.ToInt32(msg, Defines.PROTOCOL_ID);
            byte[] buffer = new byte[_bufferSize];
            Array.Copy(msg, Defines.HEADER_SIZE, buffer, 0, length);


            if (_onMonitoring == true)
            {
                _netMonitorHandler.SetReceivePacket(clientSession.SessionId, msg);

                _logger.Info(string.Format("OnMessageCompleted. protocolId: {0}, length: {1}", protocolId, length));
            }


            if (protocolId == (int)EInternalProtocol.AUTH_CLIENT_SESSION_REQ)
            {
                bool isOnline = false;
                ClientSession oldClientSession = null;

                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(buffer))
                {
                    string clientSessionId = (string)bf.Deserialize(ms);
                    if (clientSessionId == string.Empty)
                    {
                        _logger.Fatal("OnMessageCompleted. clientSessionId is empty!!!");
                        return; 
                    }


                    // 만료된 세션은 인증에서 제외시킨다.
                    if (_expiredClientSessions.Contains(clientSessionId) == false)
                    {
                        clientSession.SessionId = clientSessionId;
                        clientSession.NetState = ENetState.Connected;
                        clientSession.SessionState = ESessionState.None;


                        lock (_lockClientSessionContainer)
                        {
                            foreach (var elem in _reconnectClientSessions)
                            {
                                if (elem.SessionId == clientSessionId)
                                {
                                    oldClientSession = elem;
                                    isOnline = true;
                                    break;
                                }
                            }


                            if (isOnline == true)
                            {
                                _reconnectClientSessions.Remove(oldClientSession);
                            }


                            _authedClientSessions.Add(clientSessionId, clientSession);
                        }


                        if (isOnline == true)
                        {
                            if (ClientOnlineCallback != null)
                            {
                                ClientOnlineCallback(clientSession, oldClientSession.GetPeer());
                            }

                            if (_receiveEventAragePool != null)
                            {
                                _receiveEventAragePool.Push(oldClientSession.ReceiveEventArgs);
                            }

                            if (_sendEventAragePool != null)
                            {
                                _sendEventAragePool.Push(oldClientSession.SendEventArgs);
                            }
                        }
                        else
                        {
                            if (ClientConnectedCallback != null)
                            {
                                ClientConnectedCallback(clientSession, clientSession.SessionState);
                            }
                        }
                    }
                    else
                    {
                        clientSession.SessionState = ESessionState.Expired;
                    }
                }

                {
                    var bfs = new BinaryFormatter();
                    using (var ms = new MemoryStream())
                    {
                        bfs.Serialize(ms, isOnline);
                        bfs.Serialize(ms, (short)clientSession.SessionState);
                        clientSession.Send((int)EInternalProtocol.AUTH_CLIENT_SESSION_ACK, 
                            ms.ToArray(),
                            ms.ToArray().Length);
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
            clientSession.NetState = ENetState.Offline;


            List<ClientSession> tempRemoveSession = new List<ClientSession>();
            lock (_lockClientSessionContainer)
            {
                _authedClientSessions.Remove(clientSession.SessionId);

                if (_authedClientSessions.Count == 0)
                {
                    foreach(var elem in _reconnectClientSessions)
                    {
                        tempRemoveSession.Add(elem);
                    }
                    _reconnectClientSessions.Clear();
                }
                else
                {
                    if (clientSession.SessionState != ESessionState.None)
                    {
                        _expiredClientSessions.Add(clientSession.SessionId);
                    }
                    else
                    {
                        clientSession.AliveTimeTick = DateTime.UtcNow.AddSeconds(60).Ticks;
                        _reconnectClientSessions.Insert(0, clientSession);
                    }
                }
            }


            foreach (var elem in tempRemoveSession)
            {
                if (_receiveEventAragePool != null)
                {
                    _receiveEventAragePool.Push(elem.ReceiveEventArgs);
                }

                if (_sendEventAragePool != null)
                {
                    _sendEventAragePool.Push(elem.SendEventArgs);
                }
            }


            if (ClientOfflineCallback != null)
            {
                ClientOfflineCallback(clientSession, clientSession.SessionState);
            }
        }
    }
}
