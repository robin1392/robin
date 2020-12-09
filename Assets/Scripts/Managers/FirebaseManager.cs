using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public void LogEvent(string message)
    {
        FirebaseAnalytics.LogEvent(message);
    }
}
