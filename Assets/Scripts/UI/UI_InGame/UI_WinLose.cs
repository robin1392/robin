using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        obj_Win.SetActive(!isWin);
        obj_Perfect.SetActive(isPerfect);
        image_DeckBG.sprite = arrSprite_DeckBG[isWin ? 1 : 0];
        
        for (int i = 0; i < arrImage_Deck.Length; i++)
        {
            var iconName = JsonDataManager.Get().dataDiceInfo.dicData[deck[i]].iconName;
            arrImage_Deck[i].sprite = FileHelper.GetIcon(iconName);
        }

        text_Nickname.text = nickname;
        text_Trophy.text = trophy.ToString();
    }
}
