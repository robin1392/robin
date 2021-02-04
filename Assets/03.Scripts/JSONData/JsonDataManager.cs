
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
        DontDestroyOnLoad(this);
    }
    
    // load class count
	public int loadMaxCount = 3;
    
    
    // json data

    //
    public void LoadJsonData(string filePath , Action<string> callBack = null)
    {
    }

    //
    public void DestroyJsonData()
    {
	    //if (dataDiceInfo != null)
	    //{
		   // dataDiceInfo.DestroyData();
		   // dataDiceInfo = null;
	    //}

    }

}


