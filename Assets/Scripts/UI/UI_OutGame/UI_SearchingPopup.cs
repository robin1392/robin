using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;

public class UI_SearchingPopup : UI_Popup
{

    #region  unity base

    #endregion
    
    #region override
    protected override void OnEnable()
    {
        base.OnEnable();
        
        

    }

    public override void Close()
    {
        base.Close();
        
        
        
    }
    
    #endregion
    
    
    #region click event

    public void ClickSearchingCancel()
    {
        if (NetworkManager.Get() != null)
        {
            if (NetworkManager.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_START)
            {
                // 매칭 요청중이면 중단을 요청한다.
                NetworkManager.Get().StopMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
            }
            else if (NetworkManager.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
            {
                // 이미 상대 찾아서 커넥트 중이면 취소 못한다..
                return;
            }
        }
        
        //
        UI_Main.Get().Click_DisconnectButton();
        Close();
    }
    #endregion
    
}
