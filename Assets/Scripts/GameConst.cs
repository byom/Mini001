using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConst
{
    public static string s_ProjectID = "2a04b540-a38e-45df-ae75-806444856f9d";

#if UNITY_IOS
    public static string _adBannerUnitId = "Banner_iOS
    public static string _adRewardUnitId = "Rewarded_iOS";
    public static string _adInterstitialUnitId = "Interstitial_iOS";
#elif UNITY_ANDROID
    public static string _adBannerUnitId = "Banner_Android";
    public static string _adRewardUnitId = "Rewarded_Android";
    public static string _adInterstitialUnitId = "Interstitial_Android";
#else
    public static string _adBannerUnitId = "";
    public static string _adRewardUnitId = "";
    public static string _adIntersitialUnitId = "";
#endif

}
