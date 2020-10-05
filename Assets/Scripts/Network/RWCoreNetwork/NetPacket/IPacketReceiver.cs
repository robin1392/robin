using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace RWCoreNetwork.NetPacket
{
    public interface IPacketReceiver
    {
        bool Process(Peer peer, int protocolId, byte[] msg);
    }

    public class Packet
    {
        public int ProtocolId { get; private set; }
        public int Length { get; private set; }
        public byte[] Msg { get; private set; }
        public Peer Peer { get; private set; }

        public Packet(Peer peer, byte[] buffer, int bufferLength)
        {
            Peer = peer;
            ProtocolId = BitConverter.ToInt32(buffer, 0);
            Length = BitConverter.ToInt32(buffer, Defines.PROTOCOL_ID);
            Msg = new byte[bufferLength];
            Array.Copy(buffer, Defines.HEADER_SIZE, Msg, 0, Length);
        }
    }
}
