using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public UnityAction<bool> OnShowResult;
    
    public void ShowAd()
    {
        StartCoroutine(ShowAdsWhenReady());
    }

    IEnumerator ShowAdsWhenReady()
    {
        while (!Advertisement.IsReady(GameConst._adRewardUnitId))
        {
            yield return new WaitForSeconds(0.5f);
        }

        Advertisement.Show(GameConst._adRewardUnitId, this);
    }


    // Interface
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(GameConst._adRewardUnitId))
        {
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");

        if (adUnitId.Equals(GameConst._adRewardUnitId))
        {
        }
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(GameConst._adRewardUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");

            if (OnShowResult != null)
            {
                OnShowResult(true);
            }
        }
    }


    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        if (OnShowResult != null)
        {
            OnShowResult(false);
        }
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
    }
}
