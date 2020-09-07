using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;

public class UI_SPUpgradeButton : UI_GetDiceButton
{
    protected override void EditSpCallback(int sp)
    {
        if (PlayerController.Get().spUpgradeLevel < 5)
        {
            button.interactable = sp >= (PlayerController.Get().spUpgradeLevel + 1) * 100;
        }
        else
        {
            button.interactable = false;
        }
        
        SetImageAndText();
    }
}
