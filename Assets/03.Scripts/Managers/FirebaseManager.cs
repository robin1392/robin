#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class FirebaseManager : Singleton<FirebaseManager>
{
    private void Start()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        
        FirebaseAnalytics.SetUserId(UserInfoManager.Get().GetUserInfo().userID);
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        });
    }

    public void LogEvent(string message)
    {
        Debug.Log($"Firebase LogEvent : {message}");

        FirebaseAnalytics.LogEvent(message);
    }

    public void LogEvent(string message, Parameter[] param)
    {
#if ENABLE_LOG
        string str = $"Firebase LogEvent : {message}\n";
        for(int i = 0; i < param.Length; i++)
        {
            str += $"{param[i]}\n";
        }
        Debug.Log(str);
#endif

        FirebaseAnalytics.LogEvent(message, param);
    }
}
