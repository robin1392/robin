using System.Collections;
using System.Collections.Generic;
using RWCoreNetwork;
using RWGameProtocol.Msg;
using UnityEngine;


public class SocketRecvEvent 
{
    #region init 
    // 
    public SocketRecvEvent()
    {
    }
    
    
    #endregion
    
    
    
    
    #region join room
    // <summary>
    /// 게임 참가 응답 처리부
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="msg"></param>
    public void OnJoinGameAck(IPeer peer, MsgJoinGameAck msg)
    {
        // something to do...

        //NetworkManager.Get().SendSocket.ReadyGameReq(peer);
        //SendSocket.ReadyGameReq(peer);
    }
    
    #endregion
    
    
    

}
