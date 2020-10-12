using System;

namespace RWCoreNetwork
{
    public class Peer
    {
        public  ClientSession ClientSession { get; private set; }

        public bool IsDisconnected()
        {
            return ClientSession == null || ClientSession.NetState == NetService.ENetState.Disconnected;
        }

        public void SetClientSession(ClientSession clientSession)
        {
            ClientSession = clientSession;

            if (ClientSession != null)
            {
                ClientSession.SetPeer(this);
            }
        }

  
        public void SendPacket(int protocolId, byte[] msg)
        {
            if (ClientSession.NetState < NetService.ENetState.Offline)
            {
                ClientSession.Send(protocolId, msg, msg.Length);
            }
        }


        public void Disconnect(ESessionState sessionState)
        {
            ClientSession.SessionState = sessionState;
            ClientSession.Disconnect();
        }


        public bool ReceivePacket(short protocolId, byte[] msg)
        {
            return true;
        }


        public void OnRemoved()
        {
            //ClientSession = null;
        }
    }
}
