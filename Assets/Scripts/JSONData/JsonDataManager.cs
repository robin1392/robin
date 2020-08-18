
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class JsonDataManager : MonoBehaviour
{
    //singleton
    private static JsonDataManager _instance;
    public static JsonDataManager Get()
    {
        return _instance;
    }
    
    public void Awake()
    {
        _instance = this;
    }
    
    // load class count
	public int loadMaxCount = 1;
    
    
    // json data
	public DiceInfo dataDiceInfo = null;


    //
    public void LoadJsonData(string filePath , Action<string> callBack = null)
    {
		dataDiceInfo = new DiceInfo(filePath , callBack);

    }

    //
    public void DestroyJsonData()
    {
		dataDiceInfo.DestroyData();
		dataDiceInfo = null;

    }

}


