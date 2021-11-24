using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static readonly string s_ScoreBestKey = "Score_Best";

    [SerializeField] Button _resetBtn;

    [SerializeField] TextMeshProUGUI _scoreTxt;
    [SerializeField] TextMeshProUGUI _bestTxt;


    void Awake()
    {
        Init();

        _resetBtn.onClick.AddListener(OnClickReset);

        Board.Instance.OnUpdateScoreAction += OnUpdateScore;
    }

    void Init()
    {
        _bestTxt.text = PlayerPrefs.GetInt(s_ScoreBestKey).ToString();
    }

    void OnClickReset()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

        //AdsManager.RewardedAds.OnShowResult = delegate(bool arg) 
        //{
            
        //};

        AdsManager.RewardedAds.ShowAd();
    }

    void OnUpdateScore(int score)
    {
        _scoreTxt.text = score.ToString();

        if(PlayerPrefs.GetInt(s_ScoreBestKey) < score)
        {
            _bestTxt.text = score.ToString();

            PlayerPrefs.SetInt(s_ScoreBestKey, score);
        }
    }

    void OnDestroy()
    {
        _resetBtn.onClick.RemoveListener(OnClickReset);

        if(Board.Instance != null)
        {
            Board.Instance.OnUpdateScoreAction -= OnUpdateScore;
        }
    }
}