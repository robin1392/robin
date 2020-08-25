using System;

namespace RWCoreNet
{
    public interface IPeer
    {
        void SendPacket(short protocolId, byte[] msg);

        bool ReceivePacket(short protocolId, byte[] msg);

        void OnRemoved();

        void Disconnect();
    }
}