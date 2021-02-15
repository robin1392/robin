using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldUIManager : SingletonDestroy<WorldUIManager>
{
    
    
    
    #region world ui element

    [Header("Stage UI")] 
    public Image imageSpawnTime;
    public Text textSpawnTime;
    public TextMeshProUGUI tmpWave;
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
        tmpWave.text = wave.ToString(); //$"{wave}";
    }

    public void SetTextSpawnTime(float time)
    {
        textSpawnTime.text = $"{Mathf.CeilToInt(time):F0}";
    }

    public void SetSpawnTime(float amount)
    {
        imageSpawnTime.fillAmount = amount;
    }

    public float GetSpawnAmount()
    {
        return imageSpawnTime.fillAmount;
    }

    public void AddSP(int addSP)
    {
        textAddSP.text = $"+{addSP}";
        textAddSP.DOFade(1f, 0.5f).OnComplete(() =>
        {
            textAddSP.DOFade(0f, 0.5f);
        });
    }

    #endregion

    #region system

    
    #endregion


    
}
