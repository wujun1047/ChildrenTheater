using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public event Action<int> UpdateCountDownEvent;  // 通知界面更新倒计时，参数为时间（s）
    public event Action<int> UpdateScoreEvent;      // 通知界面更新分数，参数为分数
    public event Action<int> GameOverEvent;         // 通知游戏结束，参数为分数

    bool _bStartGame;
    int _cdTimer;   // 倒计时
    int _score;     // 分数

    public int Score
    {
        get { return _score; }

        set
        {
            _score = value;
            if (UpdateScoreEvent != null)
                UpdateScoreEvent(_score);
        }
    }

    void Awake()
    {
        _bStartGame = false;
        _cdTimer = 15 * 60;  //倒计时15分钟
        Score = 0;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        GameOver();
    }

    public void StartGame()
    {
        _bStartGame = true;
        InvokeRepeating("_OneSecondTimer", 0f, 1f);
    }

    public void GameOver()
    {
        if (_bStartGame)
        {
            _bStartGame = false;
            HistoryScoresManager.Instance.AddHistoryScore(new HistoryScore(DateTime.Now.Ticks, _score));
            CancelInvoke("_OneSecondTimer");
            if (GameOverEvent != null)
                GameOverEvent(_score);
        }
    }

    public void AddScore(int s)
    {
        Score += s;
    }

    void _OneSecondTimer()
    {
        _cdTimer--;
        if (UpdateCountDownEvent != null)
            UpdateCountDownEvent(_cdTimer);
    }

    //static int s_score = 100;
    //void OnGUI()
    //{
    //    int idx = 0;
    //    if (GUI.Button(new Rect(10, 20 + 100 * idx++, 150, 80), "AddScore"))
    //    {
    //        HistoryScore hs = new HistoryScore(DateTime.Now.Ticks, s_score++);
    //        HistoryScoresManager.Instance.AddHistoryScore(hs);
    //    }

    //    if (GUI.Button(new Rect(10, 20 + 100 * idx++, 150, 80), "ReadScore"))
    //    {
    //        List<HistoryScore> historyScores = HistoryScoresManager.Instance.HistoryScores;
    //        for (int i = 0; i < historyScores.Count; ++i)
    //        {
    //            HistoryScore hs = historyScores[i];
    //            Debug.Log("T:" + new DateTime(hs.time).ToLongTimeString() + " s:" + hs.score.ToString());
    //        }
    //    }
    //}
}
