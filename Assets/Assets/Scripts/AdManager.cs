using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using TMPro;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    public string adUnitId;
    
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;
    
    // Callback for when coins should be added
    private System.Action onCoinsRewarded;
    private int coinsToAdd = 0;
    
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
    }

    void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Mobile Ads initialized");
        });

        this.RequestBanner();
        this.LoadAd();  
        this.RequestInterstitial();
        this.RequestRewarded();
    }

    // Your existing banner and interstitial code remains the same...
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-8442722447213282/3719547954";
#elif UNITY_IPHONE
        string adUnitId = "";
#else
        string adUnitId = "unexpected_platform";
#endif
        
        Debug.Log("Creating banner view");
        if (_bannerView != null)
        {
            DestroyAd();
        }
        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void LoadAd()
    {
        if (_bannerView == null)
        {
            Debug.Log("Creating banner view");
            if (_bannerView != null)
            {
                DestroyAd();
            }
            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        }

        var adRequest = new AdRequest();
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-8442722447213282/8780302944";
#elif UNITY_IPHONE
        string adUnitId = "";
#else
        string adUnitId = "unexpected_platform";
#endif
        
        Debug.Log("Creating interstitial view");
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");
        var adRequest = new AdRequest();

        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {
       if (_interstitialAd != null && _interstitialAd.CanShowAd())
       {
           Debug.Log("Showing interstitial ad.");
           _interstitialAd.Show();
       }
       else
       {
           Debug.LogError("Interstitial ad is not ready yet.");
       }
    }

    // ENHANCED REWARDED AD LOGIC FOR COIN REWARDS
    private void RequestRewarded()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-8442722447213282/6307679702";
#elif UNITY_IPHONE
        string adUnitId = "";
#else
        string adUnitId = "unexpected_platform";
#endif

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");
        var adRequest = new AdRequest();

        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    HideLoadingPanel();
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                _rewardedAd = ad;
                RegisterRewardedAdEvents(_rewardedAd);
                HideLoadingPanel();
            });
    }

    // Register events for rewarded ad
    private void RegisterRewardedAdEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed.");
            RequestRewarded(); // Load next ad
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + error);
            ShowLoadingMessage("Ad failed to load. Please try again.");
            StartCoroutine(HideLoadingAfterDelay(2f));
        };
    }

    // MAIN METHOD TO CALL FOR COIN REWARDS
    public void ShowRewardedAdForCoins(int coinAmount, System.Action onSuccess = null)
    {
        coinsToAdd = coinAmount;
        onCoinsRewarded = onSuccess;

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad for coins.");
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad completed. Reward: {reward.Type}, Amount: {reward.Amount}");
                
                // Add coins after successful ad completion
                AddCoinsAfterAd();
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready. Loading...");
            ShowLoadingMessage("Google is loading ads. Kindly wait for sometime...");
            RequestRewarded();
        }
    }

    // Simple method to show rewarded ad
    public void ShowRewardedAdFor3xCoins(int coinAmount, System.Action onSuccess = null)
    {
        coinsToAdd = coinAmount;
        onCoinsRewarded = onSuccess;

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad for coins.");
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad completed. Reward: {reward.Type}, Amount: {reward.Amount}");
                
                // Add coins after successful ad completion
                //AddCoinsAfterAd();
                Add3xcoins();
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready. Loading...");
            ShowLoadingMessage("Google is loading ads. Kindly wait for sometime...");
            RequestRewarded();
        }
    }
    private void Add3xcoins()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.On3xCoin();
            Debug.Log($"Added 30 coins after watching ad!");
        }

        // Call success callback if provided
        onCoinsRewarded?.Invoke();
        
        // Reset values
        //coinsToAdd = 0;
        onCoinsRewarded = null;
        
        HideLoadingPanel();
    }

    // Add coins after successful ad viewing
    private void AddCoinsAfterAd()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(30);
            Debug.Log($"Added 30 coins after watching ad!");
        }

        // Call success callback if provided
        onCoinsRewarded?.Invoke();
        
        // Reset values
        //coinsToAdd = 0;
        onCoinsRewarded = null;
        
        HideLoadingPanel();
    }

    // Loading panel management
    private void ShowLoadingMessage(string message)
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            if (loadingText != null)
            {
                loadingText.text = message;
            }
        }
    }

    private void HideLoadingPanel()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private IEnumerator HideLoadingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideLoadingPanel();
    }

    // Alternative method with custom callback
    public void ShowRewardedAdForCoins(int coinAmount, System.Action onAdCompleted, System.Action onAdFailed)
    {
        coinsToAdd = coinAmount;

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // Add coins after successful ad completion
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddCoins(30);
                }
                
                onAdCompleted?.Invoke();
                coinsToAdd = 0;
            });
        }
        else
        {
            ShowLoadingMessage("Google is loading ads. Kindly wait for sometime...");
            
            // Try to load ad and show when ready
            StartCoroutine(LoadAndShowRewardedAd(coinAmount, onAdCompleted, onAdFailed));
        }
    }

    private IEnumerator LoadAndShowRewardedAd(int coinAmount, System.Action onSuccess, System.Action onFailed)
    {
        RequestRewarded();
        
        float timeout = 10f; // 10 seconds timeout
        float elapsed = 0f;
        
        while (_rewardedAd == null && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            HideLoadingPanel();
            _rewardedAd.Show((Reward reward) =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddCoins(30);
                }
                onSuccess?.Invoke();
            });
        }
        else
        {
            ShowLoadingMessage("Failed to load ad. Please try again later.");
            StartCoroutine(HideLoadingAfterDelay(1f));
            onFailed?.Invoke();
        }
    }
}



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GoogleMobileAds;
// using GoogleMobileAds.Api;
// using JetBrains.Annotations;
// using System;
// using UnityEngine.SceneManagement;

// public class AdManager : MonoBehaviour
// {
//     private BannerView _bannerView;
//     private InterstitialAd interstitial;
//     public string adUnitId;
    

//     // Start is called before the first frame update
//     void Start()
//     {
        
//         // Initialize the Google Mobile Ads SDK.
//         MobileAds.Initialize((InitializationStatus initStatus) =>
//         {
//             // This callback is called once the MobileAds SDK is initialized.
//         });

//         this.RequestBanner();
//         this.RequestInterstitial();
//         this.RequestRewarded();
//     }

//     //Banner ads

//     private void RequestBanner()
//     {
// #if UNITY_ANDROID
//         string adUnitId = "ca-app-pub-8442722447213282/3719547954";
// #elif UNITY_IPHONE
//             string adUnitId = "";
// #else
//         string adUnitId = "unexpected_platform";
// #endif
//         /// <summary>
//         /// Creates a 320x50 banner view at top of the screen.
//         /// </summary>
//         /// 
//         Debug.Log("Creating banner view");

//         // If we already have a banner, destroy the old one.
//         if (_bannerView != null)
//         {
//             DestroyAd();
//         }

//         // Create a 320x50 banner at top of the screen
//         _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

//     }

//     /// <summary>
//     /// Destroys the banner view.
//     /// </summary>
//     public void DestroyAd()
//     {
//         if (_bannerView != null)
//         {
//             Debug.Log("Destroying banner view.");
//             _bannerView.Destroy();
//             _bannerView = null;
//         }
//     }

//     public void LoadAd()
//     {
//         // create an instance of a banner view first.
//         if (_bannerView == null)
//         {
//             Debug.Log("Creating banner view");

//             // If we already have a banner, destroy the old one.
//             if (_bannerView != null)
//             {
//                 DestroyAd();
//             }

//             // Create a 320x50 banner at top of the screen
//             _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
//         }

//         // create our request used to load the ad.
//         var adRequest = new AdRequest();

//         // send the request to load the ad.
//         Debug.Log("Loading banner ad.");
//         _bannerView.LoadAd(adRequest);
//     }

//     //InterstitialAds


//     private void RequestInterstitial()
//     {
// #if UNITY_ANDROID
//         string adUnitId = "ca-app-pub-8442722447213282/8780302944";
// #elif UNITY_IPHONE
//             string adUnitId = "";
// #else
//         string adUnitId = "unexpected_platform";
// #endif
//         /// <summary>
//         /// Creates a 320x50 banner view at top of the screen.
//         /// </summary>
//         /// 
//         Debug.Log("Creating interstitial view");

//         // Clean up the old ad before loading a new one.
//         if (_interstitialAd != null)
//         {
//             _interstitialAd.Destroy();
//             _interstitialAd = null;
//         }

//         Debug.Log("Loading the interstitial ad.");

//         // create our request used to load the ad.
//         var adRequest = new AdRequest();

//         // send the request to load the ad.
//         InterstitialAd.Load(adUnitId, adRequest,
//             (InterstitialAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("interstitial ad failed to load an ad " +
//                                    "with error : " + error);
//                     return;
//                 }

//                 Debug.Log("Interstitial ad loaded with response : "
//                           + ad.GetResponseInfo());

//                 _interstitialAd = ad;
//             });

//     }

//     private InterstitialAd _interstitialAd;

//     /// <summary>
//     /// Loads the interstitial ad.
//     /// </summary>
//     public void LoadInterstitialAd()
//     {
//         // Clean up the old ad before loading a new one.
//         if (_interstitialAd != null)
//         {
//             _interstitialAd.Destroy();
//             _interstitialAd = null;
//         }

//         Debug.Log("Loading the interstitial ad.");

//         // create our request used to load the ad.
//         var adRequest = new AdRequest();

//         // send the request to load the ad.
//         InterstitialAd.Load(adUnitId, adRequest,
//             (InterstitialAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("interstitial ad failed to load an ad " +
//                                    "with error : " + error);
//                     return;
//                 }

//                 Debug.Log("Interstitial ad loaded with response : "
//                           + ad.GetResponseInfo());

//                 _interstitialAd = ad;
//             });
//     }

//     /// <summary>
//     /// Shows the interstitial ad.
//     /// </summary>
//     public void ShowInterstitialAd()
//     {
//        if (_interstitialAd != null && _interstitialAd.CanShowAd())
//        {
//            Debug.Log("Showing interstitial ad.");
//            _interstitialAd.Show();
//        }
//        else
//        {
//            Debug.LogError("Interstitial ad is not ready yet.");
//        }
//     }

//     // public void ShowInterstitialAd(System.Action onAdClosedCallback)
//     // {
//     //     if (_interstitialAd != null && _interstitialAd.CanShowAd())
//     //     {
//     //         Debug.Log("Showing interstitial ad.");

//     //         // Add the event handler for when the ad is closed
//     //         _interstitialAd.OnAdFullScreenContentClosed += () =>
//     //         {
//     //             Debug.Log("Interstitial ad closed.");

//     //             // Call the callback function passed in
//     //             onAdClosedCallback?.Invoke();

//     //             // Remove the event handler to avoid memory leaks
//     //             _interstitialAd.OnAdFullScreenContentClosed -= onAdClosedCallback;
//     //         };

//     //         _interstitialAd.Show();
//     //     }
//     //     else
//     //     {
//     //         Debug.LogError("Interstitial ad is not ready yet.");
//     //         onAdClosedCallback?.Invoke();  // Call the callback even if the ad is not ready
//     //     }
//     // }

    

//     // public void ShowInterstitialhomeAd()
//     // {
//     //     if (_interstitialAd != null && _interstitialAd.CanShowAd())
//     //     {
//     //         Debug.Log("Showing interstitial ad.");

//     //         // Add the event handler for when the ad is closed
//     //         _interstitialAd.OnAdFullScreenContentClosed += HandleOnAdClosed;

//     //         _interstitialAd.Show();
//     //     }
//     //     else
//     //     {
//     //         Debug.LogError("Interstitial ad is not ready yet.");
//     //         HandleOnAdClosed();  // Trigger ad close logic if no ad is shown
//     //     }
//     // }

//     // private void HandleOnAdClosed()
//     // {
//     //     Debug.Log("Interstitial ad closed.");

//     //     // Remove the event handler to avoid memory leaks
//     //     _interstitialAd.OnAdFullScreenContentClosed -= HandleOnAdClosed;

//     //     // Ensure the screen orientation is set correctly
//     //     Screen.orientation = ScreenOrientation.Portrait;

//     //     // Now trigger the next action, like loading the new scene
//     //     SceneManager.LoadScene(0);
//     // }
//     //Rewarded ads

//     private void RequestRewarded()
//     {
// #if UNITY_ANDROID
//         string adUnitId = "ca-app-pub-8442722447213282/6307679702";
// #elif UNITY_IPHONE
//             string adUnitId = "";
// #else
//         string adUnitId = "unexpected_platform";
// #endif

//         // Clean up the old ad before loading a new one.
//         if (_rewardedAd != null)
//         {
//             _rewardedAd.Destroy();
//             _rewardedAd = null;
//         }

//         Debug.Log("Loading the rewarded ad.");

//         // create our request used to load the ad.
//         var adRequest = new AdRequest();

//         // send the request to load the ad.
//         RewardedAd.Load(adUnitId, adRequest,
//             (RewardedAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("Rewarded ad failed to load an ad " +
//                                    "with error : " + error);
//                     return;
//                 }

//                 Debug.Log("Rewarded ad loaded with response : "
//                           + ad.GetResponseInfo());

//                 _rewardedAd = ad;
//             });
//     }

//     RewardedAd _rewardedAd;

//     private bool isAdLoaded = false;

//     public void LoadRewardedAd()
//     {
//         if (_rewardedAd != null)
//         {
//             _rewardedAd.Destroy();
//             _rewardedAd = null;
//         }

//         Debug.Log("Loading the rewarded ad.");

//         var adRequest = new AdRequest();
//         RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
//         {
//             if (error != null || ad == null)
//             {
//                 Debug.LogError("Rewarded ad failed to load: " + error);
//                 isAdLoaded = false;
//                 return;
//             }

//             Debug.Log("Rewarded ad loaded successfully.");
//             _rewardedAd = ad;
//             isAdLoaded = true; // Set the flag when ad is loaded
//         });
//     }

    


//     /// <summary>
//     /// Loads the rewarded ad.
//     /// </summary>
    

//     public void ShowRewardedAd()
//     {
//         const string rewardMsg =
//             "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

//         if (_rewardedAd != null && _rewardedAd.CanShowAd())
//         {
//             _rewardedAd.Show((Reward reward) =>
//             {
//                 // TODO: Reward the user.
//                 Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
//             });
//             LoadRewardedAd();
//         }
//     }

//     public void ShowRewardedcoinAd(Action onAdRewarded)
//     {
//         const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

//         if (_rewardedAd != null && _rewardedAd.CanShowAd())
//         {
//             _rewardedAd.Show((Reward reward) =>
//             {
//                 // Log the reward details (for debugging)
//                 Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

//                 // Call the callback to reward the user
//                 onAdRewarded?.Invoke();

//                 // After the ad is shown, reload a new ad
//                 LoadRewardedAd();  // Re-load the next rewarded ad
//             });
//         }
//         else
//         {
//             Debug.LogError("Rewarded ad is not ready yet.");
//             RequestRewarded();  // If no ad is available, ensure one is being loaded
//         }
//     }
// }

