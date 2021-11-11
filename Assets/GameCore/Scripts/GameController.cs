using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] Button _resetBtn;

    [SerializeField] TextMeshProUGUI _scoreTxt;


    void Awake()
    {
        _resetBtn.onClick.AddListener(OnClickReset);

        Board.Instance.OnUpdateScoreAction += OnUpdateScore;
    }

    void OnClickReset()
    {
        var scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
    }

    void OnUpdateScore(int score)
    {
        _scoreTxt.text = score.ToString();
    }

    void OnDestroy()
    {
        _resetBtn.onClick.RemoveListener(OnClickReset);

        Board.Instance.OnUpdateScoreAction -= OnUpdateScore;
    }
}