using System;
using System.Collections.Generic;
using RWCoreLib.Log;

namespace RWCoreNetwork.NetPacket
{

    /// <summary>
	/// 모든 세션의 수신 패킷을 만들고 큐에 추가한다.
	/// </summary>
    public class PacketHandler
    {
        // 프로토콜 정의 델리게이트
        public delegate bool InterceptProtocolDelegate(Peer peer, int protocolId, byte[] msg, int msgLength);
        public InterceptProtocolDelegate InterceptProtocol;


        protected IPacketReceiver PacketReceiver { get; private set; }

        protected ILog _logger { get; private set; }


        protected bool _isActivated { get; set; }

        // 수신 큐
        protected Queue<Packet> _receiveQueue;
        protected object _lockReceiveQueue;

        private readonly int _packetProcessCount;
        private readonly int _bufferSize;



        public PacketHandler(IPacketReceiver packetReceiver, ILog logger, int packetProcessCount, int bufferSize)
        {
            PacketReceiver = packetReceiver;
            _logger = logger;

            _packetProcessCount = packetProcessCount;
            _bufferSize = bufferSize;

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


        public virtual void EnqueuePacket(Peer peer, byte[] msg)
        {
            Packet packet = new Packet(peer, msg, _bufferSize);
            lock (_lockReceiveQueue)
            {
                _receiveQueue.Enqueue(packet);
            }
        }


        public virtual void EnqueuePacket(Peer peer, int protocolId, byte[] msg, int length)
        {
            Packet packet = new Packet(peer, protocolId, msg, length);
            lock (_lockReceiveQueue)
            {
                _receiveQueue.Enqueue(packet);
            }
        }



        public virtual Packet DequeuePacket()
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
                Packet packet = DequeuePacket();
                if (packet == null)
                {
                    return;
                }


                if (InterceptProtocol != null)
                {
                    if (InterceptProtocol(packet.Peer, packet.ProtocolId, packet.Msg, packet.Length) == true)
                    {
                        continue;
                    }
                }


               if (PacketReceiver.Process(packet.Peer, packet.ProtocolId, packet.Msg) == false)
                {
                    throw new Exception(string.Format("failed to process receive packet. protocol {0}", packet.ProtocolId));
                }
            }
        }
    }
}