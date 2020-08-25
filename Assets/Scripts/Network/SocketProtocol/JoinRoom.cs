using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

/*
namespace RWSocketProtocol.Protocol
{
    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public struct UserInfo
    {
        public int UserId;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string UserName;
    }

    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)] 
    public class RoomInfo
    {
        public int RoomId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UserInfo[] UserInfoArray = new UserInfo[4];
    }

    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinRoomReq : Serializer<MsgJoinRoomReq>
    {        
        public int RoomId;
    }

    [Serializable] 
    [StructLayout(LayoutKind.Sequential, Pack = 1)] 
    public class MsgJoinRoomAck : Serializer<MsgJoinRoomAck>
    {        
        public RoomInfo RoomInfo;
    }
}*/