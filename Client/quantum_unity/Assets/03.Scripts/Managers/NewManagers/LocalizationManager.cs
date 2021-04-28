using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class LocalizationManager
{

    public static string GetLocalCode()
    {
        string countryCode = ObscuredPrefs.GetString("CountryCode");

        if (string.IsNullOrEmpty(countryCode))
        {
            countryCode = Application.systemLanguage.ToString().Substring(0, 2).ToUpper();
            ObscuredPrefs.SetString("CountryCode", countryCode);
        }

        return countryCode;
    }
    
    
    
    
    #region dice desc

    public static string GetLangDesc(int textid)
    {
        Global.COUNTRYCODE code = UnityUtil.GetEnumByStringNonError<Global.COUNTRYCODE>(GetLocalCode());

        switch (code)
        {
            case Global.COUNTRYCODE.KO:
            {
                RandomWarsResource.Data.TDataLangKO tDataLangKO;
                if (TableManager.Get().LangKO.GetData(textid, out tDataLangKO))
                {
                    return tDataLangKO.textDesc;
                }
                else
                {
                    return $"KO {textid}";
                }
            }
            default:
            {
                RandomWarsResource.Data.TDataLangEN tDataLangEN;
                if (TableManager.Get().LangEN.GetData(textid, out tDataLangEN))
                {
                    return tDataLangEN.textDesc;
                }
                else
                {
                    return $"EN {textid}";
                }
            }
        }
    }

    public static string GetLangDesc(string textid)
    {
        Global.COUNTRYCODE code = UnityUtil.GetEnumByStringNonError<Global.COUNTRYCODE>(GetLocalCode());

        switch (code)
        {
            case Global.COUNTRYCODE.KO:
            {
                RandomWarsResource.Data.TDataLangKO tDataLangKO;
                if (TableManager.Get().LangKO.GetData(text => String.Compare(text.name, textid, StringComparison.Ordinal) == 0, out tDataLangKO))
                {
                    return tDataLangKO.textDesc;
                }
                else
                {
                    return $"KO {textid}";
                }
            }
            default:
            {
                RandomWarsResource.Data.TDataLangEN tDataLangEN;
                if (TableManager.Get().LangEN.GetData(text => String.Compare(text.name, textid, StringComparison.Ordinal) == 0, out tDataLangEN))
                {
                    return tDataLangEN.textDesc;
                }
                else
                {
                    return $"EN {textid}";
                }
            }
        }
    }
    
    #endregion
    
}
