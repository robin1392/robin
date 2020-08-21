using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILocalizationHelp : MonoBehaviour
{

    [Header("Localization Info")] 
    public int localizeLangKey;


    #region unity base
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
    
    #region convert lang

    public void ConvertLang()
    {
        Text textThis = this.transform.GetComponent<Text>();
        
    }
    #endregion
    
    
}
