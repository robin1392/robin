using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Rank_Slot : MonoBehaviour
{
    public Image image_Rank;
    public Text text_Class;
    public Text text_Trophy;
    public Text text_Name;
    public UI_WinLose winlose;

    public Sprite[] arrSprite_Rank;

    public void Initialize(int rank, int trophy, string name)
    {
        if (trophy >= 0)
        {
            text_Trophy.text = trophy.ToString();
            text_Name.text = name;
        }
        else
        {
            text_Trophy.text = string.Empty;
            text_Name.text = string.Empty;
        }
    }
}
