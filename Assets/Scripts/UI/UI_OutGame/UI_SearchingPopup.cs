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
        if (WebPacket.Get() != null)
        {
            // 이미 상대 찾아서 커넥트 중이면 취소 못한다..
            if (WebPacket.Get().netMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
                return;
        }
        
        //
        UI_Main.Get().Click_DisconnectButton();
        Close();
    }
    #endregion
    
}
