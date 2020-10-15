using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RWCoreNetwork
{
    /// <summary>
    /// 소켓 I/O용 버퍼를 SocketAsyncEventArgs objects에 할당/해제함.(not thread safe)
    /// </summary>
    internal class BufferHandler
    {
        int m_numBytes;
        byte[] m_buffer;
        Stack<int> m_freeIndexPool;
        int m_currentIndex;
        int m_bufferSize;


        public BufferHandler(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// 버퍼를 초기화한다.
        /// </summary>
        public void InitBuffer()
        {
            m_buffer = new byte[m_numBytes];
        }

        /// <summary>
        /// SocketAsyncEventArgs object에 버퍼를 설정한다.
        /// </summary>
        /// <returns>할당 성공시 true, 실패시 false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }

                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }

            return true;
        }

        /// <summary>
        /// SocketAsyncEventArgs object 버퍼를 해제한다.
        /// </summary>   
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}