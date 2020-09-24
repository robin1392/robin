using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWCoreNetwork.NetPacket
{
    public interface IPacketReceiver
    {
        bool Process(Peer peer, int protocolId, byte[] msg);
    }

    public class Packet
    {
        public int ProtocolId { get; private set; }
        public byte[] Msg { get; private set; }
        public Peer Peer { get; private set; }

        public Packet(Peer peer, byte[] buffer)
        {
            Peer = peer;
            ProtocolId = BitConverter.ToInt32(buffer, 0);

            Msg = new byte[buffer.Length - Defines.HEADER_SIZE];
            Array.Copy(buffer, Defines.HEADER_SIZE, Msg, 0, buffer.Length - Defines.HEADER_SIZE);
        }
    }
}
