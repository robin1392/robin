using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityUtil
{
    
    
    
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
    
    
}
