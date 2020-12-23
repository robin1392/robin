using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

public class TutorialManager : Singleton<TutorialManager>
{
    public bool isTutorial;
    [Space] public Transform ts_BattleButton;
    private int stepCount = 0;
    
    private void Start()
    {
        if (ObscuredPrefs.GetBool("Tutorial", false) == true)
        {
            gameObject.SetActive(false);
            return;
        }

        isTutorial = true;
        StartCoroutine(TutorialCoroutine());
    }

    IEnumerator TutorialCoroutine()
    {
        yield return new WaitWhile(() => stepCount < 1);
        Step();
        
        yield return new WaitWhile(() => stepCount < 1);
        Step();
    }
    
    public void Click_NextStep()
    {
        stepCount++;
    }

    private void Step()
    {
        transform.GetChild(stepCount).gameObject.SetActive(false);
        transform.GetChild(stepCount + 1).gameObject.SetActive(true);
        
        switch (stepCount)
        {
            case 1:
                ts_BattleButton.parent = transform.GetChild(stepCount + 1);
                break;
        }
    }
}
