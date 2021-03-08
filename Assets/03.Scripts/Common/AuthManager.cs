using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Debug = ED.Debug;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using Template.Account.GameBaseAccount.Common;

public class AuthManager : Singleton<AuthManager>
{
     public bool isAuthenticated => Social.localUser.authenticated;
     private EPlatformType type;
     private bool isConfirm;

#if UNITY_ANDROID
     private void Start()
     {
          PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestIdToken().Build();
          
          PlayGamesPlatform.InitializeInstance(config);
          PlayGamesPlatform.DebugLogEnabled = true;

          PlayGamesPlatform.Activate();
     }
#endif
     
     public void Login()
     {
          if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
          {
               Social.localUser.Authenticate((bool success, string error) => // 로그인 시도
               {
                    UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                    UI_Start.Get().btn_GooglePlay.gameObject.SetActive(false);
                    UI_Start.Get().btn_GameCenter.gameObject.SetActive(false);
                    
                    if (success) // 성공하면
                    {
#if UNITY_EDITOR
                         type = EPlatformType.Guest;
#elif UNITY_ANDROID
                         type = EPlatformType.Android;
#elif UNITY_IOS
                         type = EPlatformType.IOS;
#endif
                         StartCoroutine(LoginCoroutine(true));
                    }
                    else // 실패하면
                    {
                         Debug.Log("Login Error : " + error);
                         UI_Start.Get().btn_GuestAccount.gameObject.SetActive(true);
#if UNITY_ANDROID
                         UI_Start.Get().btn_GooglePlay.gameObject.SetActive(true);
#elif UNITY_IOS
                         UI_Start.Get().btn_GameCenter.gameObject.SetActive(true);
#endif
                    }
               });
          }
          else
          {
               UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
               UI_Start.Get().btn_GooglePlay.gameObject.SetActive(false);
               UI_Start.Get().btn_GameCenter.gameObject.SetActive(false);
               
               StartCoroutine(LoginCoroutine(true));
          }
     }

     private GameBaseAccountProtocol.ReceiveAccountPlatformLinkAckDelegate platformLinkCallback;
     public void LinkPlatform(bool isConfirm, GameBaseAccountProtocol.ReceiveAccountPlatformLinkAckDelegate callback)
     {
          this.isConfirm = isConfirm;
          platformLinkCallback = callback;
          if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
          {
               Social.localUser.Authenticate((bool success, string error) => // 로그인 시도
               {
                    if (success) // 성공하면
                    {
#if UNITY_EDITOR
                         type = EPlatformType.Guest;
#elif UNITY_ANDROID
                         type = EPlatformType.Android;
#elif UNITY_IOS
                         type = EPlatformType.IOS;
#endif
                         StartCoroutine(LoginCoroutine(false));
                    }
                    else // 실패하면
                    {
                         Debug.Log("Login Error : " + error);
                    }
               });
          }
          else
          {
#if UNITY_EDITOR
               type = EPlatformType.Guest;
#elif UNITY_ANDROID
               type = EPlatformType.Android;
#elif UNITY_IOS
               type = EPlatformType.IOS;
#endif
               StartCoroutine(LoginCoroutine(false));
          }
     }

     IEnumerator LoginCoroutine(bool isLogin)
     {
          while (string.IsNullOrEmpty(GetTokens()))
          {
               yield return null;
          }

          string token = GetTokens();
          string guid = SystemInfo.deviceUniqueIdentifier;
          string appid = Application.identifier;
          string currentVersion = Application.version;
          string os = SystemInfo.operatingSystem;
          string osVersion = os;
#if UNITY_ANDROID
          osVersion = os.Replace("Android OS ", string.Empty);
#elif UNITY_IOS
          osVersion = os.Replace("iPhone OS ", string.Empty);
#endif
          string device = SystemInfo.deviceUniqueIdentifier;
          string country = Application.systemLanguage.ToString();
          
          //UserInfoManager.Get().GetUserInfo().SetPlatformID(token);

          Debug.Log($"Login Success!\nID: {Social.localUser.id}\nGameID: {Social.localUser.gameId}\nName: {Social.localUser.userName}\nToken: {token}");
          
          if (isLogin)
               NetworkManager.session.AccountTemplate.AccountLoginReq(NetworkManager.session.HttpClient, token, (int)type, guid, string.Empty, appid, currentVersion, os, osVersion, device, country, GameStateManager.Get().OnReceiveAccountLoginAck);
          else
               NetworkManager.session.AccountTemplate.AccountPlatformLinkReq(NetworkManager.session.HttpClient, token, (int)type, isConfirm, platformLinkCallback);
     }

     public string GetTokens()
     {
          if (Social.localUser.authenticated)
          {
#if UNITY_EDITOR
               return null;
#elif UNITY_ANDROID
               return PlayGamesPlatform.Instance.GetIdToken();
#elif UNITY_IOS
               return Social.localUser.id;
#endif
          }
          else
          {
               Debug.Log("Not logined!");
               return null;
          }
     }

     public void Logout()
     {
          if (Social.localUser.authenticated) // 로그인 되어 있다면
          {
#if UNITY_ANDROID
               ((PlayGamesPlatform)Social.Active).SignOut();
#elif UNITY_IOS
#endif
          }
          
          UserInfoManager.Get().GetUserInfo().SetPlatformID(string.Empty);
          ObscuredPrefs.SetInt("PlatformType", (int)EPlatformType.None);
          Debug.Log("GPGS Logout !");
     }
}
