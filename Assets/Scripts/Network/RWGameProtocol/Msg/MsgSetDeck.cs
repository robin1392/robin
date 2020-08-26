using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetDeckReq : Serializer<MsgSetDeckReq>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Deck = new int[5];
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSetDeckAck : Serializer<MsgSetDeckAck>
    {
        public short ErrorCode;
    }


}
