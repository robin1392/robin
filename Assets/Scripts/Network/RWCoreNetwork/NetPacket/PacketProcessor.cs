using System;
using System.Collections.Generic;

namespace RWCoreNetwork.NetPacket
{
    public interface IPacketProcessor
    {
        bool DoWork(IPeer peer, short protocolId, byte[] data);
    }
}