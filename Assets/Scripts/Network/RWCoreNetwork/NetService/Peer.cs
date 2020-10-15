using System;

namespace RWCoreNetwork.NetService
{
    public class Peer
    {
        public  ClientSession ClientSession { get; private set; }

        public bool IsDisconnected()
        {
            return ClientSession == null || ClientSession.NetState == ENetState.Disconnected;
        }

        public void SetClientSession(ClientSession clientSession)
        {
            ClientSession = clientSession;

            if (ClientSession != null)
            {
                ClientSession.SetPeer(this);
            }
        }

  
        public virtual void SendPacket(int protocolId, byte[] msg)
        {
            if (ClientSession.NetState != ENetState.Connected)
            {
                return;
            }

            ClientSession.Send(protocolId, msg, msg.Length);
        }


        public virtual void Disconnect(ESessionState sessionState)
        {
            if (ClientSession.SessionState != ESessionState.None)
            {
                return;
            }

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


    public class ServerPeer : Peer
    {
        public override void Disconnect(ESessionState sessionState)
        {
            if (ClientSession.SessionState != ESessionState.None)
            {
                return;
            }


            ClientSession.SessionState = sessionState;

            // 세션 접속 종료 상태를 클라이언트에게 먼저 알리고 소켓 연결을 끊는다.
            ClientSession.SendInternalDisconnectSessionNotify();
        }
    }
}
