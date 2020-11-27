using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;

namespace Service.Net
{
    public class NetClient : NetBase, IDisposable
    {

        public override void Init(NetServiceConfig config)
        {
            base.Init(config);
        }


        public void Dispose()
        {
            
        }   


        public void Connect(string serverAddr, int port, string gameSessionId, string clientSessionId)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.LingerState = new LingerOption(true, 10);
            socket.NoDelay = true;


            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnCompletedMessageCallback;
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), port);
            args.UserToken = new NetClientToken
            {
                ClientSessionId = clientSessionId,
                GameSessionId = gameSessionId
            };


            bool pendiing = socket.ConnectAsync(args);
            if (pendiing == false)
            {
                OnCompletedMessageCallback(socket, args);
            }
        }


        void OnCompletedMessageCallback(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                // 연결 에러 처리
                IPEndPoint remoteIpEndPoint = e.RemoteEndPoint as IPEndPoint;
                return;
            }


            Socket socket = sender as Socket;
            NetSocketAsyncEventArgs receiveArgs = _receiveEventAragePool.Pop();
            NetSocketAsyncEventArgs sendArgs = _sendEventAragePool.Pop();

            ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            NetClientToken connectionToken = e.UserToken as NetClientToken;
            if (clientSession == null || connectionToken == null)
            {
                return;
            }


            // 패킷 수신 시작
            BeginReceive(socket, receiveArgs, sendArgs);


            // 서버와의 연결이 성공하면 서버로 세션 상태를 요청한다.
            clientSession.Id = connectionToken.ClientSessionId;
            clientSession.NetState = ENetState.Connecting;
            clientSession.SendNetAuthSessionReq(connectionToken.GameSessionId);
        }


        protected override void OnCloseClientSocket(ClientSession clientSession, SocketError error)
        {
            clientSession.NetState = ENetState.Disconnected;
        }


        protected override void OnCompletedMessageCallback(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            if (clientSession == null)
            {
                return;
            }


            // 서비스 내부 메세지을 처리한다.
            bool f = ProcessNetServiceMessage(clientSession, protocolId, msg, length);
            if (f == false)
            {
                // 릴레이 메세지를 처리한다.
                if (clientSession.GameSession.PushRelayMessage(clientSession, protocolId, msg, length) == false)
                {
                    // 외부 메세지를 처리한다.
                    clientSession.GameSession.PushExternalMessage(clientSession, protocolId, msg, length);
                }
            }
        }


        protected override bool ProcessNetServiceMessage(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            switch((ENetProtocol)protocolId)
            {
                case ENetProtocol.AUTH_SESSION_ACK:
                {
                    string gameSessionId;
                    string clientSessionId;
                    ENetState netState;
                    ESessionState sessionState;
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream(msg))
                    {
                        gameSessionId = (string)bf.Deserialize(ms);
                        clientSessionId = (string)bf.Deserialize(ms);
                        netState = (ENetState)(byte)bf.Deserialize(ms);
                        sessionState = (ESessionState)(short)bf.Deserialize(ms);
                    }


                    // 게임 세션 
                    GameSession gameSession = GetGameSession(gameSessionId);
                    if (gameSession == null)
                    {
                        break;
                    }

                    clientSession.GameSession = gameSession;
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


    }
}