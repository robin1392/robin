using System.Collections;
using System.Collections.Generic;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultValue : MonoBehaviour
{
    public Text text_Trophy;
    public Text text_Gold;
    public Text text_Key;
    public Image[] arrImages;
    public Material mtl_Grayscale;

    public void Initialize(int trophy, int seasonTrophy, int rankTrophy, int gold, int key)
    {
        // text_Trophy.text = $"{(trophy < 0 ? string.Empty : "+")}{trophy}";
        // text_Gold.text = $"{(gold < 0 ? string.Empty : "+")}{gold}";
        // text_Key.text = $"{(key < 0 ? string.Empty : "+")}{key}";
        //
        // if (trophy <= 0)
        // {
        //     text_Trophy.color = trophy < 0 ? ParadoxNotion.ColorUtils.HexToColor("DF362D") : Color.gray;
        //     arrImages[1].material = trophy == 0 ? mtl_Grayscale : null;
        // }
        //
        // if (gold <= 0)
        // {
        //     text_Gold.color = Color.gray;
        //     arrImages[2].material = mtl_Grayscale;
        // }
        //
        // if (key <= 0)
        // {
        //     text_Key.color = Color.gray;
        //     arrImages[3].material = mtl_Grayscale;
        // }
        //
        // if (trophy == 0 && gold == 0 && key == 0)
        // {
        //     arrImages[0].material = mtl_Grayscale;
        // }

        text_Trophy.text = $"트로피:{trophy}, 시즌포인트:{seasonTrophy}, 랭킹포인트:{rankTrophy}, 골드:{gold}, 키:{key}";
    }

    public void Initialize(MsgReward[] rewards)
    {
        TDataItemList data;
        string str = string.Empty;
        for (int i = 0; i < rewards.Length; i++)
        {
            if (TableManager.Get().ItemList.GetData(rewards[i].ItemId, out data))
            {
                if (i > 0) str += ", ";
                str += $"ItemID:{data.id} Value:{rewards[i].Value}";
            }
        }
    }
}
