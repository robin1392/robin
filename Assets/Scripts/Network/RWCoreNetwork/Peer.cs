using System;

namespace RWCoreNetwork
{
    public class Peer
    {
        protected UserToken _userToken;

        public bool IsDisconnected()
        {
            return _userToken == null || _userToken.NetState == NetService.ENetState.Disconnected;
        }

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
            //_userToken = null;
        }
    }
}
