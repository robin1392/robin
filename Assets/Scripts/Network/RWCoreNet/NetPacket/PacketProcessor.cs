using System;
using System.Collections.Generic;
using RWCoreNet;

namespace RWCoreNet.NetPacket
{
    public interface IPacketProcessor
    {
        bool DoWork(IPeer session, short protocolId, byte[] data);
    }
}