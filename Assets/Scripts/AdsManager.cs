using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    // @TODO Config data.
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOsGameId;
    [SerializeField] bool _testMode = true;

    bool _enablePerPlacementMode = false; // 是否单独加载每个adUnit


    private string _gameId;

    public static AdsManager Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<AdsManager>();
            return _inst;
        }
    }

    public static BannerAds BannerAds
    {
        get
        {
            if (_bannerAds == null) _bannerAds = FindObjectOfType<BannerAds>();
            return _bannerAds;
        }
    }

    public static RewardedAds RewardedAds
    {
        get
        {
            if (_rewardedAds == null) _rewardedAds = FindObjectOfType<RewardedAds>();
            return _rewardedAds;
        }
    }

    public static InterstitialAds InterstitialAds
    {
        get
        {
            if (_interstitialAds == null) _interstitialAds = FindObjectOfType<InterstitialAds>();
            return _interstitialAds;
        }
    }

    private static AdsManager _inst;
    private static BannerAds _bannerAds;
    private static RewardedAds _rewardedAds;
    private static InterstitialAds _interstitialAds;


    private 

    void Awake()
    {
        InitializeAds();

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, _enablePerPlacementMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
