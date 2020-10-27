using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RandomWarsProtocol;

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
// #if UNITY_EDITOR
//         isWin = true;
//         
//         winlose_My.Initialize(isWin, true, new int[] { 1000, 1001, 1002, 1003, 1004 }, "ED", 1234);
//         winlose_Other.Initialize(!isWin, false, new int[] { 2000, 2001, 2002, 2003, 2004 }, "COM", 4321);
//         btn_ShowValues.interactable = false;
//         Invoke("EnableShowValuesButton", 2f);
// #else
        isWin = winLose;

        winlose_My.Initialize(isWin, winningStreak, NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().playerInfo.Name, NetworkManager.Get().GetNetInfo().playerInfo.Trophy);
        winlose_Other.Initialize(!isWin, winningStreak, NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().otherInfo.Name, NetworkManager.Get().GetNetInfo().otherInfo.Trophy);
        btn_ShowValues.interactable = false;

        List<MsgReward> list = new List<MsgReward>(normalReward);
        arrValue[0].text_Trophy.text = list.Find(msg => msg.RewardType == ERewardType.Trophy)?.Value.ToString();
        arrValue[0].text_Gold.text = list.Find(msg => msg.RewardType == ERewardType.Gold)?.Value.ToString();
        arrValue[0].text_Key.text = list.Find(msg => msg.RewardType == ERewardType.Key)?.Value.ToString();
        
        list = new List<MsgReward>(streakReward);
        arrValue[1].text_Trophy.text = list.Find(msg => msg.RewardType == ERewardType.Trophy)?.Value.ToString();
        arrValue[1].text_Gold.text = list.Find(msg => msg.RewardType == ERewardType.Gold)?.Value.ToString();
        arrValue[1].text_Key.text = list.Find(msg => msg.RewardType == ERewardType.Key)?.Value.ToString();
        text_WinningStreak.text = winningStreak.ToString();
        
        list = new List<MsgReward>(perfectReward);
        arrValue[2].text_Trophy.text = list.Find(msg => msg.RewardType == ERewardType.Trophy)?.Value.ToString();
        arrValue[2].text_Gold.text = list.Find(msg => msg.RewardType == ERewardType.Gold)?.Value.ToString();
        arrValue[2].text_Key.text = list.Find(msg => msg.RewardType == ERewardType.Key)?.Value.ToString();
        
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
    }

    IEnumerator ShowResultValuesCoroutine()
    {
        Ease ease = Ease.OutQuint;
        ((RectTransform) winlose_Other.transform).DOScale(Vector3.zero, 0.3f).SetEase(ease);
        ((RectTransform) text_VS.transform).DOScale(Vector3.zero, 0.3f).SetEase(ease);
        ((RectTransform) winlose_My.transform).DOAnchorPosY(-320 + 840, 0.5f).SetEase(ease);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < arrValue.Length; i++)
        {
            arrValue[i].gameObject.SetActive(true);
            arrValue[i].transform.localScale = Vector3.zero;
            arrValue[i].transform.DOScale(Vector3.one, 0.3f).SetEase(ease);
            arrValue[i].Initialize(9999, 9999, 9999);
            yield return new WaitForSeconds(0.15f);
        }
        btn_End.gameObject.SetActive(true);
        ((RectTransform) btn_End.transform).DOScale(Vector3.one, 0.3f).SetEase(ease);
    }
}
