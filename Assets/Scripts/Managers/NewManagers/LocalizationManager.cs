using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalizationManager
{

    public static string GetLocalCode()
    {
        string countryCode = "";
        
        countryCode = Application.systemLanguage.ToString().Substring(0, 2).ToUpper();
        
        if (countryCode.Length == 0)
            countryCode = "KO";
        
        if (countryCode != "KO") return "EN";

        return countryCode;
    }
    
    
    
    
    #region dice desc

    public static string GetLangDesc(int textid)
    {
        string descString = "";

        Global.COUNTRYCODE code = UnityUtil.GetEnumByStringNonError<Global.COUNTRYCODE>(GetLocalCode());

        /*descString = code == Global.COUNTRYCODE.KO ?
            JsonDataManager.Get().dataLangKO.GetData(textid).textDesc :
            JsonDataManager.Get().dataLangEN.GetData(textid).textDesc;*/
        
        if (code == Global.COUNTRYCODE.KO)
        {
            RandomWarsResource.Data.TDataLangKO tDataLangKO;
            if (TableManager.Get().LangKO.GetData(textid, out tDataLangKO) == false)
            {
                descString = "KO " + textid.ToString();
                return descString;
            }

            descString = tDataLangKO.textDesc;
        }
        else
        {
            RandomWarsResource.Data.TDataLangKO tDataLangKO;
            if (TableManager.Get().LangKO.GetData(textid, out tDataLangKO) == false)
            {
                descString = "KO " + textid.ToString();
                return descString;
            }

            descString = tDataLangKO.textDesc;
        }
        
        return descString;
    }

    
    #endregion
    
}
