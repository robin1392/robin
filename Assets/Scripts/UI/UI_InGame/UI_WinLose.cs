using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_WinLose : MonoBehaviour
{
    public GameObject obj_Win;
    public GameObject obj_Lose;
    public GameObject obj_Perfect;
    public Image image_DeckBG;
    public Sprite[] arrSprite_DeckBG;
    public Image[] arrImage_Deck;
    public Image image_Tier_Icon;
    public Text text_Nickname;
    public Text text_Trophy;

    public void Initialize(bool isWin, bool isPerfect, int[] deck, string nickname, int trophy)
    {
        obj_Win.SetActive(isWin);
        obj_Lose.SetActive(!isWin);
        if (isPerfect) Invoke("ActivatePerfect", 2f);
        image_DeckBG.sprite = arrSprite_DeckBG[isWin ? 1 : 0];
        
        for (int i = 0; i < arrImage_Deck.Length; i++)
        {
            var iconName = JsonDataManager.Get().dataDiceInfo.dicData[deck[i]].iconName;
            arrImage_Deck[i].sprite = FileHelper.GetIcon(iconName);
            arrImage_Deck[i].transform.localScale = Vector3.zero;
            arrImage_Deck[i].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuint).SetDelay(i * 0.1f);
        }

        text_Nickname.text = nickname;
        text_Trophy.text = trophy.ToString();
    }

    private void ActivatePerfect()
    {
        obj_Perfect.SetActive(true);
    }
}
