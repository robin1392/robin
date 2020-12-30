using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;
using Debug = ED.Debug;
using DG.Tweening;
using ED;
using RandomWarsProtocol;

public class TutorialManager : MonoBehaviour
{
    public static bool isTutorial;
    public Image image_NextStep;
    [Space] 
    public Transform ts_BattleButton;
    public Transform ts_GetDiceButton;
    public Transform ts_DiceField;

    private static int stepCount = 0;
    private static int nextStepCount = 1;
    private Transform ts_OldParent;

    public static int getDiceCount
    {
        get;
        private set;
    }
    
    private void Start()
    {
        if (ObscuredPrefs.GetBool("Tutorial", false) == true)
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
        while (true)
        {
            Step();
        
            yield return new WaitWhile(() => stepCount < nextStepCount);
            nextStepCount++;
        }
        // Step();
        //
        // yield return new WaitWhile(() => stepCount < 1);
        // Step();
        //
        // yield return new WaitWhile(() => stepCount < 2);
        // Step();
    }
    
    public void Click_NextStep()
    {
        Time.timeScale = 1f;
        Click_NextStepCallback();
        stepCount++;
    }

    private void Click_NextStepCallback()
    {
        switch (stepCount)
        {
            case 6:
                InGameManager.Get().playerController.currentHealth = InGameManager.Get().playerController.maxHealth * 0.666f;
                InGameManager.Get().playerController.RefreshHealthBar();

                MsgMonster msg = new MsgMonster();
                msg.Hp = Int32.MaxValue;
                msg.Id = 1;
                msg.Power = 30000;
                msg.AttackSpeed = 100;
                msg.DataId = 20001;
                msg.MoveSpeed = 100;
                msg.Effect = 30000;
                InGameManager.Get().playerController.SpawnMonster(msg);
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
        yield return new WaitForSeconds(delay);
        
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
        yield return new WaitForSeconds(time);
        
        Step();
    }

    private void GetDice()
    {
        getDiceCount++;
        if (stepCount == 4 && getDiceCount >= 5)
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
        stepCount++;
    }

    private void Step()
    {
        Debug.Log("Tutorial step : " + stepCount);

        image_NextStep.DOFade(0.78f, 0).SetUpdate(true);
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == stepCount + 1);
        }
        
        switch (stepCount)
        {
            case 0:
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = true;
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
                    Time.timeScale = 0f;
                    image_NextStep.DOFade(0f, 0f);
                    transform.GetChild(stepCount + 1).GetChild(0).gameObject.SetActive(true);
                    Debug.Log("Ingame tutorial");
                }
                break;
            case 3:
                //image_NextStep.DOFade(0.78f, 0f).SetUpdate(true);
                Time.timeScale = 0f;
                break;
            case 4: // 주사위 소환 버튼
                Time.timeScale = 0f;
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_GetDiceButton.parent;
                ts_GetDiceButton.parent = transform.GetChild(stepCount + 1);
                ts_GetDiceButton.GetComponent<Button>().onClick.AddListener(GetDice);
                break;
            case 5:
                Time.timeScale = 0f;
                break;
            case 6:
                Time.timeScale = 0f;
                break;
            case 7:
                Time.timeScale = 0f;
                InGameManager.Get().playerController.TutorialAddSP();
                break;
            case 8:    // 두번째 주사위 소환
                Time.timeScale = 0f;
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_GetDiceButton.parent;
                ts_GetDiceButton.parent = transform.GetChild(stepCount + 1);
                ts_GetDiceButton.GetComponent<Button>().onClick.AddListener(GetDice);
                break;
            case 9:
                Time.timeScale = 0f;
                transform.GetChild(stepCount + 1).GetComponent<Button>().interactable = false;
                ts_OldParent = ts_DiceField.parent;
                ts_DiceField.parent = transform.GetChild(stepCount + 1);
                break;
            case 10:
                ts_DiceField.parent = ts_OldParent;
                break;
            default:
                Time.timeScale = 1f;
                break;
        }
    }
}
