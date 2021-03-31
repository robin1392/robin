using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ED;

public class UI_WinLose : MonoBehaviour
{
    public GameObject obj_Win;
    public GameObject obj_Lose;
    public Image image_DeckBG;
    public Image[] arrImage_Deck;
    public Image image_Guardian;
    public Text text_GuardianName;
    public Image image_Tier_Icon;
    public Text text_Nickname;
    public Text text_Trophy;

    public void Initialize(bool isWin, bool isPerfect, int winningStreak, int[] deck, int guadianId, string nickname, int trophy)
    {
        if (InGameManager.Get().playType == PLAY_TYPE.BATTLE)
        {
            obj_Win.SetActive(isWin);
            obj_Lose.SetActive(!isWin);
        }
        
        if (isWin && isPerfect)
        {
            //Invoke("ActivatePerfect", 2f);
        }
        
        for (int i = 0; i < deck.Length; i++)
        {
            RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
            if (TableManager.Get().DiceInfo.GetData(deck[i], out dataDiceInfo) == false)
            {
                continue;
            }
                
            arrImage_Deck[i].sprite = FileHelper.GetIcon(dataDiceInfo.iconName);
            arrImage_Deck[i].SetNativeSize();
            arrImage_Deck[i].transform.localScale = Vector3.zero;
            arrImage_Deck[i].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuint).SetDelay(i * 0.1f)
                .SetUpdate(true);
        }
        
        RandomWarsResource.Data.TDataGuardianInfo dataGuardianInfo;
        if (TableManager.Get().GuardianInfo.GetData(guadianId, out dataGuardianInfo) == false)
        {
            image_Guardian.sprite = FileHelper.GetIcon(dataGuardianInfo.iconName);
            text_GuardianName.text = dataGuardianInfo.name;    
        }

        text_Nickname.text = nickname;
        text_Trophy.text = trophy.ToString();
    }

    public void Initialize(int guadianId, int[] deck, string nickname, int trophy)
    {
        obj_Win.SetActive(false);
        obj_Lose.SetActive(false);
        
        for (int i = 0; i < 5; i++)
        {
            RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
            if (TableManager.Get().DiceInfo.GetData(deck[i], out dataDiceInfo) == false)
            {
                continue;
            }
                
            // arrImage_Deck[i].sprite = FileHelper.GetIcon(dataDiceInfo.iconName);
            arrImage_Deck[i].SetNativeSize();
            arrImage_Deck[i].transform.localScale = Vector3.zero;
            arrImage_Deck[i].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuint).SetDelay(i * 0.1f)
                .SetUpdate(true);
        }
        
        RandomWarsResource.Data.TDataGuardianInfo dataGuardianInfo;
        if (TableManager.Get().GuardianInfo.GetData(guadianId, out dataGuardianInfo) == true)
        {
            // image_Guardian.sprite = FileHelper.GetIcon(dataGuardianInfo.iconName);
            text_GuardianName.text = dataGuardianInfo.name;   
        }

        text_Nickname.text = nickname;
        text_Trophy.text = trophy.ToString();
    }

    // private void ActivatePerfect()
    // {
    //     obj_Perfect.SetActive(true);
    //
    //     SoundManager.instance.Play(Global.E_SOUND.SFX_UI_PERFECT);
    // }
}
