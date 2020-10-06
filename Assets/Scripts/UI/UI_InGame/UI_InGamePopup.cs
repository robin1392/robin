using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ED;

public class UI_InGamePopup : SingletonDestroy<UI_InGamePopup>
{
    
    
    
    #region ui element variable
    public GameObject popup_Waiting;
    public GameObject popup_Result;
    public Text text_Result;
    

        
    public GameObject obj_Low_HP_Effect;
    //[Header("DEV UI")] 

    public GameObject obj_Indicator;
    #endregion



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
        DestroyElement();
        
        base.OnDestroy();
    }

    #endregion
    
    #region init destroy

    public void InitUIElement()
    {
        
    }

    public void DestroyElement()
    {
        
    }
    #endregion

    
    
    public void SetViewWaiting(bool view)
    {
        popup_Waiting.SetActive(view);
    }

    public void SetPopupResult(bool view)
    {
        popup_Result.SetActive(view);
    }

    public void SetResultText(string text)
    {
        text_Result.text = text;
    }

    public void ViewLowHP(bool view)
    {
        obj_Low_HP_Effect.SetActive(view);
    }

    public bool GetLowHP()
    {
        return obj_Low_HP_Effect.activeSelf;
    }

    public void ViewGameIndicator(bool view)
    {
        obj_Indicator.SetActive(view);
    }
}
