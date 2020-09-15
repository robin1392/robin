using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWCoreNetwork.NetPacket
{
    public interface IPacketProcessor
    {
        bool Run(Peer peer, short protocolId, byte[] data);
    }

    public class Packet
    {
        public short ProtocolId { get; private set; }
        public byte[] Data { get; private set; }
        public Peer Peer { get; private set; }

        public Packet(Peer peer, short protocolId, byte[] buffer, int length)
        {
            Peer = peer;
            ProtocolId = protocolId;

            Data = new byte[length];
            Array.Copy(buffer, Data, length);
        }
    }
}
