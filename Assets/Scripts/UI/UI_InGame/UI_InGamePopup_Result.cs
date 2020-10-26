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
        isWin = InGameManager.Get().playerController.isAlive;
        
        winlose_My.Initialize(isWin, InGameManager.Get().playerController.currentHealth > 20000, NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().playerInfo.Name, 0);
        winlose_Other.Initialize(!isWin, InGameManager.Get().playerController.targetPlayer.currentHealth > 20000, NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().otherInfo.Name, 0);
        btn_ShowValues.interactable = false;
        Invoke("EnableShowValuesButton", 2f);
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
        ((RectTransform) winlose_Other.transform).DOScale(Vector3.zero, 0.3f);
        ((RectTransform) text_VS.transform).DOScale(Vector3.zero, 0.3f);
        ((RectTransform) winlose_My.transform).DOAnchorPosY(-320 + 840, 0.5f);
        yield return new WaitForSeconds(0.5f);
        btn_End.gameObject.SetActive(true);
        ((RectTransform) btn_End.transform).DOScale(Vector3.one, 0.3f);
    }
}
