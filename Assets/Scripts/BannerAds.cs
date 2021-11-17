using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;


public class BannerAds : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(GameConst._adBannerUnitId))
        {
            yield return new WaitForSeconds(0.5f);
        }

        ShowBannerAd();
    }
    
    void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.SetPosition(_bannerPosition);
        Advertisement.Banner.Show(GameConst._adBannerUnitId, options);
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