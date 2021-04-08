using System;
using System.Collections.Generic;
using Service.Core;

namespace Service.Net
{
    public class MessageQueue
    {
        private Queue<Message> _msgQueue;
        private ObjectPool<Message> _msgPool;


        public MessageQueue()
        {
            _msgQueue = new Queue<Message>();
            _msgPool = new ObjectPool<Message>();
        }


        public void Init(int poolCapacity, int bufferSize)
        {
            for (int i = 0; i < poolCapacity; i++)
            {
                Message obj = new Message();
                obj.Init(bufferSize);
                _msgPool.Push(obj);
            }
        }


        public virtual void Enqueue(ClientSession session, byte[] buffer)
        {
            Message msg = _msgPool.Pop();
            msg.Set(session, buffer);

            lock (_msgQueue)
            {
                _msgQueue.Enqueue(msg);
            }
        }

        
        public virtual void Enqueue(ClientSession session, int protocolId, byte[] data, int length)
        {
            Message msg = _msgPool.Pop();
            msg.Set(session, protocolId, data, length);

            lock (_msgQueue)
            {
                _msgQueue.Enqueue(msg);
            }
        }


        public  void Dequeue()
        {
            Message msg = null;
            lock (_msgQueue)
            {
                msg = _msgQueue.Dequeue();
            }

            //_msgPool.Push(msg);
        }


        public Message Peek()
        {
            lock (_msgQueue)
            {
                return (_msgQueue.Count > 0)
                    ? _msgQueue.Peek() : null;
            }
        }
    }
}