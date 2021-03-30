using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using ED;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = UnityEngine.Debug;

public class WorldUIManager : SingletonDestroy<WorldUIManager>
{
    
    
    
    #region world ui element

    [Header("Stage UI")] 
    public Image imageSpawnTimeGray;
    public Image imageSpawnTime;
    public Image imageTimerIcon;
    public Text textSpawnTime;
    public Text textWave;
    public Text textAddSP;

    [Header("Canvas")] public Canvas canvas_UnitHPBar;

    
    //public Image image_HealthBar;
    //public Text text_Health;
    
    #endregion
    
    #region unity base
    
    public override void Awake()
    {
        base.Awake();

        InitializeManager();
    }

    public override void OnDestroy()
    {
        DestroyManager();
        
        base.OnDestroy();
    }

    #endregion
    
    
    #region init & destroy

    public void InitializeManager()
    {
        
    }

    public void DestroyManager()
    {
        
    }

    #endregion
    
    
    
    
    #region get set

    public void SetWave(int wave)
    {
        textWave.text = $"{wave}";
    }

    public void RotateTimerIcon()
    {
        imageTimerIcon.rectTransform.DOLocalRotate(new Vector3(0, 0, 359f), 1f, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            imageTimerIcon.rectTransform.localRotation = Quaternion.identity;
        });
    }

    public void SetTextSpawnTime(float time)
    {
        string str = $"{Mathf.CeilToInt(time):F0}";
        if (String.CompareOrdinal(textSpawnTime.text, str) != 0)
        {
            textSpawnTime.text = str;
            // textSpawnTime.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }
    }

    public AnimationCurve curve;
    private bool isGaugeTweening;
    
    public void SetSpawnTime(float amount)
    {
        imageSpawnTimeGray.fillAmount = amount;
    }

    public float GetSpawnAmount()
    {
        return imageSpawnTimeGray.fillAmount;
    }

    public void AddSP(int addSP)
    {
        SetAddSpText(addSP);
        textAddSP.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
    }

    public void SetAddSpText(int addSp)
    {
        textAddSP.text = $"{addSp}";
    }

    #endregion

    #region system

    
    #endregion


    
}
