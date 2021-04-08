using System;

namespace Service.Net
{

    public class Peer : ClientSession
    {
        public UserToken UserToken { get; private set; }


        public void Dispose()
        {
            UserToken = null;
        }

        public bool IsConnected()
        {
            return UserToken != null 
            && UserToken.Socket != null 
            && UserToken.Socket.Connected;
        }


        public bool Compare(Peer other)
        {
            if (other.UserToken == null)
            {
                return false;
            }
            
            return this.UserToken.SessionId == other.UserToken.SessionId;
        }


        public void SetUserToken(UserToken userToken)
        {
            UserToken = userToken;
            if (UserToken != null)
            {
                UserToken.Peer = this;
            }
        }


        public override bool Send(int protocolId, byte[] buffer)
        {
            if (UserToken == null 
            || UserToken.NetState != ENetState.Connected)
            {
                return false;
            }

            UserToken.Send(protocolId, buffer, buffer.Length);
            return true;
        }


        public virtual void Disconnect(ESessionState sessionState)
        {
            if (UserToken == null 
            || UserToken.SessionState != ESessionState.None)
            {
                return;
            }

            UserToken.SessionState = sessionState;
        }
    }
}