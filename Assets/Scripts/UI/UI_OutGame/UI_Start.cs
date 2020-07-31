using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Start : SingletonDestroy<UI_Start>
{

    private Text textGameStatus;
    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();

        InitUIElement();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion
    
    
    #region ui component

    public void InitUIElement()
    {
        textGameStatus = this.transform.Find("PanelTitle/Text_Status").GetComponent<Text>();
    }
    #endregion
    
    
    #region system

    public void SetTextStatus(string statusText)
    {
        textGameStatus.text = $"{statusText}";
    }
    #endregion
    
    
}
