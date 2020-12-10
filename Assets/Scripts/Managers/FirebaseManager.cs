#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public void LogEvent(string message)
    {
        Debug.Log($"Firebase LogEvent : {message}");

        FirebaseAnalytics.LogEvent(message);
    }
}
