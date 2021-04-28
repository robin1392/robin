#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
//using Firebase.Crashlytics;

public class FirebaseManager : Singleton<FirebaseManager>
{
    private void Start()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        var userInfo = UserInfoManager.Get().GetUserInfo(); 
        SetUserId(userInfo.userID);
        SetNickName(userInfo.userNickName);

        userInfo.RegisterOnSetUserId(SetUserId);
        userInfo.RegisterOnSetUserNickName(SetNickName);

        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        });
    }

    private void SetNickName(string userNickName)
    {
        FirebaseAnalytics.SetUserProperty("Nick", userNickName);
        //Crashlytics.SetCustomKey("Nick", userNickName);
    }

    void SetUserId(string userId)
    {
        FirebaseAnalytics.SetUserId(userId);
        //Crashlytics.SetUserId(userId);
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
