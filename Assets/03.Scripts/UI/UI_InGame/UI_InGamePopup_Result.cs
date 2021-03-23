using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using RandomWarsProtocol;
using RandomWarsResource.Data;
using Service.Core;
using Template.User.RandomwarsUser.Common;
using Button = UnityEngine.UI.Button;

public class UI_InGamePopup_Result : MonoBehaviour
{
    [Header("Win Lose")]
    public UI_WinLose winlose_Other;
    public UI_WinLose winlose_My;

    [Space] 
    public RectTransform rts_WinMessage;
    public Text text_WinMessage;
    public RectTransform rts_CoopIcon;

    [Header("Result Value")] 
    public Text text_Win_Trophy;
    public Text text_Win_Gold;
    public Text text_Win_Key;
    public Text text_Lose_Trophy;
    public Text text_Lose_Gold;
    
    [Header("Button")]
    public Button btn_ShowValues;
    public Button btn_End;
    public Button btn_AD;
    public Image image_ADReward_Icon;
    public Text text_ADReward_Count;

    [Header("Coop")] 
    public RectTransform rts_ScrollView;
    public RectTransform rts_CoopRewardContent;
    public GameObject pref_CoopRewardSlot;
    private List<TDataItemList> listBox = new List<TDataItemList>();
    
    private bool isWin;

    enum REWARD_TYPE
    {
        TROPHY,
        GOLD,
        KEY,
        SEASON_TROPHY,
        RANK_TROPHY,
    }

    enum REWARD_CATEGORY
    {
        NORMAL,
        WINSTREAK,
        PERFECT,
    }

    private int[,] rewards = new int[3,5];
    private AdRewardInfo loseReward;
    
    public void Initialize(bool winLose, int winningStreak, ItemBaseInfo[] normalReward, ItemBaseInfo[] streakReward, ItemBaseInfo[] perfectReward, AdRewardInfo loseReward)
    {
        this.loseReward = loseReward;
        
        if (TutorialManager.isTutorial)
        {
            TutorialManager.stepCount++;
        }
        
        isWin = winLose;

        winlose_My.Initialize(isWin, (perfectReward != null && perfectReward.Length > 0), winningStreak, NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().playerInfo.Name, NetworkManager.Get().GetNetInfo().playerInfo.Trophy);
        winlose_Other.Initialize(!isWin, InGameManager.Get().playerController.targetPlayer.currentHealth > 20000, winningStreak, NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().otherInfo.Name, NetworkManager.Get().GetNetInfo().otherInfo.Trophy);
        btn_ShowValues.interactable = false;

        // 경쟁전일경우
        if (NetworkManager.Get().playType == Global.PLAY_TYPE.BATTLE)
        {
            int normalGold = 0;
            int normalTrophy = 0;
            int normalSeasonTrophy = 0;
            int normalRankTrophy = 0;
            int normalKey = 0;
            if (normalReward != null)
            {
                foreach (var reward in normalReward)
                {
                    switch ((EItemListKey) reward.ItemId)
                    {
                        case EItemListKey.gold:
                            //normalGold = reward.Value;
                            rewards[(int)REWARD_CATEGORY.NORMAL, (int)REWARD_TYPE.GOLD] = reward.Value;
                            break;
                        case EItemListKey.thropy:
                            //normalTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.NORMAL, (int)REWARD_TYPE.TROPHY] = reward.Value;
                            break;
                        case EItemListKey.seasonthropy:
                            //normalSeasonTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.NORMAL, (int)REWARD_TYPE.SEASON_TROPHY] = reward.Value;
                            break;
                        case EItemListKey.rankthropy:
                            //normalRankTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.NORMAL, (int)REWARD_TYPE.RANK_TROPHY] = reward.Value;
                            break;
                        case EItemListKey.key:
                            //normalKey = reward.Value;
                            rewards[(int)REWARD_CATEGORY.NORMAL, (int)REWARD_TYPE.KEY] = reward.Value;
                            break;
                    }
                }
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
                    switch ((EItemListKey) reward.ItemId)
                    {
                        case EItemListKey.gold:
                            //normalGold = reward.Value;
                            rewards[(int)REWARD_CATEGORY.WINSTREAK, (int)REWARD_TYPE.GOLD] = reward.Value;
                            break;
                        case EItemListKey.thropy:
                            //normalTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.WINSTREAK, (int)REWARD_TYPE.TROPHY] = reward.Value;
                            break;
                        case EItemListKey.seasonthropy:
                            //normalSeasonTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.WINSTREAK, (int)REWARD_TYPE.SEASON_TROPHY] = reward.Value;
                            break;
                        case EItemListKey.rankthropy:
                            //normalRankTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.WINSTREAK, (int)REWARD_TYPE.RANK_TROPHY] = reward.Value;
                            break;
                        case EItemListKey.key:
                            //normalKey = reward.Value;
                            rewards[(int)REWARD_CATEGORY.WINSTREAK, (int)REWARD_TYPE.KEY] = reward.Value;
                            break;
                    }
                }
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
                    switch ((EItemListKey) reward.ItemId)
                    {
                        case EItemListKey.gold:
                            //normalGold = reward.Value;
                            rewards[(int)REWARD_CATEGORY.PERFECT, (int)REWARD_TYPE.GOLD] = reward.Value;
                            break;
                        case EItemListKey.thropy:
                            //normalTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.PERFECT, (int)REWARD_TYPE.TROPHY] = reward.Value;
                            break;
                        case EItemListKey.seasonthropy:
                            //normalSeasonTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.PERFECT, (int)REWARD_TYPE.SEASON_TROPHY] = reward.Value;
                            break;
                        case EItemListKey.rankthropy:
                            //normalRankTrophy = reward.Value;
                            rewards[(int)REWARD_CATEGORY.PERFECT, (int)REWARD_TYPE.RANK_TROPHY] = reward.Value;
                            break;
                        case EItemListKey.key:
                            //normalKey = reward.Value;
                            rewards[(int)REWARD_CATEGORY.PERFECT, (int)REWARD_TYPE.KEY] = reward.Value;
                            break;
                    }
                }
            }

            int totalTrophy = rewards[0, 0] + rewards[1, 0] + rewards[2, 0];
            int totalGold = rewards[0, 1] + rewards[1, 1] + rewards[2, 1];
            int totalKey = rewards[0, 2] + rewards[1, 2] + rewards[2, 2];
            int totalSeasonTrophy = rewards[0, 3] + rewards[1, 3] + rewards[2, 3];
            int totalRankTrophy = rewards[0, 4] + rewards[1, 4] + rewards[2, 4];

            var userInfo = UserInfoManager.Get().GetUserInfo();
            userInfo.trophy += totalTrophy;
            userInfo.seasonTrophy += totalSeasonTrophy;
            userInfo.rankPoint += totalRankTrophy;
            userInfo.gold += totalGold;
            userInfo.key += totalKey;
        }
        else                    // 협동전일경우
        {
            var userInfo = UserInfoManager.Get().GetUserInfo();
            TDataItemList data;
            for (int i = 0; normalReward != null && i < normalReward.Length; i++)
            {
                if (TableManager.Get().ItemList.GetData(normalReward[i].ItemId, out data))
                {
                    listBox.Add(data);
                    if (userInfo.dicBox.ContainsKey(data.id)) userInfo.dicBox[data.id] += normalReward[i].Value;
                    else userInfo.dicBox.Add(data.id, normalReward[i].Value);
                }
            }
            for (int i = 0; streakReward != null && i < streakReward.Length; i++)
            {
                if (TableManager.Get().ItemList.GetData(streakReward[i].ItemId, out data))
                {
                    listBox.Add(data);
                    if (userInfo.dicBox.ContainsKey(data.id)) userInfo.dicBox[data.id] += streakReward[i].Value;
                    else userInfo.dicBox.Add(data.id, streakReward[i].Value);
                }
            }
            for (int i = 0; perfectReward != null && i < perfectReward.Length; i++)
            {
                if (TableManager.Get().ItemList.GetData(perfectReward[i].ItemId, out data))
                {
                    listBox.Add(data);
                    if (userInfo.dicBox.ContainsKey(data.id)) userInfo.dicBox[data.id] += perfectReward[i].Value;
                    else userInfo.dicBox.Add(data.id, perfectReward[i].Value);
                }
            }
        }

        Invoke("EnableShowValuesButton", 2f);
    }

    private void EnableShowValuesButton()
    {
        btn_ShowValues.interactable = true;
        StartCoroutine(AutoSkipCoroutine());
    }

    IEnumerator AutoSkipCoroutine()
    {
        yield return new WaitForSeconds(3f);
        
        Click_ShowResultValues();
    }
    
    public void Click_ShowResultValues()
    {
        StopAllCoroutines();
        btn_ShowValues.gameObject.SetActive(false);
        StartCoroutine(ShowResultValuesCoroutine());

        SoundManager.instance.Play(isWin ? Global.E_SOUND.SFX_WIN : Global.E_SOUND.SFX_LOSE);
    }

    IEnumerator ShowResultValuesCoroutine()
    {
        Ease ease = Ease.InBack;

        if (NetworkManager.Get().playType == Global.PLAY_TYPE.BATTLE)
        {
            ((RectTransform) winlose_Other.transform).DOAnchorPosY(520 + 1200, 0.5f).SetEase(ease);
            ((RectTransform) winlose_My.transform).DOAnchorPosY(-320 + 520, 0.5f).SetEase(ease).SetDelay(0.1f);
            yield return new WaitForSeconds(0.6f);
            
            if (isWin)
            {
                text_Win_Trophy.gameObject.SetActive(true);
                text_Win_Trophy.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                text_Win_Gold.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetDelay(0.1f).OnStart(() =>
                {
                    text_Win_Gold.gameObject.SetActive(true);
                });
                text_Win_Key.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetDelay(0.2f).OnStart(() =>
                {
                    text_Win_Key.gameObject.SetActive(true);
                });
            }
            else
            {
                text_Lose_Trophy.gameObject.SetActive(true);
                text_Lose_Trophy.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                text_Lose_Gold.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetDelay(0.1f).OnStart(() =>
                {
                    text_Lose_Gold.gameObject.SetActive(true);
                });
            }

            yield return new WaitForSeconds(0.4f);
            Text text_MyRankPoint = winlose_My.text_Trophy;

            if (rewards[0, 0] != 0 || rewards[0, 1] > 0 || rewards[0, 2] > 0 || rewards[0, 4] != 0)
            {
                if (isWin) SetRewardMessage("Normal rewards !");
                yield return new WaitForSeconds(0.3f);
                if (rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.TROPHY] != 0)
                {
                    if (isWin)
                        StartCoroutine(TextCoroutine(text_Win_Trophy,
                            rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.TROPHY], true));
                    else
                        StartCoroutine(TextCoroutine(text_Lose_Trophy,
                            rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.TROPHY], true));
                }

                if (rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.GOLD] > 0)
                {
                    if (isWin)
                        StartCoroutine(TextCoroutine(text_Win_Gold,
                            rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.GOLD]));
                    else
                        StartCoroutine(TextCoroutine(text_Lose_Gold,
                            rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.GOLD]));
                }

                if (rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.KEY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Key,
                        rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.KEY]));
                }
                if (rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.RANK_TROPHY] != 0)
                {
                    StartCoroutine(TextCoroutine(text_MyRankPoint,
                        rewards[(int) REWARD_CATEGORY.NORMAL, (int) REWARD_TYPE.RANK_TROPHY]));
                }
                yield return new WaitForSeconds(1f);
                yield return new WaitForSeconds(0.3f);
            }

            if (rewards[1, 0] > 0 || rewards[1, 1] > 0 || rewards[1, 2] > 0 || rewards[1, 4] > 0)
            {
                SetRewardMessage("Winstreak rewards !");
                yield return new WaitForSeconds(0.3f);
                if (rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.TROPHY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Trophy,
                        rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.TROPHY], true));
                }
                if (rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.GOLD] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Gold,
                        rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.GOLD]));
                }
                if (rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.KEY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Key,
                        rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.KEY]));
                }
                if (rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.RANK_TROPHY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_MyRankPoint,
                        rewards[(int) REWARD_CATEGORY.WINSTREAK, (int) REWARD_TYPE.RANK_TROPHY]));
                }
                yield return new WaitForSeconds(1f);
                yield return new WaitForSeconds(0.3f);
            }

            if (rewards[2, 0] > 0 || rewards[2, 1] > 0 || rewards[2, 2] > 0 || rewards[2, 4] > 0)
            {
                SetRewardMessage("Perfect rewards !");
                yield return new WaitForSeconds(0.3f);
                if (rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.TROPHY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Trophy,
                        rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.TROPHY], true));
                }
                if (rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.GOLD] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Gold,
                        rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.GOLD]));
                }
                if (rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.KEY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_Win_Key,
                        rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.KEY]));
                }
                if (rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.RANK_TROPHY] > 0)
                {
                    StartCoroutine(TextCoroutine(text_MyRankPoint,
                        rewards[(int) REWARD_CATEGORY.PERFECT, (int) REWARD_TYPE.RANK_TROPHY]));
                }
                yield return new WaitForSeconds(1f);
                yield return new WaitForSeconds(0.3f);
            }
        }
        else            // 협동전일경우
        {
            bool isWait = true;
            //((RectTransform) winlose_Other.transform).DOAnchorPosY(520 + 200, 0.5f).SetEase(ease);
            ((RectTransform) winlose_My.transform).DOAnchorPosY(-320 - 200, 0.5f).SetEase(ease).SetDelay(0.1f);
            ease = Ease.OutBack;
            rts_CoopIcon.gameObject.SetActive(true);
            rts_CoopIcon.localScale = Vector3.zero;
            rts_CoopIcon.DOScale(1f, 0.3f).SetEase(ease).OnComplete(() =>
            {
                rts_CoopIcon.DOScale(1.1f, 0.3f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    rts_CoopIcon.DOScale(0f, 0.3f).SetEase(ease).OnComplete(() =>
                    {
                        rts_CoopIcon.gameObject.SetActive(false);
                        rts_ScrollView.gameObject.SetActive(true);
                        rts_ScrollView.localScale = Vector3.zero;
                        rts_ScrollView.DOScale(1f, 0.5f).SetEase(ease).OnComplete(() =>
                        {
                            isWait = false;
                        });
                    });
                });
            });

            while (isWait) { yield return null; }
            
            float width = GetComponentInParent<CanvasScaler>().referenceResolution.x;
            for (int i = 0; i < listBox.Count; i++)
            {
                var obj = Instantiate(pref_CoopRewardSlot, Vector3.zero, Quaternion.identity, rts_CoopRewardContent);
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(1f, 0.1f).SetEase(ease);
                obj.GetComponentInChildren<UnityEngine.UI.Image>().sprite = FileHelper.GetIcon(listBox[i].itemIcon);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rts_CoopRewardContent);
                if (rts_CoopRewardContent.sizeDelta.x > width)
                {
                    rts_CoopRewardContent.DOAnchorPosX(-(rts_CoopRewardContent.sizeDelta.x - width), 0.5f);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        // for (int i = 0; i < arrValue.Length; i++)
        // {
        //     arrValue[i].gameObject.SetActive(true);
        //     arrValue[i].transform.localScale = Vector3.zero;
        //     arrValue[i].transform.DOScale(Vector3.one, 0.3f).SetEase(ease);
        //     //arrValue[i].Initialize(9999, 9999, 9999);
        //     yield return new WaitForSeconds(0.15f);
        // }

        yield return new WaitForSeconds(0.5f);
        btn_End.gameObject.SetActive(true);
        ((RectTransform) btn_End.transform).DOScale(Vector3.one, 0.3f).SetEase(ease).OnStart(() =>
        {
            btn_End.transform.localScale = Vector3.zero;
        });
        
        if (loseReward != null && string.IsNullOrEmpty(loseReward.RewardId) == false)
        {
            // TDataItemList data;
            // TableManager.Get().ItemList.GetData(loseReward.ItemId, out data);
            {
                btn_AD.gameObject.SetActive(true);
                //image_ADReward_Icon.sprite = FileHelper.GetIcon(data.itemIcon);
                text_ADReward_Count.text = $"x{loseReward.Value}";
                btn_AD.onClick.AddListener(() =>
                {
#if UNITY_EDITOR
                    UI_InGamePopup.Get().obj_Indicator.SetActive(true);
                    NetworkManager.session.UserTemplate.UserAdRewardReq(NetworkManager.session.HttpClient,
                        loseReward.RewardId, ADRewardCallback);
#else
                    InGameManager.Get().LeaveRoomWithCallback(LeaveRoomCallback);
#endif
                });
            }
            
            ((RectTransform) btn_AD.transform).DOScale(Vector3.one, 0.3f).SetEase(ease).OnStart(() =>
            {
                btn_AD.transform.localScale = Vector3.zero;
            });
        }
    }

    private void LeaveRoomCallback()
    {
        MopubCommunicator.Instance.showVideo(ADCallback);
    }

    private void ADCallback(bool b)
    {
        if (b)
        {
            UI_InGamePopup.Get().obj_Indicator.SetActive(true);
            NetworkManager.session.UserTemplate.UserAdRewardReq(NetworkManager.session.HttpClient,
                loseReward.RewardId, ADRewardCallback);
        }
        else
        {
            GameStateManager.Get().MoveMainScene();
        }
    }

    private bool ADRewardCallback(ERandomwarsUserErrorCode errorCode, ItemBaseInfo[] arrayRewardInfo,
        QuestData[] arrayQuestData)
    {
        UI_InGamePopup.Get().obj_Indicator.SetActive(false);
        if (errorCode == ERandomwarsUserErrorCode.Success)
        {
            foreach (var reward in arrayRewardInfo)
            {
                UI_Main.listADReward.Add(reward);
            }
            UI_Popup_Quest.QuestUpdate(arrayQuestData);
            GameStateManager.Get().MoveMainScene();
            return true;
        }
        
        GameStateManager.Get().MoveMainScene();
        return false;
    }

    private void SetRewardMessage(string message)
    {
        text_WinMessage.text = message;
        rts_WinMessage.DOKill();
        rts_WinMessage.anchoredPosition = new Vector2(1300, -270);
        rts_WinMessage.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            rts_WinMessage.DOAnchorPosX(-1300, 0.3f).SetEase(Ease.OutBack).SetDelay(1f);
        });
    }

    IEnumerator TextCoroutine(Text text, int addValue, bool addPlusChar = false)
    {
        float t = 0;
        int beforeValue = Int32.Parse(text.text.Replace("+", string.Empty));
        int endValue = beforeValue + addValue;
        while (t < 1f)
        {
            t += Time.deltaTime;
            if (addPlusChar)
            {
                text.text = $"{(isWin ? "+" : string.Empty)}{Mathf.CeilToInt(Mathf.Lerp(beforeValue, endValue, t))}";
            }
            else
            {
                text.text = $"{Mathf.CeilToInt(Mathf.Lerp(beforeValue, endValue, t))}";
            }
            yield return null;
        }

        if (addPlusChar) text.text = $"{(isWin ? "+" : string.Empty)}{endValue.ToString()}";
        else text.text = endValue.ToString();
    }
}
