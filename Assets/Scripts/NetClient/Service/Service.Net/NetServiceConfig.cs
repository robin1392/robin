using System;
using Service.Core;

namespace Service.Net
{
    public class NetServiceConfig
    {
        public ILogger Logger { get; set; }
        public int Port { get; set; }
        public int MaxConnectionNum { get; set; }
        public int BufferSize { get; set; }
        public int KeepAliveInterval{ get; set; }
        public int KeepAliveTime { get; set; }
        public bool OnMonitoring { get; set; }
        public bool IsLocalServer { get; set; }
    }
}