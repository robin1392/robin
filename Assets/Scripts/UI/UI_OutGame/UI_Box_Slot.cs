using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Box_Slot : MonoBehaviour
{
    [Header("Link")]
    public Image image_BG;
    public Image image_Icon;
    public Text text_Name;
    public Image image_CostIcon;
    public Text text_Cost;

    [Header("Resources")]
    public Sprite[] arrSprite_BG;
    public Sprite[] arrSprite_BoxIcon;

    public void Initialize()
    {
        
    }
}
