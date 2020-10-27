using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;

public class UI_SearchingPopup : UI_Popup
{

    #region  unity base

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
