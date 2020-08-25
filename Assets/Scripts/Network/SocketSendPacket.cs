using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RWSocketProtocol.Protocol;


public class SocketSendPacket
{
    private SocketManager _clientService;
    
    public SocketSendPacket(SocketManager service)
    {
        _clientService = service;
    }


    public void SendTest(int var)
    {
        //MsgJoinRoomReq req = new MsgJoinRoomReq();
        //req.RoomId = roomId;
        //_clientService.Send((short)GameProtocol.MSG_JOIN_ROOM_REQ, req.Serialize());
    }
    
    
}
