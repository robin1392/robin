using System;

namespace Service.Net
{
    public class Peer
    {
        public ClientSession ClientSession { get; set; }


        public void Clear()
        {
            ClientSession = null;
        }       


        public bool IsConnected()
        {
            return ClientSession != null 
            && ClientSession.Socket != null 
            && ClientSession.Socket.Connected;
        }


        public Peer[] GetPeers(EPeerGroupType peerGroupType)
        {
            if (ClientSession.GameSession == null)
            {
                return new Peer[1];
            }

            return ClientSession.GameSession.GetPeers(peerGroupType, this);
        }

        public bool Compare(Peer other)
        {
            if (other.ClientSession == null)
            {
                return false;
            }
            
            return this.ClientSession.Id == other.ClientSession.Id;
        }


        public void SetClientSession(ClientSession clientSession)
        {
            ClientSession = clientSession;
            if (ClientSession != null)
            {
                ClientSession.Peer = this;
            }
        }


        public virtual bool SendPacket(int protocolId, byte[] msg)
        {
            if (ClientSession == null 
            || ClientSession.NetState != ENetState.Connected)
            {
                return false;
            }

            ClientSession.Send(protocolId, msg, msg.Length);
            return true;
        }


        public virtual void Disconnect(ESessionState sessionState)
        {
            if (ClientSession == null 
            || ClientSession.SessionState != ESessionState.None)
            {
                return;
            }

            ClientSession.SessionState = sessionState;
            // ClientSession.SendNetDisconnectNotify();
            // ClientSession.Disconnect();
            // if (ClientSession.GameSession != null)
            // {
            //     ClientSession.GameSession.PushInternalMessage(ClientSession, EInternalProtocol. DISCONNECT_CLIENT, null, 0);
            // }
            
        }
    }
}