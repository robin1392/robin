using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RWCoreNetwork
{
    /// <summary>
    /// SocketAsyncEventArgs objects
    /// </summary>   
    public class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;
        object m_lockPool = new object();

        public int Count
        {
            get { return m_pool.Count; }
        }

        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// SocketAsyncEventArg object를 pool에 추가한다.
        /// </summary>   
        public void Push(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("SocketAsyncEventArgsPool cannot be null");
            }

            lock(m_lockPool)
            {
                m_pool.Push(args);
            }
        }

        /// <summary>
        /// SocketAsyncEventArg object를 pool에서 삭제한다.
        /// </summary> 
        public SocketAsyncEventArgs Pop()
        {
            lock(m_lockPool)
            {
                return m_pool.Pop();
            }
        }
    }
}