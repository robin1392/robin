using System;

namespace RWCoreNetwork
{
    public interface IPeer
    {
        void SendPacket(short protocolId, byte[] msg);

        bool ReceivePacket(short protocolId, byte[] msg);

        void OnRemoved();

        void Disconnect();
    }

    public class ServerPeer : IPeer
    {
        UserToken _userToken;

        public void SetUserToken(UserToken userToken)
        {
            _userToken = userToken;
            _userToken.SetPeer(this);
        }


        public void SendPacket(short protocolId, byte[] msg)
        {
            _userToken.Send(protocolId, msg);
        }

        public void Disconnect()
        {
            _userToken.Disconnect();
        }

        public bool ReceivePacket(short protocolId, byte[] msg)
        {
            return true;
        }

        public void OnRemoved()
        {

        }
    }
}
