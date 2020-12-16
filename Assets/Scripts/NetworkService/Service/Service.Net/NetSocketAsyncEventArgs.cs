using System.Net.Sockets;
using Service.Core;


namespace Service.Net
{
    public class NetSocketAsyncEventArgs : SocketAsyncEventArgs, IObjectPool
    {
        public void Clear()
        {
        }
    }

}