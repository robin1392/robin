using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWCoreNetwork.NetPacket
{
    public interface IPacketProcessor
    {
        bool Run(IPeer peer, short protocolId, byte[] data);
    }

    public class Packet
    {
        public short ProtocolId { get; private set; }
        public byte[] Data { get; private set; }
        public IPeer Peer { get; private set; }

        public Packet(IPeer peer, short protocolId, byte[] buffer, int length)
        {
            Peer = peer;
            ProtocolId = protocolId;

            Data = new byte[length];
            Array.Copy(buffer, Data, length);
        }
    }
}
