using Service.Core;

namespace Percent
{
    public class GameBaseClientConfig
    {
        public int MaxConnectionNum { get; set; }
        public int BufferSize { get; set; }
        public int KeepAliveTime { get; set; }
        public int KeepAliveInterval { get; set; }
        public int MessageQueueCapacity { get; set; }


        public GameBaseClientConfig()
        {
            MaxConnectionNum = 1;
            BufferSize = 10240;
            KeepAliveTime = 5000;
            KeepAliveInterval = 1000;
            MessageQueueCapacity = 100;
        }
    }
}