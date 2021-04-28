using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using RandomWarsService.Network.Socket.NetService;

namespace RandomWarsService.Network.Socket.NetPacket
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


        public Packet(Peer peer, int protocolId, byte[] msg, int length)
        {
            Peer = peer;
            ProtocolId = protocolId;
            Length = length;
            Msg = msg;
        }
    }
}
