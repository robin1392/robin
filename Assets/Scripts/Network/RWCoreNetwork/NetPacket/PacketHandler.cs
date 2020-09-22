using System;
using System.Collections.Generic;

namespace RWCoreNetwork.NetPacket
{

    /// <summary>
	/// 모든 세션의 수신 패킷을 만들고 큐에 추가한다.
	/// </summary>
    public class PacketHandler
    {
        protected bool _isActivated { get; set; }

        // 패킷 보관 큐
        protected Queue<Packet> _packetQueue;

        // 큐 동기화 객체
        protected object _lockQueue;

        protected int _limitCount;

        protected IPacketProcessor PacketProcessor { get; private set; }


        public PacketHandler()
        {
            _isActivated = false;
            _packetQueue = new Queue<Packet>();
            _lockQueue = new object();           
            _limitCount = 0;
        }

        public void Init(IPacketProcessor packetProcessor, int count)
        {
            _limitCount = count;
            PacketProcessor = packetProcessor;
        }


        public virtual void SetActive(bool flag)
        {
            _isActivated = flag;
        }


        public virtual void EnqueuePacket(Packet packet)
        {
            lock (_lockQueue)
            {
                if (_packetQueue.Count >= _limitCount)
                {
                    throw new Exception("overflow receive packet queue.");
                }

                _packetQueue.Enqueue(packet);
            }
        }

        public virtual Packet DequeuePacket()
        {
            lock (_lockQueue)
            {
                if (_packetQueue.Count == 0)
                {
                    return null;
                }

                return _packetQueue.Dequeue();
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

            if (PacketProcessor == null)
            {
                return;
            }

            for (int i = 0; i < 60; i++)
            {
                Packet packet = DequeuePacket();
                if (packet == null)
                {
                    return;
                }

                PacketProcessor.Run(packet.Peer, packet.ProtocolId, packet.Data);
            }
        }
    }
}