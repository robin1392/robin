using System;
using Service.Core;

namespace Service.Net
{
    public class Message : IObjectPool
    {
        public int ProtocolId { get; private set; }
        public int Length { get; private set; }
        public byte[] Data { get; private set; }
        public ClientSession Session { get; private set; }


        public void Init(int bufferSize)
        {
            Data = new byte[bufferSize];
        }


        public void Clear()
        {
            ProtocolId = 0;
            Length = 0;
            Array.Clear(Data, 0, Data.Length);
            Session = null;
        }


        public void Set(ClientSession session, byte[] buffer)
        {
            Session = session;
            ProtocolId = BitConverter.ToInt32(buffer, 0);
            Length = BitConverter.ToInt32(buffer, Defines.PROTOCOL_ID);

            if (Length > 0)
            {
                Array.Copy(buffer, Defines.HEADER_SIZE, Data, 0, Length);
            }
        }


        public void Set(ClientSession session, int protocolId, byte[] data, int length)
        {
            Session = session;
            ProtocolId = protocolId;
            Length = length;

            if (Length > 0)
            {
                Array.Copy(data, 0, Data, 0, Length);
            }
        }
    }    
}

