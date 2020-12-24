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

    public void Initialize(int rank, int trophy, string name, int nClass, int[] deck)
    {
        image_Rank.sprite = arrSprite_Rank[rank - 1];
        
        if (trophy >= 0)
        {
            text_Trophy.text = trophy.ToString();
            text_Name.text = name;
            text_Class.text = $"CLASS {nClass.ToString()}";
        }
        else
        {
            text_Trophy.text = string.Empty;
            text_Name.text = string.Empty;
            text_Class.text = string.Empty;
        }
        
        winlose.Initialize(deck, name, trophy);
    }
}
