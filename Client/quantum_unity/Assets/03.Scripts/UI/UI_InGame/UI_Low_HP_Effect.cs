using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Low_HP_Effect : MonoBehaviour
{
    public Image image;

    private void OnEnable()
    {
        image.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
}
