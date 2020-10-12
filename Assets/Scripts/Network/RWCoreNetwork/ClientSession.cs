using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Sockets;

using RWCoreLib.Log;
using RWCoreNetwork.NetPacket;
using RWCoreNetwork.NetService;


namespace RWCoreNetwork
{
    public enum ESessionState : short
    {
        None = 0,
        // 서버에 의한 블럭
        Blocked,
        // 만료된 세션
        Expired
    }

    public delegate void CompletedMessageDelegate(ClientSession userToken, byte[] msg);


    public class ClientSession
    {
        public CompletedMessageDelegate CompletedMessageCallback;


        public string SessionId { get; set; }

        public ENetState NetState { get; set; }

        public ESessionState SessionState { get; set; }

        public Socket Socket { get; set; }
        
        public SocketAsyncEventArgs ReceiveEventArgs { get; set; }
        
        public SocketAsyncEventArgs SendEventArgs { get; set; }


        public long AliveTimeTick { get; set; }


        // session객체. 어플리케이션 딴에서 구현하여 사용.
        Peer _peer;

        // 바이트를 패킷 형식으로 해석해주는 해석기.
        MessageHandler _messageHandler;

        // 전송할 패킷을 보관해놓는 큐
        Queue<byte[]> _sendingQueue;

        // _sendingQueue lock처리에 사용되는 객체
        object _lockSendingQueue;

        ILog _logger;



        public ClientSession(ILog logger, int bufferSize)
        {
            _logger = logger;
            _peer = null;
            _messageHandler = new MessageHandler(bufferSize);
            _sendingQueue = new Queue<byte[]>();
            _lockSendingQueue = new object();
            SessionId = string.Empty;
            SessionState = ESessionState.None;
        }


        public void SetPeer(Peer peer)
        {
            _peer = peer;
        }


        public Peer GetPeer()
        {
            return _peer;
        }


        public void SetEventArgs(SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            ReceiveEventArgs = receiveArgs;
            SendEventArgs = sendArgs;


            lock (_lockSendingQueue)
            {
                _sendingQueue.Clear();
            }
        }


        /// <summary>
		///	이 매소드에서 직접 바이트 데이터를 해석해도 되지만 Message resolver클래스를 따로 둔 이유는
		///	추후에 확장성을 고려하여 다른 resolver를 구현할 때 CUserToken클래스의 코드 수정을 최소화 하기 위함이다.
        /// </summary>
        /// <param name="buffer">소켓으로부터  수신된 데이터가 들어 있는 바이트 배열</param>
        /// <param name="offset">데이터의 시작 위치를 나타내는 정수 값</param>
        /// <param name="transfered">수신된 데이터의 바이트 수</param>
        public void OnReceive(byte[] buffer, int offset, int transfered)
        {
            _messageHandler.OnReceive(this, buffer, offset, transfered, CompletedMessageCallback);
        }


        /// <summary>
        /// 패킷을 전송한다.
        /// 큐가 비어 있을 경우에는 큐에 추가한 뒤 바로 SendAsync매소드를 호출하고,
        /// 데이터가 들어있을 경우에는 새로 추가만 한다.
        /// 
        /// 큐잉된 패킷의 전송 시점 :
        ///		현재 진행중인 SendAsync가 완료되었을 때 큐를 검사하여 나머지 패킷을 전송한다.
        /// </summary>
        /// <param name="protocolId"></param>
        /// <param name="msg"></param>
        public void Send(int protocolId, byte[] msg, int length)
        {
            byte[] buffer = _messageHandler.WriteBuffer(protocolId, msg, length);
            lock (_lockSendingQueue)
            {
                // 큐가 비어 있다면 큐에 추가하고 바로 비동기 전송 매소드를 호출한다.
                if (_sendingQueue.Count == 0)
                {
                    _sendingQueue.Enqueue(buffer);
                    StartSend();
                    return;
                }

                // 큐에 무언가가 들어 있다면 아직 이전 전송이 완료되지 않은 상태이므로 큐에 추가만 하고 리턴한다.
                // 현재 수행중인 SendAsync가 완료된 이후에 큐를 검사하여 데이터가 있으면 SendAsync를 호출하여 전송해줄 것이다.
                _sendingQueue.Enqueue(buffer);
            }
        }

		/// <summary>
		/// 비동기 전송을 시작한다.
		/// </summary>
        private void StartSend()
        {
            byte[] buffer;
            lock(_lockSendingQueue)
            {
                // 전송이 아직 완료된 상태가 아니므로 데이터만 가져오고 큐에서 제거하진 않는다.
                buffer = _sendingQueue.Peek();
            }

            // 이번에 보낼 패킷 사이즈 만큼 버퍼 크기를 설정하고
            // 패킷 내용을 SocketAsyncEventArgs버퍼에 복사한다.
            SendEventArgs.SetBuffer(SendEventArgs.Offset, buffer.Length);
            Array.Copy(buffer, 0, SendEventArgs.Buffer, SendEventArgs.Offset, buffer.Length);


            // 비동기 전송 시작.
            bool pending = Socket.SendAsync(SendEventArgs);
            if (pending == false)
            {
                ProcessSend(SendEventArgs);
            }
        }


		/// <summary>
		/// 비동기 전송 완료시 호출되는 콜백 매소드.
		/// </summary>
		/// <param name="e"></param>
        public byte[] ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                Console.WriteLine("[ERROR] thread: " + Thread.CurrentThread.ManagedThreadId + ", err: " + e.SocketError);
                return null;
            }

            lock (_lockSendingQueue)
            {
                if (_sendingQueue.Count == 0)
                {
                    Console.WriteLine("[ERROR] thread: " + Thread.CurrentThread.ManagedThreadId + ", Sending queue count is less than zero");
                    return null;
                }

                // TODO : 재전송 로직 검토
                // 패킷 하나를 다 못보낸 경우는??
                byte[] buffer = _sendingQueue.Peek();
                if (e.BytesTransferred != buffer.Length)
                {
                    Console.WriteLine(string.Format("Need to send more! transferred {0},  packet size {1}", e.BytesTransferred, buffer.Length));
                    return null;
                }

                // 전송 완료된 패킷을 큐에서 제거한다.
                _sendingQueue.Dequeue();


                // 아직 전송하지 않은 대기중인 패킷이 있다면 다시한번 전송을 요청한다.
                if (_sendingQueue.Count > 0)
                {
                    StartSend();
                }

                return buffer;
            }
        }


        public void OnRemoved()
        {
            _sendingQueue.Clear();

            if (_peer != null)
            {
                _peer.OnRemoved();
            }
        }


        public void Disconnect()
        {
            try
            {
                //Console.WriteLine( string.Format("Disconnected. Handle {0}", Socket.Handle));
                Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
            Socket.Close();
        }

        public void SetKeepAlive(int keepaliveTime, int keepaliveInterval)
        {
            byte[] inOptionValues = new byte[12];
            int enable = 0 != keepaliveTime
                                 ? 1
                                 : 0;
            BitConverter.GetBytes(enable).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes(keepaliveTime).CopyTo(inOptionValues, 4);
            BitConverter.GetBytes(keepaliveInterval).CopyTo(inOptionValues, 8);

            try
            {
                Socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
                return;
            }
            catch (NotSupportedException)
            {
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValues);
                return;
            }
            catch (NotImplementedException)
            {
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inOptionValues);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}