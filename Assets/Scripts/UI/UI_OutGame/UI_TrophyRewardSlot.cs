using System.Collections;
using System.Collections.Generic;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_TrophyRewardSlot : MonoBehaviour
{
    public GameObject obj_Trophy;

    [Space]
    public Image image_Guage;
    public Image image_Guage_BG;
    public Text text_Trophy;

    [Space]
    public Button[] arrButton;
    public Image[] arrImage_Icon;
    public Text[] arrText_Value;
    public GameObject[] arrObj_Lock;
    public GameObject[] arrObj_Check;

    public void Initialize(int row, int myTrophy, int getVipRow, int getNormalRow)
    {
        float minTrophy = 0;
        float maxTrophy = 0;
        
        if (row == 0)
        {
            text_Trophy.text = $"0";
            var v = image_Guage.rectTransform.sizeDelta;
            v.x *= 0.5f;
            image_Guage.rectTransform.sizeDelta = v;
            image_Guage_BG.rectTransform.sizeDelta = v;
            v = image_Guage.rectTransform.anchoredPosition;
            v.y = -200;
            image_Guage.rectTransform.anchoredPosition = v;
            image_Guage_BG.rectTransform.anchoredPosition = v;
            
            arrButton[0].gameObject.SetActive(false);
            arrButton[1].gameObject.SetActive(false);
            
            var dataNormal = new TDataClassReward();
            if (TableManager.Get().ClassReward.GetData(row + 1, out dataNormal))
            {
                maxTrophy = Mathf.Lerp(0f, dataNormal.rankPoint, 0.5f);
            }
        }
        else
        {
            var dataPass = new TDataClassReward();
            var dataNormal = new TDataClassReward();
            TableManager.Get().ClassReward.GetData(row, out dataNormal);
            TableManager.Get().ClassReward.GetData(row + 1000, out dataPass);

            text_Trophy.text = dataPass.rankPoint.ToString();
            arrText_Value[0].text = $"{dataPass.ItemId}\nx{dataPass.ItemValue}";
            arrText_Value[1].text = $"{dataNormal.ItemId}\nx{dataNormal.ItemValue}";
            
            // set min, max
            if (TableManager.Get().ClassReward.GetData(row - 1, out dataNormal))
            {
                minTrophy = Mathf.Lerp(dataNormal.rankPoint, dataPass.rankPoint, 0.5f);
            }
            
            if (TableManager.Get().ClassReward.GetData(row + 1, out dataNormal) == false)
            {
                var v = image_Guage.rectTransform.sizeDelta;
                v.x *= 0.5f;
                image_Guage.rectTransform.sizeDelta = v;
                image_Guage_BG.rectTransform.sizeDelta = v;

                maxTrophy = dataPass.rankPoint;
            }
            else
            {
                maxTrophy = Mathf.Lerp(dataPass.rankPoint, dataNormal.rankPoint, 0.5f);
            }
            
            // set buttons
            if (myTrophy < dataPass.rankPoint)    // 트로피 부족
            {
                arrObj_Lock[0].SetActive(true);
                arrObj_Lock[1].SetActive(true);
                arrObj_Check[0].SetActive(false);
                arrObj_Check[1].SetActive(false);
                arrButton[0].interactable = false;
                arrButton[1].interactable = false;
            }
            else
            {
                bool getNormal = getNormalRow >= row;
                bool getPass = getVipRow >= row + 1000;
                
                arrObj_Lock[0].SetActive(false);
                arrObj_Lock[1].SetActive(false);
                arrObj_Check[0].SetActive(getPass);
                arrObj_Check[1].SetActive(getNormal);
                arrButton[0].interactable = !getPass;
                arrButton[1].interactable = !getNormal;
            }
        }

        image_Guage.fillAmount = (myTrophy - minTrophy) / (float)maxTrophy;
    }
}
