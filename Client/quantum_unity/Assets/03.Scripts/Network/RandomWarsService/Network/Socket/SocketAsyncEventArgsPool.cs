using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RandomWarsService.Network.Socket
{
    /// <summary>
    /// SocketAsyncEventArgs objects
    /// </summary>   
    public class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> _pool;
        object _lockPool = new object();

        public int Count
        {
            get { return _pool.Count; }
        }

        public SocketAsyncEventArgsPool(int capacity)
        {
            _pool = new Stack<SocketAsyncEventArgs>(capacity);
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

            lock(_lockPool)
            {
                _pool.Push(args);
            }
        }

        /// <summary>
        /// SocketAsyncEventArg object를 pool에서 삭제한다.
        /// </summary> 
        public SocketAsyncEventArgs Pop()
        {
            lock(_lockPool)
            {
                return _pool.Pop();
            }
        }
    }
}