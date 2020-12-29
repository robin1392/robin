using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardSlot : MonoBehaviour
{
    public GameObject obj_Trophy;

    [Space]
    public Image image_Guage;
    public Text text_Trophy;
    
    [Space]
    public Image[] arrImage_Icon;
    public Text[] arrText_Value;
    public GameObject[] arrObj_Lock;
    public GameObject[] arrObj_Check;
}
