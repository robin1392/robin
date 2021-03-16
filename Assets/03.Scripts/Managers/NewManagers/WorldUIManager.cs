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

    public void Start()
    {
        textAddSP.DOFade(0f, 0f);
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
        // if (amount < 0.05f && InGameManager.Get().time > 2f) amount = 1f;
        imageSpawnTimeGray.fillAmount = amount;

        if (!isGaugeTweening)
        {
            if (amount < 0.2f) imageSpawnTime.fillAmount = 0f;
            else if (amount > 0.25f && imageSpawnTime.fillAmount < 0.25f)
            {
                isGaugeTweening = true;
                imageSpawnTime.DOFillAmount(0.26f, 0.2f).SetEase(curve).OnComplete(() =>
                {
                    imageSpawnTime.DOFillAmount(0.25f, 0.1f).OnComplete(() =>
                    {
                        isGaugeTweening = false;
                    });
                });
            }
            else if (amount > 0.5f && imageSpawnTime.fillAmount < 0.5f)
            {
                isGaugeTweening = true;
                imageSpawnTime.DOFillAmount(0.51f, 0.2f).SetEase(curve).OnComplete(() =>
                {
                    imageSpawnTime.DOFillAmount(0.5f, 0.1f).OnComplete(() =>
                    {
                        isGaugeTweening = false;
                    });
                });
            }
            else if (amount > 0.75f && imageSpawnTime.fillAmount < 0.75f)
            {
                isGaugeTweening = true;
                imageSpawnTime.DOFillAmount(0.76f, 0.2f).SetEase(curve).OnComplete(() =>
                {
                    imageSpawnTime.DOFillAmount(0.75f, 0.1f).OnComplete(() =>
                    {
                        isGaugeTweening = false;
                    });
                });
            }
            // else if (amount > 0.8f && imageSpawnTime.fillAmount < 0.8f)
            // {
            //     isGaugeTweening = true;
            //     imageSpawnTime.DOFillAmount(0.81f, 0.2f).SetEase(curve).OnComplete(() =>
            //     {
            //         imageSpawnTime.DOFillAmount(0.8f, 0.1f).OnComplete(() =>
            //         {
            //             isGaugeTweening = false;
            //         });
            //     });
            // }
            else if (amount >= 1f && imageSpawnTime.fillAmount < 1f) 
                imageSpawnTime.DOFillAmount(1f, 0.2f).SetEase(curve);
        }
    }

    public float GetSpawnAmount()
    {
        return imageSpawnTimeGray.fillAmount;
    }

    public void AddSP(int addSP)
    {
        SetAddSpText(addSP);
        // textAddSP.transform.DOScale(1.3f, 0.1f).OnComplete(() =>
        // {
        //     textAddSP.transform.DOScale(1f, 0.2f);
        // });
        Sequence sq = DOTween.Sequence().OnStart(() =>
        {
            textAddSP.transform.localScale = Vector3.zero;
        }).Append(textAddSP.transform.DOScale(1f, 0.3f))
            .SetEase(Ease.OutBack);
        textAddSP.DOFade(1f, 0.5f).OnComplete(() =>
        {
            textAddSP.DOFade(0f, 0.5f);
        });
    }

    public void SetAddSpText(int addSp)
    {
        textAddSP.text = $"+{addSp}";
    }

    #endregion

    #region system

    
    #endregion


    
}
