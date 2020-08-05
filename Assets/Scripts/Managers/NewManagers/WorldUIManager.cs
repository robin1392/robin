using System.Collections;
using System.Collections.Generic;
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

    public Image imageTopHPBar;
    public Image imageBottomHPBar;

    public Text text_TopHealth;
    public Text text_BottomHealth;

    
    //public Image image_HealthBar;
    //public Text text_Health;
    
    #endregion
    
    #region unity base
    
    public override void Awake()
    {
        base.Awake();

        InitializeManager();
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
        tmpWave.text = $"{wave}";
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

    public Image GetHealthBar(bool bottom)
    {
        return bottom ? imageBottomHPBar : imageTopHPBar;
    }

    public Text GetHealthText(bool bottom)
    {
        return bottom ? text_BottomHealth : text_TopHealth;
    }


    //public void SetHealth(bool bottom)
    //{
        //image_HealthBar = bottom ? imageBottomHPBar : imageTopHPBar;
        //text_Health = bottom ? text_BottomHealth : text_TopHealth;
    //}

    //public void SetHealthText(float currentHp)
    //{
        //text_Health.text = $"{Mathf.CeilToInt(currentHp)}";
    //}
    #endregion

    #region system

    
    #endregion


    
}
