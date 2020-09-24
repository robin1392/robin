using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWCoreNetwork.NetPacket
{
    public interface IPacketReceiver
    {
        bool Process(Peer peer, short protocolId, byte[] data);
    }

    public class Packet
    {
        public short ProtocolId { get; private set; }
        public byte[] Data { get; private set; }
        public Peer Peer { get; private set; }

        public Packet(Peer peer, byte[] buffer)
        {
            Peer = peer;
            ProtocolId = BitConverter.ToInt16(buffer, 0);

            int headerSize = Defines.PROTOCOL_ID + Defines.HEADERSIZE;
            Data = new byte[buffer.Length - headerSize];
            Array.Copy(buffer, headerSize, Data, 0, buffer.Length - headerSize);
        }
    }
}
