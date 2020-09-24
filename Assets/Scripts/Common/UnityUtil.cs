#if UNITY_EDITOR
#define ENABLE_LOG
#endif


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class UnityUtil
{

    public static Color HexToColor(string hex)
    {
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
    
    public static T GetEnumByStringNonError<T>(string value) where T : IConvertible
    {

        if (Enum.IsDefined(typeof(T), value) == false)
        {
            T type = (T)Enum.ToObject(typeof(T), -1);
            return type;
        }
        else
        {
            T type = (T)Enum.Parse(typeof(T), value);
            return type;
        }

    }
    
    #region enum convert

    public static T ToEnum<T>(string val)
    {
        //변환 오류일경우 디폴트
        if (!Enum.IsDefined(typeof(T), val))
            return default(T);

        return (T) System.Enum.Parse(typeof(T), val, true);

    }

    #endregion
    
    
    #region object load

    public static GameObject Instantiate(string pathFile)
    {
        GameObject createobj = Resources.Load<GameObject>(pathFile);
        createobj = GameObject.Instantiate(createobj);

        return createobj;
    }
    #endregion
    
    #region debug color
    
    public static void Print(string preMessge, string message = "", string logColor = "white")
    {
        Debug.Log(string.Format(" {0}  : <color={1}> {2} </color>", preMessge, logColor, message));
    }

    #endregion
    
}

//#define CLIPDEF

public static class ClipboardExtension
{
    /// <summary>
    /// Puts the string into the Clipboard.
    /// </summary>
    public static void CopyToClipboard(this string str)
    {
//#if CLIPDEF
        GUIUtility.systemCopyBuffer = str;
//#endif
    }
}

