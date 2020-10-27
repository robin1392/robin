using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultValue : MonoBehaviour
{
    public Text text_Trophy;
    public Text text_Gold;
    public Text text_Key;
    public Image[] arrImages;
    public Material mtl_Grayscale;

    public void Initialize(int trophy, int gold, int key)
    {
        text_Trophy.text = $"{(trophy < 0 ? string.Empty : "+")}{trophy}";
        text_Gold.text = $"{(gold < 0 ? string.Empty : "+")}{gold}";
        text_Key.text = $"{(key < 0 ? string.Empty : "+")}{key}";

        if (trophy <= 0)
        {
            text_Trophy.color = Color.gray;
            arrImages[1].material = mtl_Grayscale;
        }

        if (gold <= 0)
        {
            text_Gold.color = Color.gray;
            arrImages[2].material = mtl_Grayscale;
        }

        if (key <= 0)
        {
            text_Key.color = Color.gray;
            arrImages[3].material = mtl_Grayscale;
        }
        
        if (trophy == 0 && gold == 0 && key == 0)
        {
            arrImages[0].material = mtl_Grayscale;
        }
    }
}
