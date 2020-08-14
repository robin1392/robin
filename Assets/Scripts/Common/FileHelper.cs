﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class FileHelper 
{

    public static GameObject LoadPrefab(string fileName , Global.E_LOADTYPE loadType , Transform parent = null)
    {
        string rscPath = "";
        if (loadType == Global.E_LOADTYPE.LOAD_MINION)
        {
            rscPath = "Minion/";
        }
        else if (loadType == Global.E_LOADTYPE.LOAD_MAGIC)
        {
            rscPath = "Magic/";
        }

        GameObject loadObj = Resources.Load(rscPath + fileName) as GameObject;
//        loadObj = GameObject.Instantiate(loadObj);
//        loadObj.name = fileName;
//        if(parent != null)
//            loadObj.transform.parent = parent;
        
        return loadObj;
    }
    



    public static Sprite GetIcon(string name)
    {
        string iconPath = "Image/dice/";
        
        Sprite iconSpr = Resources.Load<Sprite>(iconPath + name);
        if (iconSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  Icon Not exist ");
        }
        
        return iconSpr;
    }

    public static Sprite GetCardIcon(string name)
    {
        string cardpath = "Image/dice_card";
        
        Sprite cardSpr = Resources.Load<Sprite>(cardpath + name);// as Sprite;
        if (cardSpr == null)
        {
            Debug.LogWarning("<color=yellow> " + name + "</color>" + "  card Not exist ");
        }
        
        return cardSpr;
    }


    public static Color GetColor(int r, int g, int b)
    {
        Color newColor = new Color32((byte)r , (byte)g , (byte)b , 255);

        return newColor;
    }
}
