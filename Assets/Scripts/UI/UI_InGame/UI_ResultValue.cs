using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultValue : MonoBehaviour
{
    public Text text_Trophy;
    public Text text_Gold;
    public Text text_Key;

    public void Initialize(int trophy, int gold, int key)
    {
        text_Trophy.text = trophy.ToString();
        text_Gold.text = gold.ToString();
        text_Key.text = key.ToString();
    }
}
