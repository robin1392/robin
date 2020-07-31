using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : SingletonDestroy<UI_Loading>
{

    private Image imageProgress;
    private Text textProgress;
    
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
    #endregion
    
    #region ui component

    public void InitUIElement()
    {
        imageProgress = this.transform.Find("Image_Progress").GetComponent<Image>();
        textProgress = this.transform.Find("Image_Progress/TextProgress").GetComponent<Text>();
    }
    #endregion
    
    #region system

    public void SetViewProgress(float progress)
    {
        imageProgress.fillAmount = progress < 1.0f ? progress : 1.0f;
        
        textProgress.text = $"{progress * 100.0f}%";
        
    }
    #endregion
    
}
