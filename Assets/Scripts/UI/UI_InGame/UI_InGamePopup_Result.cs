using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_InGamePopup_Result : MonoBehaviour
{
    [Header("Win Lose")]
    public UI_WinLose winlose_Other;
    public UI_WinLose winlose_My;
    public Text text_VS;
    public UI_ResultValue[] arrValue;
    
    [Space]
    public Button btn_ShowValues;
    public Button btn_End;
    
    private bool isWin;
    
    public void Initialize()
    {
        #if UNITY_EDITOR
        isWin = true;
        
        winlose_My.Initialize(isWin, true, new int[] { 1000, 1001, 1002, 1003, 1004 }, "ED", 1234);
        winlose_Other.Initialize(!isWin, false, new int[] { 2000, 2001, 2002, 2003, 2004 }, "COM", 4321);
        btn_ShowValues.interactable = false;
        Invoke("EnableShowValuesButton", 2f);
#else
        isWin = InGameManager.Get().playerController.isAlive;

        winlose_My.Initialize(isWin, InGameManager.Get().playerController.currentHealth > 20000, NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().playerInfo.Name, 0);
        winlose_Other.Initialize(!isWin, InGameManager.Get().playerController.targetPlayer.currentHealth > 20000, NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().otherInfo.Name, 0);
        btn_ShowValues.interactable = false;
        Invoke("EnableShowValuesButton", 2f);
#endif

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
