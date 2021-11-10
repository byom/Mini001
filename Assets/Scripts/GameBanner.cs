using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;


public class GameBanner : MonoBehaviour
{

#if UNITY_IOS
    private string _adUnitId = "Banner_iOS";
#elif UNITY_ANDROID
    private string _adUnitId = "Banner_Android";
#else
    private string _adUnitId = "";
#endif

    void Start()
    {
        LoadBanner();

        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(_adUnitId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(_adUnitId);
    }


    public void LoadBanner()
    {
        Debug.Log("Load Banner");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, options);
    }


    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        ShowBannerAd();
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
    }

    void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(_adUnitId, options);
    }


    void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked()
    {
    }

    void OnBannerShown()
    {
    }

    void OnBannerHidden()
    {
    }
}