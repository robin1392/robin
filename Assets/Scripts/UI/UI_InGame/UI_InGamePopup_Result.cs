using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RandomWarsProtocol;
using RandomWarsResource.Data;

public class UI_InGamePopup_Result : MonoBehaviour
{
    [Header("Win Lose")]
    public UI_WinLose winlose_Other;
    public UI_WinLose winlose_My;
    public Text text_VS;
    public Text text_WinningStreak;
    public UI_ResultValue[] arrValue;
    
    [Space]
    public Button btn_ShowValues;
    public Button btn_End;
    
    private bool isWin;
    
    public void Initialize(bool winLose, int winningStreak, MsgReward[] normalReward, MsgReward[] streakReward, MsgReward[] perfectReward)
    {
        if (TutorialManager.isTutorial)
        {
            TutorialManager.stepCount++;
        }
        
// #if UNITY_EDITOR
//         isWin = true;
//         
//         winlose_My.Initialize(isWin, true, new int[] { 1000, 1001, 1002, 1003, 1004 }, "ED", 1234);
//         winlose_Other.Initialize(!isWin, false, new int[] { 2000, 2001, 2002, 2003, 2004 }, "COM", 4321);
//         btn_ShowValues.interactable = false;
//         Invoke("EnableShowValuesButton", 2f);
// #else
        isWin = winLose;

        winlose_My.Initialize(isWin, (perfectReward != null && perfectReward.Length > 0), winningStreak, NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().playerInfo.Name, NetworkManager.Get().GetNetInfo().playerInfo.Trophy);
        winlose_Other.Initialize(!isWin, InGameManager.Get().playerController.targetPlayer.currentHealth > 20000, winningStreak, NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().otherInfo.Name, NetworkManager.Get().GetNetInfo().otherInfo.Trophy);
        btn_ShowValues.interactable = false;

        int normalGold = 0;
        int normalTrophy = 0;
        int normalSeasonTrophy = 0;
        int normalRankTrophy = 0;
        int normalKey = 0;
        if (normalReward != null)
        {
            foreach (var reward in normalReward)
            {
                switch ((EItemListKey)reward.ItemId)
                {
                    case EItemListKey.gold:
                        normalGold = reward.Value;
                        break;
                    case EItemListKey.thropy:
                        normalTrophy = reward.Value;
                        break;
                    case EItemListKey.seasonthropy:
                        normalSeasonTrophy = reward.Value;
                        break;
                    case EItemListKey.rankthropy:
                        normalRankTrophy = reward.Value;
                        break;
                    case EItemListKey.key:
                        normalKey = reward.Value;
                        break;
                }
            }
            
            arrValue[0].Initialize(normalTrophy, normalGold, normalKey);
        }

        int streakGold = 0;
        int streakTrophy = 0;
        int streakSeasonTrophy = 0;
        int streakRankTrophy = 0;
        int streakKey = 0;
        if (streakReward != null)
        {
            foreach (var reward in streakReward)
            {
                switch ((EItemListKey)reward.ItemId)
                {
                    case EItemListKey.gold:
                        streakGold = reward.Value;
                        break;
                    case EItemListKey.thropy:
                        streakTrophy = reward.Value;
                        break;
                    case EItemListKey.seasonthropy:
                        streakSeasonTrophy = reward.Value;
                        break;
                    case EItemListKey.rankthropy:
                        streakRankTrophy = reward.Value;
                        break;
                    case EItemListKey.key:
                        streakKey = reward.Value;
                        break;
                }
            }
            
            arrValue[1].Initialize(streakTrophy, streakGold, streakKey);
            text_WinningStreak.text = winningStreak.ToString();
            text_WinningStreak.color = winningStreak > 0 ? text_WinningStreak.color : Color.gray;
        }

        int perfectGold = 0;
        int perfectTrophy = 0;
        int perfectSeasonTrophy = 0;
        int perfectRankTrophy = 0;
        int perfectKey = 0;
        if (perfectReward != null)
        {
            foreach (var reward in perfectReward)
            {
                switch ((EItemListKey)reward.ItemId)
                {
                    case EItemListKey.gold:
                        perfectGold = reward.Value;
                        break;
                    case EItemListKey.thropy:
                        perfectTrophy = reward.Value;
                        break;
                    case EItemListKey.seasonthropy:
                        perfectSeasonTrophy = reward.Value;
                        break;
                    case EItemListKey.rankthropy:
                        perfectRankTrophy = reward.Value;
                        break;
                    case EItemListKey.key:
                        perfectKey = reward.Value;
                        break;
                }
            }
            
            arrValue[2].Initialize(perfectTrophy, perfectGold, perfectKey);
        }

        int totalTrophy = normalTrophy + streakTrophy + perfectTrophy;
        int totalGold = normalGold + streakGold + perfectGold;
        int totalKey = normalKey + streakKey + perfectKey;
        int totalSeasonTrophy = normalSeasonTrophy + streakSeasonTrophy + perfectSeasonTrophy;
        int totalRankTrophy = normalRankTrophy + streakRankTrophy + perfectRankTrophy;
        arrValue[3].Initialize(totalTrophy, totalGold, totalKey);

        var userInfo = UserInfoManager.Get().GetUserInfo();
        userInfo.trophy += totalTrophy;
        userInfo.seasonTrophy += totalSeasonTrophy;
        userInfo.rankPoint += totalRankTrophy;
        userInfo.gold += totalGold;
        userInfo.key += totalKey;

        Invoke("EnableShowValuesButton", 2f);
//#endif

    }

    private void EnableShowValuesButton()
    {
        btn_ShowValues.interactable = true;
    }

    public void Click_ShowResultValues()
    {
        btn_ShowValues.gameObject.SetActive(false);
        StartCoroutine(ShowResultValuesCoroutine());

        SoundManager.instance.Play(isWin ? Global.E_SOUND.SFX_WIN : Global.E_SOUND.SFX_LOSE);
    }

    IEnumerator ShowResultValuesCoroutine()
    {
        Ease ease = Ease.OutBack;
        ((RectTransform) winlose_Other.transform).DOScale(Vector3.zero, 0.3f).SetEase(ease);
        ((RectTransform) text_VS.transform).DOScale(Vector3.zero, 0.3f).SetEase(ease);
        ((RectTransform) winlose_My.transform).DOAnchorPosY(-320 + 840, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < arrValue.Length; i++)
        {
            arrValue[i].gameObject.SetActive(true);
            arrValue[i].transform.localScale = Vector3.zero;
            arrValue[i].transform.DOScale(Vector3.one, 0.3f).SetEase(ease);
            //arrValue[i].Initialize(9999, 9999, 9999);
            yield return new WaitForSeconds(0.15f);
        }
        btn_End.gameObject.SetActive(true);
        ((RectTransform) btn_End.transform).DOScale(Vector3.one, 0.3f).SetEase(ease);
    }
}
