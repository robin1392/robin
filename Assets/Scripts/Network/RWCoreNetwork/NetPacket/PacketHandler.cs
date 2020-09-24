using System;
using System.Collections.Generic;

namespace RWCoreNetwork.NetPacket
{

    /// <summary>
	/// 모든 세션의 수신 패킷을 만들고 큐에 추가한다.
	/// </summary>
    public class PacketHandler
    {
        // 프로토콜 정의 델리게이트
        public delegate bool InterceptProtocolDelegate(Peer peer, short protocolId, byte[] msg);
        public InterceptProtocolDelegate InterceptProtocol;


        protected IPacketReceiver PacketReceiver { get; private set; }

        protected bool _isActivated { get; set; }

        // 수신 큐
        protected Queue<Packet> _receiveQueue;
        protected object _lockReceiveQueue;

        protected Queue<byte[]> _sendQueue;
        protected object _lockSendQueue;

        private readonly int _packetProcessCount;



        public PacketHandler(IPacketReceiver packetReceiver, int packetProcessCount)
        {
            PacketReceiver = packetReceiver;
            _packetProcessCount = packetProcessCount;

            _isActivated = true;
            _receiveQueue = new Queue<Packet>();
            _lockReceiveQueue = new object();
        }


        public int Count()
        {
            return _receiveQueue.Count;
        }


        public virtual void SetActive(bool flag)
        {
            _isActivated = flag;
        }


        public virtual void EnqueueReceivePacket(Peer peer, byte[] msg)
        {
            Packet packet = new Packet(peer, msg);
            lock (_lockReceiveQueue)
            {
                _receiveQueue.Enqueue(packet);
            }
        }


        public virtual Packet DequeueReceivePacket()
        {
            lock (_lockReceiveQueue)
            {
                if (_receiveQueue.Count == 0)
                {
                    return null;
                }

                return _receiveQueue.Dequeue();
            }
        }


        public virtual void EnqueueSendPacket(Peer peer, short protocolId, byte[] msg)
        {
            //byte[] buffer = new byte[1024];

            //// protocol id
            //int offset = 0;
            //byte[] tmpBuffer = BitConverter.GetBytes(protocolId);
            //Array.Copy(tmpBuffer, 0, buffer, offset, tmpBuffer.Length);

            //// body length
            //offset = tmpBuffer.Length;
            //tmpBuffer = BitConverter.GetBytes((short)(buffer.Length - 4));
            //Array.Copy(tmpBuffer, 0, buffer, offset, tmpBuffer.Length);

            //// msg
            //offset += tmpBuffer.Length;
            //Array.Copy(msg, 0, buffer, offset, msg.Length);


            //lock (_lockSendQueue)
            //{
            //    _sendQueue.Enqueue(buffer);
            //}
        }


        public virtual byte[] DequeueSendPacket()
        {
            lock (_lockSendQueue)
            {
                if (_sendQueue.Count == 0)
                {
                    return null;
                }

                return _sendQueue.Dequeue();
            }
        }


        /// <summary>
        /// 큐에서 패킷을 하나 꺼내서 처리한다.
        /// 외부 스레드의 루프문 내에서 호출되어야 한다.
        /// (유니티 클라이언트에서 사용할 목적으로 구현됨)
        /// </summary>
        public virtual void Update()
        {
            if (_isActivated == false)
            {
                return;
            }

            if (PacketReceiver == null)
            {
                return;
            }

            for (int i = 0; i < _packetProcessCount; i++)
            {
                Packet packet = DequeueReceivePacket();
                if (packet == null)
                {
                    return;
                }


                if (InterceptProtocol != null)
                {
                    if (InterceptProtocol(packet.Peer, packet.ProtocolId, packet.Data) == true)
                    {
                        continue;
                    }
                }


                PacketReceiver.Process(packet.Peer, packet.ProtocolId, packet.Data);
            }
        }
    }
}