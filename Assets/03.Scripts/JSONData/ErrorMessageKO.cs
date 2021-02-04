using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JSONOBJECT;

public class ErrorMessageKOData
{
	public string stingKey = "";
	public int id = 0;
	public string textDesc = "";

}

public class ErrorMessageKO
{
    public Dictionary<string, ErrorMessageKOData> dicData = new Dictionary<string, ErrorMessageKOData>();

    public ErrorMessageKOData GetData(string key)
    {
        if (dicData.ContainsKey(key))
            return dicData[key];

        Debug.Log( "DATA KEY INVALID : " + "<color=yellow>" + key + "</color>");
        return null;
    }
    
    public bool IsContainKey(string key)
    {
        return dicData.ContainsKey(key);
    }
    
    public ErrorMessageKO(string filePath , Action<string> callBack = null)
    {
        JSONObject jsonData = JsonDataParse.OpenFile(filePath + "ErrorMessageKO.json");
        if (jsonData == null)
        {
            Debug.LogError("JSON DATA NOT EXIST : "+ filePath + "ErrorMessageKO.json" );
            return;
        }

        for (int i = 0; i < jsonData.list.Count; i++)
        {
            ErrorMessageKOData info = new ErrorMessageKOData();

			info.stingKey = (string)JsonDataParse.GetParseData(info.stingKey.GetType(), jsonData[i]["stingKey"].ToString());
			info.id = (int)JsonDataParse.GetParseData(info.id.GetType(), jsonData[i]["id"].ToString());
			info.textDesc = (string)JsonDataParse.GetParseData(info.textDesc.GetType(), jsonData[i]["textDesc"].ToString());

            
            dicData.Add(info.stingKey , info);
        }
        
        if(callBack != null)
            callBack("ErrorMessageKO load Complete");
    }


    public void DestroyData()
    {
        dicData.Clear();
        dicData = null;
    }
}


