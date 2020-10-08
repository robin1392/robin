using System;

namespace RWCoreNetwork
{
    public class Peer
    {
        protected ClientSession _clientSession;

        public bool IsDisconnected()
        {
            return _clientSession == null || _clientSession.NetState == NetService.ENetState.Disconnected;
        }

        public void SetClientSession(ClientSession clientSession)
        {
            _clientSession = clientSession;

            if (_clientSession != null)
            {
                _clientSession.SetPeer(this);
            }
        }

        public ClientSession GetClientSession()
        {
            return _clientSession;
        }


        public void SendPacket(int protocolId, byte[] msg)
        {
            if (_clientSession.NetState < NetService.ENetState.Offline)
            {
                _clientSession.Send(protocolId, msg, msg.Length);
            }
        }

        public void Disconnect()
        {
            _clientSession.Disconnect();
        }

        public bool ReceivePacket(short protocolId, byte[] msg)
        {
            return true;
        }

        public void OnRemoved()
        {
            //_clientSession = null;
        }
    }
}
