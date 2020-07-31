#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Firebase.Auth;
using UnityEngine.SceneManagement;

namespace ED
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager instance;

        public Button button_GuestLogin;
        public GameObject obj_InputName;
        public InputField InputField_Name;

        // private FirebaseAuth auth;
        // private FirebaseUser user;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        //
        // private void Start()
        // {
        //     auth = FirebaseAuth.DefaultInstance;
        //     auth.StateChanged += AuthStateChanged;
        //     AuthStateChanged(this, null);
        //
        //     if (user != null && !string.IsNullOrEmpty(user.UserId))
        //     {
        //         FindObjectOfType<Loading>().LoadMainScene();
        //     }
        //     else
        //     {
        //         button_GuestLogin.gameObject.SetActive(true);
        //     }
        // }
        //
        // void AuthStateChanged(object sender, EventArgs eventArgs) {
        //     if (auth.CurrentUser != user) {
        //         bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
        //         if (!signedIn && user != null) {
        //             Debug.Log("Signed out " + user.UserId);
        //         }
        //         user = auth.CurrentUser;
        //         if (signedIn) {
        //             Debug.Log("Signed in " + user.UserId);
        //             string displayName = user.DisplayName ?? "";
        //             string emailAddress = user.Email ?? "";
        //             //var photoUrl = user.PhotoUrl ?? "";
        //         }
        //     }
        // }
        // public void Click_GuestLoginButton()
        // {
        //     obj_InputName.SetActive(true);
        // }
        //
        // public void Click_InputNameCompleteButton()
        // {
        //     auth = FirebaseAuth.DefaultInstance;
        //     auth.SignInAnonymouslyAsync().ContinueWith(task =>
        //     {
        //         if (task.IsCanceled)
        //         {
        //             Debug.LogError("SignInAnonymouslyAsync was canceled.");
        //             return;
        //         }
        //
        //         if (task.IsFaulted)
        //         {
        //             Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
        //             return;
        //         }
        //
        //         user = task.Result;
        //         Debug.LogFormat("User signed in successfully: {0} ({1})",
        //             user.DisplayName, user.UserId);
        //     });
        // }
    }
}
