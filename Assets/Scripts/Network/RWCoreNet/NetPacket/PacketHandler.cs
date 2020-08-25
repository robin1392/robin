using System;
using System.Collections.Generic;

namespace RWCoreNet.NetPacket
{
    /// <summary>
	/// 모든 세션의 수신 패킷을 만들고 큐에 추가한다.
    /// 
	/// </summary>
    public class PacketHandler
    {
        Queue<Packet> m_packetQueue;
        object m_lockQueue;
        int m_limitCount;

        protected IPacketProcessor PacketProcessor { get; private set; }

        public PacketHandler()
        {
            m_packetQueue = new Queue<Packet>();
            m_lockQueue = new object();           
            m_limitCount = 0;
        }

        public void Init(IPacketProcessor packetProcessor, int count)
        {
            m_limitCount = count;
            PacketProcessor = packetProcessor;
        }

        public virtual void Enqueue(Packet packet)
        {
            lock (m_lockQueue)
            {
                if (m_packetQueue.Count >= m_limitCount)
                {
                    // 경고 메세지만 출력하고 정상적으로 큐에 추가시킨다.
                    Console.WriteLine("overflow receive packet queue.");
                }

                m_packetQueue.Enqueue(packet);
            }
        }

        public virtual Packet Dequeue()
        {
            lock (m_lockQueue)
            {
                if (m_packetQueue.Count == 0)
                {
                    return null;
                }

                return m_packetQueue.Dequeue();
            }
        }
        
		/// <summary>
		/// 큐에서 패킷을 하나 꺼내서 처리한다.
        /// 외부 스레드의 루프문 내에서 호출되어야 한다.
        /// (유니티 클라이언트에서 사용할 목적으로 구현됨)
		/// </summary>
        public virtual void ProcessPacket()
        {
            try
            {
                Packet packet = Dequeue();
                if (packet == null)
                {
                    return;
                }

                if (PacketProcessor == null)
                {
                    throw new Exception("not found packet processor.");
                }

                PacketProcessor.DoWork(packet.Session, packet.ProtocolId, packet.Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}