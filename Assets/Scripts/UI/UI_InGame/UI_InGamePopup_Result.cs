using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGamePopup_Result : MonoBehaviour
{
    [Header("Win Lose")]
    public UI_WinLose winlose_Other;
    public UI_WinLose winlose_My;
    
    [Space]
    public Button btn_ShowValues;
    
    private bool isWin;
    
    public void Initialize()
    {
        isWin = InGameManager.Get().playerController.isAlive;
        
        winlose_My.Initialize(isWin, NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().playerInfo.Name, 0);
        winlose_Other.Initialize(!isWin, NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray, NetworkManager.Get().GetNetInfo().otherInfo.Name, 0);
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
    }
}
