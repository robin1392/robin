using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;

public class UI_SPUpgradeButton : UI_GetDiceButton
{
    protected override void EditSpCallback(int sp)
    {
        button.interactable = sp >= (PlayerController.Get().spUpgradeLevel + 1) * 500;
        SetImageAndText();
    }
}
