using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Service.Net
{
    public delegate void CompletedMessageDelegate(ClientSession clientSession, int protocolId, byte[] msg, int length);

    public class ClientSession
    {
        public CompletedMessageDelegate CompletedMessageCallback;

        public Socket Socket { get; set; }

        public NetSocketAsyncEventArgs ReceiveEventArgs { get; set; }
        
        public NetSocketAsyncEventArgs SendEventArgs { get; set; }

        public GameSession GameSession { get; set; }

        public Peer Peer { get; set; }

        public string Id { get; set; }
    
        public long AliveTimeTick { get; set; }

        public long PauseLimitTimeTick { get; set; }
        public int PauseCount { get; set; }

        public ENetState NetState { get; set; }

        public ESessionState SessionState { get; set; }

        // 바이트를 패킷 형식으로 해석해주는 해석기.
        private MessageParser _messageParser;

        // 전송할 패킷을 보관해놓는 큐
        private Queue<byte[]> _sendingQueue;

        // _sendingQueue lock처리에 사용되는 객체
        private object _lockSendingQueue;



        public ClientSession(int bufferSize)
        {
            Peer = null;
            GameSession = null;

            _messageParser = new MessageParser(bufferSize);
            _sendingQueue = new Queue<byte[]>();
            _lockSendingQueue = new object();

            AliveTimeTick = 0;
            PauseLimitTimeTick = 0;
            PauseCount = 0;
        }


        public void SetEventArgs(NetSocketAsyncEventArgs receiveArgs, NetSocketAsyncEventArgs sendArgs)
        {
            ReceiveEventArgs = receiveArgs;
            SendEventArgs = sendArgs;

            lock (_lockSendingQueue)
            {
                _sendingQueue.Clear();
            }
        }


        public void Clear()
        {
            _sendingQueue.Clear();

            if (Peer != null)
            {
                Peer.Dispose();
            }
        }


        public void OnReceive(byte[] buffer, int offset, int transfered)
        {
            _messageParser.OnReceive(this, buffer, offset, transfered, CompletedMessageCallback);
        }


        public void Send(int protocolId, byte[] msg, int length)
        {
            if (Socket.Connected == false)
            {
                return;
            }


            byte[] buffer = _messageParser.WriteBuffer(protocolId, msg, length);
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


        public byte[] ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                return null;
            }

            lock (_lockSendingQueue)
            {
                if (_sendingQueue.Count == 0)
                {
                    return null;
                }

                // TODO : 재전송 로직 검토
                // 패킷 하나를 다 못보낸 경우는??
                byte[] buffer = _sendingQueue.Peek();
                if (e.BytesTransferred != buffer.Length)
                {
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


        public void Disconnect()
        {
            try
            {
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


        public bool ExpiredPauseTime(long nowTick)
        {
            return PauseLimitTimeTick != 0 
                && PauseLimitTimeTick < nowTick;
        }


        public bool OverPauseCount()
        {
            return ++PauseCount >= 30;
        }
    }
}