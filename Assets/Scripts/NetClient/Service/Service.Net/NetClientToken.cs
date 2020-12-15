using System;

namespace Service.Net
{
    public class NetClientToken
    {
        public string ServerAddr { get; set; }
        public int Port { get; set; }
        public string PlayerSessionId { get; set; }
        public ENetState NetState { get; set; }
        public long PlayTimeStampTick { get; set; }
    }
}