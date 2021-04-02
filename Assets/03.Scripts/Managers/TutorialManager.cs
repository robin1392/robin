using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = ED.Debug;
using DG.Tweening;
using ED;
using MirageTest.Scripts;

public class TutorialManager : MonoBehaviour
{
    public static bool isTutorial;
    public Image image_NextStep;
    [Space] 
    public Transform ts_BattleButton;
    public Transform ts_GetDiceButton;
    public Transform ts_DiceField;
    public Transform ts_UpgradeButton;

    public static int stepCount = 2;
    private static int nextStepCount = 3;
    private Transform ts_OldParent;

    public static int getDiceCount
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (true)//UserInfoManager.Get().GetUserInfo().isEndTutorial)
        {
            isTutorial = false;
            gameObject.SetActive(false);
            return;
        }

        isTutorial = true;
        //Step();
        StartCoroutine(TutorialCoroutine());
    }

    IEnumerator TutorialCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            Step();
        
            yield return new WaitWhile(() => stepCount < nextStepCount);
            nextStepCount++;
        }
    }
    
    public void Click_NextStep()
    {
        Time.timeScale = 1f;
        Click_NextStepCallback();
        stepCount++;
        Debug.Log($"Click_NextStep {stepCount} ");
    }

    private void Click_NextStepCallback()
    {
        switch (stepCount)
        {
            case 6:
                var localPlayerState = RWNetworkClient.Get().GetLocalPlayerState();
                var localPlayerOwnerTag = localPlayerState.ownerTag;
                var server = RWNetworkServer.Get();
                var localPlayerTower = server.Towers.Find(t => t.ownerTag == localPlayerOwnerTag);
                localPlayerTower.currentHealth = localPlayerTower.maxHealth * 0.666f;

                server.CreateActorWithGuardianId(
                    5001,
                    server.Towers[0].ownerTag,
                    server.Towers[0].team,
                    server.Towers[0].transform.position);
                // guadian.maxHealth = int.MaxValue;
                // guadian.currentHealth = int.MaxValue;
                // guadian.power = 30000;
                // guadian.attackSpeed = 100;
                // guadian.moveSpeed = 100;
                // guadian.effect = 30000;
                // server.serverGameLogic.ServerObjectManager.Spawn(guadian.NetIdentity);
                break;
        }
    }

    public void Click_EndCurrentStep()
    {
        Time.timeScale = 1f;
        image_NextStep.DOFade(0f, 0f);
        transform.GetChild(stepCount + 1).gameObject.SetActive(false);
    }

    public void Click_NextStepDelay(float delay)
    {
        Click_EndCurrentStep();
        StartCoroutine(Click_NextStepDelayCoroutine(delay));
    }

    IEnumerator Click_NextStepDelayCoroutine(float delay)
    {
        Debug.Log("Click_NextStepDelayCoroutine ");
        yield return new WaitForSecondsRealtime(delay);
        
        Click_NextStep();
    }

    private void UpdateTime(float time)
    {
        transform.GetChild(stepCount + 1).gameObject.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(UpdateTimeCoroutine(time));
    }

    IEnumerator UpdateTimeCoroutine(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        
        Step();
    }

    private void GetDice()
    {
        getDiceCount++;
        if (stepCount == 4 && getDiceCount > 5)
        {
            ts_GetDiceButton.parent = ts_OldParent;
            ts_GetDiceButton.GetComponent<Button>().onClick.RemoveListener(GetDice);
            Click_NextStepDelay(18f);
        }
        else if (stepCount == 8 && getDiceCount >= 15)
        {
            ts_GetDiceButton.parent = ts_OldParent;
            ts_GetDiceButton.GetComponent<Button>().onClick.RemoveListener(GetDice);
            Click_NextStep();
        }
    }

    public static void MergeComplete()
    {
        if (stepCount < 11) stepCount++;
    }

    private void Step()
    {
        Debug.Log("Tutorial step : " + stepCount);

        image_NextStep.DOFade(0.78f, 0).SetUpdate(true);
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == stepCount + 1);
        }
        
        var server = FindObjectOfType<RWNetworkServer>();
        
        switch (stepCount)
        {
            case 0:
                //transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = true;
                break;
            case 1:
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_BattleButton.parent;
                ts_BattleButton.parent = transform.GetChild(stepCount + 1);
                ts_BattleButton.GetComponent<Button>().onClick.AddListener(Click_NextStep);
                break;
            case 2:
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Debug.Log("Outgame tutorial end");
                    image_NextStep.DOFade(0f, 0f);
                    ts_BattleButton.GetComponent<Button>().onClick.RemoveListener(Click_NextStep);
                    var images = ts_BattleButton.GetComponentsInChildren<Image>();
                    foreach (var image in images)
                    {
                        image.DOFade(0f, 0.25f);
                    }
                    var texts = ts_BattleButton.GetComponentsInChildren<Text>();
                    foreach (var text in texts)
                    {
                        text.DOFade(0f, 0.25f);
                    }
                    ts_BattleButton.parent = ts_OldParent;
                }
                else
                {
                    Time.timeScale = 0.0f;
                    image_NextStep.DOFade(0f, 0).SetUpdate(true);
                    transform.GetChild(stepCount + 1).GetChild(0).gameObject.SetActive(true);
                    Debug.Log("Ingame tutorial");
                }
                break;
            case 3:
                Time.timeScale = 0.0f;
                server.serverGameLogic._gameMode.PlayerState1.sp += 100;
                server.serverGameLogic._gameMode.PlayerState2.sp += 1000;

                server.serverGameLogic._gameMode.PlayerState2.GetDice(3, 1);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(1, 2);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(3, 3);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(2, 5);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(4, 7);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(2, 9);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(0, 11);
                server.serverGameLogic._gameMode.PlayerState2.GetDice(0, 13);
                break;
            case 4: // 주사위 소환 버튼
                Time.timeScale = 0.0f;
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_GetDiceButton.parent;
                ts_GetDiceButton.parent = transform.GetChild(stepCount + 1);
                ts_GetDiceButton.GetComponent<Button>().onClick.AddListener(GetDice);
                break;
            case 5:
                Time.timeScale = 0.0f;
                break;
            case 6:
                Time.timeScale = 0.0f;
                break;
            case 7:
                Time.timeScale = 0.0f;
                server.serverGameLogic._gameMode.PlayerState1.sp += 5000;
                break;
            case 8:    // 두번째 주사위 소환
                Time.timeScale = 0.0f;
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_GetDiceButton.parent;
                ts_GetDiceButton.parent = transform.GetChild(stepCount + 1);
                ts_GetDiceButton.GetComponent<Button>().onClick.AddListener(GetDice);
                break;
            case 9:
                Time.timeScale = 0.0f;
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_DiceField.parent;
                ts_DiceField.parent = transform.GetChild(stepCount + 1);
                break;
            case 10:
                Time.timeScale = 1f;
                image_NextStep.DOFade(0, 0).SetUpdate(true);
                image_NextStep.raycastTarget = true;
                ts_DiceField.parent = ts_OldParent;
                Click_NextStepDelay(3f);
                break;
            case 11:
                UI_DiceField.Get().BroadcastMessage("AttachIcon");
                image_NextStep.raycastTarget = true;
                Time.timeScale = 0.0f;
                
                ts_OldParent = ts_UpgradeButton.parent;
                ts_UpgradeButton.parent = transform.GetChild(stepCount + 1);
                ts_UpgradeButton.GetComponent<Button>().onClick.AddListener(Click_NextStep);
                break;
            case 12:
                Time.timeScale = 1f;
                image_NextStep.DOFade(0, 0).SetUpdate(true);
                image_NextStep.raycastTarget = false;
                ts_UpgradeButton.GetComponent<Button>().onClick.RemoveListener(Click_NextStep);
                ts_UpgradeButton.parent = ts_OldParent;
                break;
            case 13:
                Time.timeScale = 1f;
                image_NextStep.raycastTarget = false;
                image_NextStep.DOFade(0, 0).SetUpdate(true);
                break;
            case 14:
                Time.timeScale = 1f;
                break;
            default:
                image_NextStep.DOFade(0, 0).SetUpdate(true);
                image_NextStep.raycastTarget = false;
                Time.timeScale = 1f;
                NetworkManager.session.UserTemplate.UserTutorialEndReq(NetworkManager.session.HttpClient, OnEndTutorial);
                isTutorial = false;
                //NetworkManager.Get().EndTutorialReq(UserInfoManager.Get().GetUserInfo().userID);
                break;
        }
    }

    public void Skip()
    {
        Debug.Log("Tutorial Skip !!");
        StopAllCoroutines();
        stepCount = 15;
        Step();
    }

    bool OnEndTutorial(ERandomwarsUserErrorCode errorCode, bool endTutorial)
    {
        return true;
    }
}
