using System;

namespace Service.Net
{
    public class ClientSession
    {
        public readonly string SessionKey;
        public readonly string UserId;
        public readonly string PlatformId;


        public ClientSession()
        {
            SessionKey = string.Empty;
            UserId = string.Empty;
            PlatformId = string.Empty;
        }


        public ClientSession(string sessionKey, string userId, string platformId)
        {
            SessionKey = sessionKey;
            UserId = userId;
            PlatformId = platformId;
        }

        public virtual bool Send(int protocolId, byte[] buffer)
        {
            return false;
        }

        public virtual bool Send(int protocolId, string method, string json)
        {
            return false;
        }
    }


}