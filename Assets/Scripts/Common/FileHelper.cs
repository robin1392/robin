using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class FileHelper 
{

    public static GameObject LoadPrefab(string fileName , Global.E_LOADTYPE loadType , Transform parent = null)
    {
        string rscPath = "";

        switch (loadType)
        {
            case Global.E_LOADTYPE.LOAD_MINION:
                rscPath = "Minion/";
                break;
            case Global.E_LOADTYPE.LOAD_MAGIC:
                rscPath = "Magic/";
                break;
            case Global.E_LOADTYPE.LOAD_MAIN_MINION:
                rscPath = "CommonPrefabs/Units/";
                break;
            case Global.E_LOADTYPE.LOAD_MAIN_MAGIC:
                rscPath = "CommonPrefabs/object/";
                break;
            case Global.E_LOADTYPE.LOAD_COOP_BOSS:
                rscPath = "Boss/";
                break;
            case Global.E_LOADTYPE.LOAD_GUARDIAN:
                rscPath = "Guardian/";
                break;
        }

        GameObject loadObj = Resources.Load(rscPath + fileName) as GameObject;
//        loadObj = GameObject.Instantiate(loadObj);
//        loadObj.name = fileName;
//        if(parent != null)
//            loadObj.transform.parent = parent;
        
        return loadObj;
    }
    



    public static Sprite GetDiceIcon(string name)
    {
        string iconPath = "Image/dice/";
        
        Sprite iconSpr = Resources.Load<Sprite>(iconPath + name);
        if (iconSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  Icon Not exist ");
        }
        
        return iconSpr;
    }

    public static Sprite GetIcon(string name)
    {
        string iconPath = "Image/icon/";
        
        Sprite iconSpr = Resources.Load<Sprite>(iconPath + name);
        if (iconSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  Icon Not exist ");
        }
        
        return iconSpr;
    }

    public static Sprite GetBoxIcon(string name)
    {
        string iconPath = "Image/box/";
        
        Sprite iconSpr = Resources.Load<Sprite>(iconPath + name);
        if (iconSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  Icon Not exist ");
        }
        
        return iconSpr;
    }

    public static Sprite GetCardIcon(string name)
    {
        string cardpath = "Image/dice_card/";
        
        Sprite cardSpr = Resources.Load<Sprite>(cardpath + name);// as Sprite;
        if (cardSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  card Not exist ");
        }
        
        return cardSpr;
    }

    public static Sprite GetIllust(string name)
    {
        string cardpath = "Image/dice_illust/";
        
        Sprite cardSpr = Resources.Load<Sprite>(cardpath + name);// as Sprite;
        if (cardSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  card Not exist ");
        }
        
        return cardSpr;
    }

    public static Sprite GetNullIcon()
    {
        return Resources.Load<Sprite>("Image/null");
    }

    public static Color GetColor(int r, int g, int b)
    {
        Color newColor = new Color32((byte)r , (byte)g , (byte)b , 255);

        return newColor;
    }
    
    public static Color GetColor(int[] color)
    {
        Color newColor = new Color32((byte)color[0] , (byte)color[1] , (byte)color[2] , 255);

        return newColor;
    }
}
