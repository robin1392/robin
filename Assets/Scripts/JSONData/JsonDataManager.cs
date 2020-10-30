
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
    public BoxInfo dataBoxInfo = null;
    public DiceLevelUpInfo dataDiceLevelUpInfo = null;
    public GlobalDataInfo dataGlobalDataInfo = null;


    //
    public void LoadJsonData(string filePath , Action<string> callBack = null)
    {
		dataDiceInfo = new DiceInfo(filePath , callBack);
		dataLangKO = new LangKO(filePath , callBack);
		dataLangEN = new LangEN(filePath , callBack);
        dataBoxInfo = new BoxInfo(filePath, callBack);
        dataDiceLevelUpInfo = new DiceLevelUpInfo(filePath, callBack);
        dataGlobalDataInfo = new GlobalDataInfo(filePath, callBack);
    }

    //
    public void DestroyJsonData()
    {
	    if (dataDiceInfo != null)
	    {
		    dataDiceInfo.DestroyData();
		    dataDiceInfo = null;
	    }

	    if (dataLangKO != null)
	    {
		    dataLangKO.DestroyData();
		    dataLangKO = null;
	    }

	    if (dataLangEN != null)
	    {
		    dataLangEN.DestroyData();
		    dataLangEN = null;
	    }

        if (dataBoxInfo != null)
        {
            dataBoxInfo.DestroyData();
            dataBoxInfo = null;
        }

    }

}


