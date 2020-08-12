using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public struct dataTitle
{
    public string dataName;
    public string dataType;
};


/// <summary>
/// strint => string int 로 만들어진 구조체형 변수
/// </summary>
public struct stringint
{
    public string strEvent;
    public int nValue;
}

public struct longint
{
    public long lValue;
    public int nValue;
}

public class ExtendDataType 
{    
}


#region string parse data
public class StringToDataParse
{
    #region singlton
    static StringToDataParse _instance;
    static public StringToDataParse Get()
    {
        if (_instance == null)
        {
            _instance = new StringToDataParse();
        }
        return _instance;
    }
    #endregion



    #region DataTable Value Parse

    public void DivideData(string dataType, string value, out object valueData)
    {
        valueData = null;

        char divch = ',';
        string[] divres = value.Split(divch);
        int recount = divres.Length;

        switch (dataType)
        {
            case "int":
                {
                    List<int> resultList = new List<int>();
                    for (int i = 0; i < recount; i++)
                    {
                        string cleardata = divres[i].Replace('[', ' ').Trim();
                        cleardata = cleardata.Replace(']', ' ').Trim();
                        int elem = 0;
                        int.TryParse(cleardata, out elem);
                        resultList.Add(elem);
                    }
                    valueData = (object)resultList;
                }
                break;
            case "long":
                {
                    List<long> resultList = new List<long>();
                    for (int i = 0; i < recount; i++)
                    {
                        string cleardata = divres[i].Replace('[', ' ').Trim();
                        cleardata = cleardata.Replace(']', ' ').Trim();
                        long elem = 0;
                        long.TryParse(cleardata, out elem);
                        resultList.Add(elem);
                    }
                    valueData = (object)resultList;
                }
                break;
            case "bool":
                {
                    List<bool> resultList = new List<bool>();
                    for (int i = 0; i < recount; i++)
                    {
                        string cleardata = divres[i].Replace('[', ' ').Trim();
                        cleardata = cleardata.Replace(']', ' ').Trim();
                        bool elem = false;
                        bool.TryParse(cleardata, out elem);
                        resultList.Add(elem);
                    }
                    valueData = (object)resultList;
                }
                break;
            case "string":
                {
                    List<string> resultList = new List<string>();
                    for (int i = 0; i < recount; i++)
                    {
                        string cleardata = divres[i].Replace('[', ' ').Trim();
                        cleardata = cleardata.Replace(']', ' ').Trim();
                        resultList.Add(cleardata);
                    }
                    valueData = (object)resultList;
                }
                break;
            case "stringint":
                {
                    List<stringint> resultList = new List<stringint>();
                    List<string> listElem = GetElementDivide(value);
                    for (int i = 0; i < listElem.Count; i++)
                    {
                        stringint elemt = new stringint();
                        string[] elemDivres = listElem[i].Split(divch);

                        if (elemDivres[0] == null)
                            continue;

                        string cleardata1 = elemDivres[0].Replace('[', ' ').Trim();
                        cleardata1 = cleardata1.Replace(']', ' ').Trim();

                        if (elemDivres.Length <= 1)
                            continue;

                        string cleardata2 = elemDivres[1].Replace('[', ' ').Trim();
                        cleardata2 = cleardata2.Replace(']', ' ').Trim();

                        elemt.strEvent = cleardata1;
                        int.TryParse(cleardata2, out elemt.nValue);

                        resultList.Add(elemt);
                    }

                    valueData = (object)resultList;
                }
                break;
            case "longint":
                {
                    List<longint> resultList = new List<longint>();

                    List<string> listElem = GetElementDivide(value);
                    for (int i = 0; i < listElem.Count; i++)
                    {
                        longint elemt = new longint();
                        string[] elemDivres = listElem[i].Split(divch);

                        if (elemDivres[0] == null)
                            continue;

                        string cleardata1 = elemDivres[0].Replace('[', ' ').Trim();
                        cleardata1 = cleardata1.Replace(']', ' ').Trim();

                        if (elemDivres.Length <= 1)
                            continue;

                        string cleardata2 = elemDivres[1].Replace('[', ' ').Trim();
                        cleardata2 = cleardata2.Replace(']', ' ').Trim();

                        long.TryParse(cleardata1, out elemt.lValue);
                        int.TryParse(cleardata2, out elemt.nValue);

                        resultList.Add(elemt);
                    }

                    valueData = (object)resultList;
                }
                break;
        }
    }

    public List<string> GetElementDivide(string value)
    {
        List<string> listElem = new List<string>();

        char divch = ',';
        string[] divres = value.Split(divch);
        int recount = divres.Length;

        string eleone = "";
        for (int i = 0; i < recount; i++)
        {
            // start
            if (divres[i].Contains("[") == true)
            {
                eleone = "";
            }
            eleone += divres[i];

            //end
            if (divres[i].Contains("]") == true)
            {
                listElem.Add(eleone);
            }
            else
            {
                eleone += ",";
            }
        }
        return listElem;
    }
    #endregion
}
#endregion


#region json string data parse

public class JsonDataParse
{
    
    public static JSONObject OpenFile(string filename)
    {
        string readString = "";
        readString = File.ReadAllText(filename);

        if (string.IsNullOrEmpty(readString))
            return null;
        
        return new JSONObject(readString);
    }

    
    public static object GetParseData(Type type , string dataString)
    {
        string cutString = dataString.Replace("\"", "");
        if (type == typeof(int))
        {
            int returnInt = int.Parse(cutString);
            return returnInt;
        }
        else if (type == typeof(long))
        {
            long returnLong = long.Parse(cutString);
            return returnLong;
        }
        else if (type == typeof(float))
        {
            float returnFloat = float.Parse(cutString);
            return returnFloat;
        }
        else if (type == typeof(string))
        {
            return cutString;
        }
        else if (type == typeof(List<int>))
        {
            string[] values = cutString.Split(',');
            List<int> retList = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                retList.Add(int.Parse(values[i]));
            }
            return retList;
        }
        else if (type == typeof(List<long>))
        {
            string[] values = cutString.Split(',');
            List<long> retList = new List<long>();
            for (int i = 0; i < values.Length; i++)
            {
                retList.Add(long.Parse(values[i]));
            }
            return retList;
        }
        else if (type == typeof(List<string>))
        {
            string[] values = cutString.Split(',');
            List<string> retList = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                retList.Add(values[i]);
            }
            return retList;
        }
        else if (type == typeof(List<float>))
        {
            string[] values = cutString.Split(',');
            List<float> retList = new List<float>();
            for (int i = 0; i < values.Length; i++)
            {
                retList.Add(float.Parse(values[i]));
            }
            return retList;
        }
        
        return null;
    }
    
}

#endregion

