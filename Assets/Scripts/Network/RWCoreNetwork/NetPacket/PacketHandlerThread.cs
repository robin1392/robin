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


        public PacketHandlerThread()
        {
            _loopEvent = new AutoResetEvent(false);
            _thread = new Thread(new ThreadStart(DoWork));
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


        public override void EnqueuePacket(Peer peer, short protocolId, byte[] msg)
        {
            base.EnqueuePacket(peer, protocolId, msg);
            _loopEvent.Set();
        }


        public override void Update()
        {
        }


        public void DoWork()
        {
            Packet packet = null; 
            while(true)
            {
                packet = DequeuePacket();
                if (packet == null)
                {
                    // 더이상 처리할 패킷이 없으면 스레드 대기.
                    _loopEvent.WaitOne();
                    continue;
                }

                PacketProcessor.Run(packet.Peer, packet.ProtocolId, packet.Data);
            }
        }
    }
}