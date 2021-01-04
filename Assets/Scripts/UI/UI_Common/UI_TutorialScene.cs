using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TutorialScene : MonoBehaviour
{
    private Button btn;
    
    private void OnEnable()
    {
        if (btn == null) btn = GetComponent<Button>();
        
        if (btn.interactable)
        {
            btn.interactable = false;
            StartCoroutine(ButtonCoroutine());
        }
    }

    IEnumerator ButtonCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        btn.interactable = true;
    }
}
