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
        protected UserToken _userToken;

        public void SetUserToken(UserToken userToken)
        {
            _userToken = userToken;

            if (_userToken != null)
            {
                _userToken.SetPeer(this);
            }
        }

        public UserToken GetUserToken()
        {
            return _userToken;
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
            _userToken = null;
        }
    }
}
