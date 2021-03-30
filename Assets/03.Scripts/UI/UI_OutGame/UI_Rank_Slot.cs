using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_Rank_Slot : MonoBehaviour
{
    public Image image_Rank;
    public Text text_Rank;
    public Text text_Class;
    public Text text_Trophy;
    public Text text_Name;
    public UI_WinLose winlose;

    public void Initialize(int rank, int trophy, string name, int nClass, int[] deck)
    {
        Debug.Log($"Rank slot initialize: rank[{rank}], trophy[{trophy}], name[{name}], class[{nClass}], deck[{string.Join(",", deck)}]");
        //image_Rank.sprite = arrSprite_Rank[Mathf.Clamp(rank - 1, 0, 3)];
        
        if (trophy >= 0)
        {
            text_Rank.text = rank.ToString();
            text_Trophy.text = trophy.ToString();
            text_Name.text = name;
            text_Class.text = $"{Global.g_class} {nClass.ToString()}";
        }
        else
        {
            text_Trophy.text = string.Empty;
            text_Name.text = string.Empty;
            text_Class.text = string.Empty;
        }
        
        winlose.Initialize(deck[5], deck, name, trophy);
    }
}
