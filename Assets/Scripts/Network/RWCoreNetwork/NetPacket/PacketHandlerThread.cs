using System;
using System.Collections.Generic;
using System.Threading;

namespace RWCoreNetwork.NetPacket
{
    /// <summary>
	/// 스레드 기반 패킷 핸들러
    /// TODO : [개선] 복수의 스레드 기반으로 패킷을 처리하도록 개선 필요.
	/// </summary>
    public class PacketHandlerThread : PacketHandler
    {
        Thread _thread;
		AutoResetEvent _loopEvent;


        public PacketHandlerThread(IPacketReceiver packetReceiver, int packetProcessCount)
            : base(packetReceiver, packetProcessCount)
        {
            _loopEvent = new AutoResetEvent(false);
            _thread = new Thread(new ThreadStart(ProcessReceive));
            _thread.Start();
        }


        public override void SetActive(bool flag)
        {
            _isActivated = flag;

            if (_isActivated == true)
            {
                _loopEvent.Set();
            }
        }


        public override void EnqueueReceivePacket(Peer peer, byte[] msg)
        {
            base.EnqueueReceivePacket(peer, msg);
            _loopEvent.Set();
        }


        public override void Update()
        {
        }


        public void ProcessReceive()
        {
            Packet packet = null; 
            while(true)
            {
                packet = DequeueReceivePacket();
                if (packet == null)
                {
                    // 더이상 처리할 패킷이 없으면 스레드 대기.
                    _loopEvent.WaitOne();
                    continue;
                }


                if (InterceptProtocol != null)
                {
                    if (InterceptProtocol(packet.Peer, packet.ProtocolId, packet.Msg) == true)
                    {
                        continue;
                    }
                }


                PacketReceiver.Process(packet.Peer, packet.ProtocolId, packet.Msg);
            }
        }
    }
}