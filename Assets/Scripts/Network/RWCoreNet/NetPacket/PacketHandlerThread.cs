using System;
using System.Threading;

namespace RWCoreNet.NetPacket
{
    public class PacketHandlerThread : PacketHandler
    {
        Thread m_thread;
		AutoResetEvent m_loopEvent;

        public PacketHandlerThread()
        {
            m_loopEvent = new AutoResetEvent(false);
            m_thread = new Thread(new ThreadStart(ProcessPacket));
            m_thread.Start();
        }

        public override void Enqueue(Packet packet)
        {
            base.Enqueue(packet);
            m_loopEvent.Set();
        }

        public override void ProcessPacket()
        {
            Packet packet = null; 
            while(true)
            {
                packet = Dequeue();
                if (packet == null)
                {
                    // 더이상 처리할 패킷이 없으면 스레드 대기.
                    m_loopEvent.WaitOne();
                    continue;
                }
                
                PacketProcessor.DoWork(packet.Session, packet.ProtocolId, packet.Data);
            }
        }
    }
}