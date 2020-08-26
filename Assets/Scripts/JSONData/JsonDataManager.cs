
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
	public DiceInfo dataDiceInfo = null;
	public LangKO dataLangKO = null;
	public LangEN dataLangEN = null;


    //
    public void LoadJsonData(string filePath , Action<string> callBack = null)
    {
		dataDiceInfo = new DiceInfo(filePath , callBack);
		dataLangKO = new LangKO(filePath , callBack);
		dataLangEN = new LangEN(filePath , callBack);

    }

    //
    public void DestroyJsonData()
    {
		dataDiceInfo.DestroyData();
		dataDiceInfo = null;
		dataLangKO.DestroyData();
		dataLangKO = null;
		dataLangEN.DestroyData();
		dataLangEN = null;

    }

}


