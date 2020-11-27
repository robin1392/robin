using System.Threading;


namespace Service.Net
{
    public class MessageQueueThread : MessageQueue
    {
        public delegate bool ProcessMessageDelegate(Message msg);
        public ProcessMessageDelegate ProcessMessageCallback;
        private Thread _thread;
        private AutoResetEvent _loopEvent;


        public MessageQueueThread()
        {
            _loopEvent = new AutoResetEvent(false);
            _thread = new Thread(new ThreadStart(DoWork));
        }


        public void Init(int poolCapacity, int bufferSize, ProcessMessageDelegate processMessageCallback)
        {
            base.Init(poolCapacity, bufferSize);
            _thread.Start();

            ProcessMessageCallback = processMessageCallback;
        }


        private void DoWork()
        {
            Message msg = null;
            while (true)
            {
                msg = Peek();
                if (msg == null)
                {
                    // 더이상 처리할 패킷이 없으면 스레드 대기.
                    _loopEvent.WaitOne();
                    continue;
                }


                if (ProcessMessageCallback(msg) == true)
                {
                    Dequeue();
                }
            }
        }
        

        public override void Enqueue(ClientSession clientSession, byte[] buffer)
        {
            base.Enqueue(clientSession, buffer);
            _loopEvent.Set();
        }


        public override void Enqueue(ClientSession clientSession, int protocolId, byte[] data, int length)
        {
            base.Enqueue(clientSession, protocolId, data, length);
            _loopEvent.Set();
        }
    }
}