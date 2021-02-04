using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoPubInternal;
using MoPubInternal.ThirdParty.MiniJSON;


public delegate void MopubCallback();
public delegate void MopubVideoCallback(bool state);

public class MopubCommunicator : MonoBehaviour
{

    public static MopubCommunicator sInstance;

    public static MopubCommunicator Instance
    {
        get
        {
            if(sInstance == null)
            {
                var obj = new GameObject("@MopubCommunicator");
                sInstance = obj.AddComponent<MopubCommunicator>();
            }
            return sInstance;
        }
    }

    string strVideoKey;
    string strInterstitialKey;

    string strAppId;
    string strBannerKey;

    MopubVideoCallback videoCallback = null;
    MopubCallback interstitialCallback = null;

    bool bannerNotCreate = false;

    bool startLoad = false;
    bool initEnded = false;


    bool rewardState = false;


    bool bannerLoadState = false;
    bool rewardLoadState = false;
    bool interstitialLoadState = false;

    void Awake()
    {

        if(sInstance == null)
        {
            sInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }


#if UNITY_ANDROID
        strVideoKey = "eba74787288a4d0bb694aa1513d6c3c6";
        strInterstitialKey = "05ce1a8662d14d4daad4842964a9edaa";

        strAppId = "";
#else
		strVideoKey = "6c4e740a286d4f7fab710dc3af586c7f";
		strInterstitialKey = "c2170d04c20a465792f1646df515e193";

        strAppId = "";
#endif

        MoPub.InitializeSdk(new MoPub.SdkConfiguration
        {

            // MediatedNetworks = new MoPub.MediatedNetwork[]
            // {
            //     new MoPub.SupportedNetwork.AppLovin
            //     {
            //         NetworkConfiguration = new Dictionary< string, object > {
            //             { "sdk_key", "CR4KmCdD0TUPNE-ARH4HtdfmokMLxvZ8KdS4q5IqZnEx0C5STAkKoO5a6Rw2LTAg6dlxk2mHp-CqAHGdR_zEO3" },
            //         }
            //     }
            // },

            AdUnitId = strVideoKey,
            // LogLevel = MoPubBase.LogLevel.MPLogLevelDebug,

        });
        
        
        //MoPub.EnableLocationSupport(Percent.CrossPromotion.hasAgreed);

        MoPub.LoadRewardedVideoPluginsForAdUnits(new string[] { strVideoKey });
        //MoPub.LoadInterstitialPluginsForAdUnits(new string[] { strInterstitialKey });

        MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;

        // MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
        // MoPubManager.OnAdFailedEvent += OnAdFailedEvent;
        // MoPubManager.OnAdClickedEvent += OnAdClickedEvent;
        // MoPubManager.OnAdExpandedEvent += OnAdExpandedEvent;
        // MoPubManager.OnAdCollapsedEvent += OnAdCollapsedEvent;

        MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
        MoPubManager.OnInterstitialShownEvent += OnInterstitialShownEvent;
        MoPubManager.OnInterstitialClickedEvent += OnInterstitialClickedEvent;
        MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
        MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;


        MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
        MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
        MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoExpiredEvent;
        MoPubManager.OnRewardedVideoShownEvent += OnRewardedVideoShownEvent;
        MoPubManager.OnRewardedVideoClickedEvent += OnRewardedVideoClickedEvent;
        MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
        MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
        MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        MoPubManager.OnRewardedVideoLeavingApplicationEvent += OnRewardedVideoLeavingApplicationEvent;

        Debug.Log("Init Start");
    }





    public void loadVideo()
    {
        if(rewardLoadState) return;
        Debug.Log("loadVideo");
        MoPub.RequestRewardedVideo(strVideoKey);
        rewardLoadState = true;
    }

    public bool hasVideo()
    {
        return MoPub.HasRewardedVideo(strVideoKey);
    }

    public void showVideo(MopubVideoCallback callback)
    {
        if(hasVideo())
        {
            rewardLoadState = false;
            videoCallback = callback;
            MoPub.ShowRewardedVideo(strVideoKey);

            Debug.Log("show call!");
        }
        else
        {
            callback(false);
            loadVideo();
        }
    }

    public void OnRewardedVideoLoadedEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoLoadedEvent: " + adUnitId);
    }

    public void OnRewardedVideoFailedEvent(string adUnitId, string error)
    {
        Debug.Log("OnRewardedVideoExpiredEvent: " + error);
        rewardLoadState = false;
    }

    public void OnRewardedVideoExpiredEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoExpiredEvent: " + adUnitId);
        rewardLoadState = false;
    }

    public void OnRewardedVideoShownEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoShownEvent: " + adUnitId);
    }

    public void OnRewardedVideoClickedEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoClickedEvent: " + adUnitId);
    }

    public void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
    {
        rewardLoadState = false;
    }

    public void OnRewardedVideoReceivedRewardEvent(string adUnitId, string label, float amount)
    {
        Debug.Log("OnRewardedVideoReceivedRewardEvent for ad unit id " + adUnitId + " currency:" + label + " amount:" + amount);
        rewardState = true;

        if(videoCallback != null)
        {
            videoCallback(true);
            rewardState = false;
        }
        videoCallback = null;
    }

    public void OnRewardedVideoClosedEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoClosedEvent: " + adUnitId);

        StartCoroutine(loadVideoDelay());
    }

    IEnumerator loadVideoDelay()
    {
        yield return new WaitForSeconds(0.1f);
        loadVideo();
    }

    public void OnRewardedVideoLeavingApplicationEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoLeavingApplicationEvent: " + adUnitId);
    }





    public void loadInterstitial()
    {
        MoPub.RequestInterstitialAd(strInterstitialKey);
    }

    public bool hasInterstitial()
    {
        return MoPub.IsInterstitialReady(strInterstitialKey);
    }

    public void showInterstitial(MopubCallback callback)
    {
        interstitialCallback = callback;
        interstitialLoadState = false;
        MoPub.ShowInterstitialAd(strInterstitialKey);
    }

    public void OnInterstitialLoadedEvent(string adUnitId)
    {
        interstitialCallback = null;
        interstitialLoadState = true;
        Debug.Log("OnInterstitialLoadedEvent: " + adUnitId);
    }

    public void OnInterstitialFailedEvent(string adUnitId, string error)
    {
        Debug.Log("OnInterstitialFailedEvent: " + error);
    }

    public void OnInterstitialShownEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialShownEvent: " + adUnitId);
        if(interstitialCallback != null)
        {
            interstitialCallback();
        }
        interstitialCallback = null;
    }

    public void OnInterstitialClickedEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialClickedEvent: " + adUnitId);
    }
    public void OnInterstitialDismissedEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialDismissedEvent: " + adUnitId);
        loadInterstitial();
    }
    public void OnInterstitialExpiredEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialExpiredEvent: " + adUnitId);
        loadInterstitial();
    }






    // public void showBanner()
    // {
    //     Debug.Log( "show banner" );

    //     if( bannerLoadState ) return;
    //     if( bannerView == null ) return;

    //     // AdRequest request = new AdRequest.Builder().AddTestDevice("295212149BC96FCDB8AB8E37D1C76961").Build();
    //     AdRequest request = new AdRequest.Builder().Build();
    //     bannerView.LoadAd(request);

    //     bannerLoadState = true;
    // }

    // public void destroyBanner()
    // {
    //     Debug.Log( "destroy banner" );

    //     if( bannerView != null )
    //         bannerView.Destroy();
    //     bannerView = null;
    // }

    // public void HandleOnAdLoaded(object sender, EventArgs args)
    // {
    //     MonoBehaviour.print("HandleAdLoaded event received");
    // }

    // public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    // {
    //     bannerLoadState = false;
    //     MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
    //                         + args.Message);
    // }

    // public void HandleOnAdOpened(object sender, EventArgs args)
    // {
    //     MonoBehaviour.print("HandleAdOpened event received");
    // }

    // public void HandleOnAdClosed(object sender, EventArgs args)
    // {
    //     MonoBehaviour.print("HandleAdClosed event received");
    // }

    // public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    // {
    //     MonoBehaviour.print("HandleAdLeavingApplication event received");
    // }











    public void OnSdkInitializedEvent(string adUnitId)
    {
        Debug.Log("OnSdkInitializedEvent: " + adUnitId);
        if(initEnded) return;
        initEnded = true;
        // if( startLoad )
        // {
        loadVideo();
        loadInterstitial();
        // }		
    }

    // public void startLoadAd( bool notCreate )
    // {
    //     Debug.Log( "banner startLoad1 : " + startLoad + ", initEnded : " + initEnded );
    // 	if( startLoad ) return;

    //     bannerNotCreate = notCreate;

    // 	startLoad = true;
    // 	if( initEnded )
    // 	{
    // 		loadVideo();
    //         loadInterstitial();
    // 	}
    // }
}