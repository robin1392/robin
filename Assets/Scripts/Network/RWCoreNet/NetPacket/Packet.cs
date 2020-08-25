using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RWCoreNet.NetPacket
{
    public class Packet
    {
        public short ProtocolId { get; private set; }
        public byte[] Data { get; private set; }
        public IPeer Session { get; private set; }

        public Packet(IPeer session, short protocolId, byte[] buffer, int length)
        {
            Session = session;
            ProtocolId = protocolId;

            Data = new byte[length];
            Array.Copy(buffer, Data, length);
        }
    }
}