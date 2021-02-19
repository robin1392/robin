using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_CoopSpawnTurn : MonoBehaviour
{
    public RectTransform rts_Left;
    public RectTransform rts_Right;

    public Text text_MyName;
    public Text text_OtherName;
    public Color color;

    private bool isMyTurn;

    public void Set(bool isMyTurn)
    {
        this.isMyTurn = isMyTurn;
        SetUI();
    }

    private void SetUI()
    {
        rts_Left.gameObject.SetActive(isMyTurn);
        rts_Right.gameObject.SetActive(!isMyTurn);

        text_MyName.color = isMyTurn ? color : Color.white;
        text_OtherName.color = isMyTurn ? Color.white : color;
    }

    public void Reverse()
    {
        isMyTurn = !isMyTurn;
        SetUI();
    }
}
