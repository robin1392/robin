using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Service.Core;

namespace Service.Net 
{
    public class NetServiceBase
    {
        protected NetServiceConfig _config;
        
        // 게임 세션 목록
        protected GameSession _gameSession;



        public NetServiceBase()
        {
        }


        public virtual void Init(NetServiceConfig config)
        {
            _config = config;
        }


        public virtual void Clear()
        {
        }


        public virtual void Update() 
        {
        }


        public void SetGameSession(GameSession gameSession)
        {
            _gameSession = gameSession;
        }


        protected void BeginReceive(Socket clientSocket, NetSocketAsyncEventArgs receiveArgs, NetSocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 됨. 둘 다 동일한 userToken 물고 있음
            UserToken userToken = receiveArgs.UserToken as UserToken;
            userToken.SetEventArgs(receiveArgs, sendArgs);


            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용함
            userToken.Socket = clientSocket;
            userToken.SetKeepAlive(_config.KeepAliveTime, _config.KeepAliveInterval);


            // 데이터를 받을 수 있도록 소켓 메소드를 호출해준다.
            // 비동기로 수신할 경우 워커 스레드에서 대기중으로 있다가 Completed에 설정해놓은 메소드가 호출된다.
            // 동기로 완료될 경우에는 직접 완료 메소드를 호출해줘야 한다.
            bool pending = clientSocket.ReceiveAsync(receiveArgs);
            if (pending == false)
            {
                ProcessReceive(receiveArgs);
            }
        }


        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // 원격 호스트가 연결을 종료했는지 확인
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                UserToken userToken = e.UserToken as UserToken;
                userToken.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);
    

                // 다음 메시지 수신을 위해서 다시 ReceiveAsync 메소드를 호출한다.
                bool pending = userToken.Socket.ReceiveAsync(e);
                if (pending == false)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                OnCloseClientSocket(e);
            }
        }


        protected virtual void OnCloseClientSocket(SocketAsyncEventArgs e)
        {
        }


        protected void OnCompletedReceiveCallback(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation != SocketAsyncOperation.Receive)
            {
                throw new ArgumentException("The last operation completed on the socket was not a receive.");
            }

            ProcessReceive(e);
        }


        protected virtual void OnCompletedSendCallback(object sender, SocketAsyncEventArgs e)
        {
            UserToken userToken = e.UserToken as UserToken;
            byte[] msg = userToken.ProcessSend(e);
        }


        protected virtual void OnCompletedMessageCallback(UserToken userToken, int protocolId, byte[] msg, int length)
        {
        }


        protected virtual bool ProcessNetServiceMessage(UserToken userToken, int protocolId, byte[] msg, int length)
        {
            return false;
        }


        public void SendNetAuthSessionReq(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, userToken.SessionId);
                bf.Serialize(ms, (byte)userToken.NetState);
                userToken.Send((int)ENetProtocol.AUTH_SESSION_REQ,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }

        public void SendNetAuthSessionAck(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, userToken.SessionId);
                bf.Serialize(ms, (byte)userToken.NetState);
                bf.Serialize(ms, (short)userToken.SessionState);
                userToken.Send((int)ENetProtocol.AUTH_SESSION_ACK,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetDisconnectNotify(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, (short)userToken.SessionState);
                userToken.Send((int)ENetProtocol.DISCONNECT_SESSION_NOTIFY,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetPauseSessionReq(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                userToken.Send((int)ENetProtocol.PAUSE_SESSION_REQ,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        protected void SendNetPauseSessionAck(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                userToken.Send((int)ENetProtocol.PAUSE_SESSION_ACK,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetResumeSessionReq(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                userToken.Send((int)ENetProtocol.RESUME_SESSION_REQ,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetResumeSessionAck(UserToken userToken)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                userToken.Send((int)ENetProtocol.RESUME_SESSION_ACK,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }        
    }
}