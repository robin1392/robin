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
        protected ILogger _logger;

        protected NetServiceConfig _config;
        
        // 비동기 수신 소켓 작업 풀
        // protected ObjectPool<NetSocketAsyncEventArgs> _receiveEventAragePool;

        // // 비동기 송신 소켓 작업 풀
        // protected ObjectPool<NetSocketAsyncEventArgs> _sendEventAragePool;

        // 게임 세션 목록
        protected GameSession _gameSession;



        public NetServiceBase()
        {
            // _receiveEventAragePool = new ObjectPool<NetSocketAsyncEventArgs>();
            // _sendEventAragePool = new ObjectPool<NetSocketAsyncEventArgs>();
        }


        public virtual void Init(NetServiceConfig config)
        {
            _logger = config.Logger;
            _config = config;

            // for (int i = 0; i < config.MaxConnectionNum; i++)
            // {
            //     ClientSession clientSession = new ClientSession(config.BufferSize);
            //     clientSession.CompletedMessageCallback += OnCompletedMessageCallback;


            //     NetSocketAsyncEventArgs receiveArgs = new NetSocketAsyncEventArgs();
            //     receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnCompletedReceiveCallback);
            //     receiveArgs.UserToken = clientSession;
            //     receiveArgs.SetBuffer(new byte[config.BufferSize], 0, config.BufferSize);
            //     _receiveEventAragePool.Push(receiveArgs);

            //     NetSocketAsyncEventArgs sendArgs = new NetSocketAsyncEventArgs();
            //     sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnCompletedSendCallback);
            //     sendArgs.UserToken = clientSession;
            //     sendArgs.SetBuffer(new byte[config.BufferSize], 0, config.BufferSize);
            //     _sendEventAragePool.Push(sendArgs);
            // }   
        }


        public virtual void Clear()
        {
        }


        public virtual void Update() 
        {
            // if (_gameSession != null)
            // {
            //     _gameSession.Update();
            // }
        }


        public void SetGameSession(GameSession gameSession)
        {
            _gameSession = gameSession;
        }


        protected void BeginReceive(Socket clientSocket, NetSocketAsyncEventArgs receiveArgs, NetSocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 됨. 둘 다 동일한 clientSession 물고 있음
            ClientSession clientSession = receiveArgs.UserToken as ClientSession;
            clientSession.SetEventArgs(receiveArgs, sendArgs);


            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용함
            clientSession.Socket = clientSocket;
            clientSession.SetKeepAlive(_config.KeepAliveTime, _config.KeepAliveInterval);


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
                ClientSession clientSession = e.UserToken as ClientSession;
                clientSession.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);
    

                // 다음 메시지 수신을 위해서 다시 ReceiveAsync 메소드를 호출한다.
                bool pending = clientSession.Socket.ReceiveAsync(e);
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
            ClientSession clientSession = e.UserToken as ClientSession;
            byte[] msg = clientSession.ProcessSend(e);
        }


        protected virtual void OnCompletedMessageCallback(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
        }


        protected virtual bool ProcessNetServiceMessage(ClientSession clientSession, int protocolId, byte[] msg, int length)
        {
            return false;
        }


        public void SendNetAuthSessionReq(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, clientSession.SessionId);
                bf.Serialize(ms, (byte)clientSession.NetState);
                clientSession.Send((int)ENetProtocol.AUTH_SESSION_REQ,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }

        public void SendNetAuthSessionAck(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, clientSession.SessionId);
                bf.Serialize(ms, (byte)clientSession.NetState);
                bf.Serialize(ms, (short)clientSession.SessionState);
                clientSession.Send((int)ENetProtocol.AUTH_SESSION_ACK,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetDisconnectNotify(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, (short)clientSession.SessionState);
                clientSession.Send((int)ENetProtocol.DISCONNECT_SESSION_NOTIFY,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetPauseSessionReq(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                clientSession.Send((int)ENetProtocol.PAUSE_SESSION_REQ,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        protected void SendNetPauseSessionAck(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                clientSession.Send((int)ENetProtocol.PAUSE_SESSION_ACK,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetResumeSessionReq(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                clientSession.Send((int)ENetProtocol.RESUME_SESSION_REQ,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }


        public void SendNetResumeSessionAck(ClientSession clientSession)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                clientSession.Send((int)ENetProtocol.RESUME_SESSION_ACK,
                    ms.ToArray(),
                    ms.ToArray().Length);
            }
        }        
    }
}