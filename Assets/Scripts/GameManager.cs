using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static readonly int GameTotalTime = 15 * 60; // 游戏总时间（s）

    bool _bStartGame;
    int _score;     // 分数
    
    public static GameManager GetInst
    {
        get
        {
            GameManager mgr = null;
            GameObject go = GameObject.Find("GameManager");
            if (go != null)
            {
                mgr = go.GetComponent<GameManager>();
            }
            return mgr;
        }
    }

    public int Score
    {
        get { return _score; }

        set
        {
            _score = value;

            EventArgs_Int args = new EventArgs_Int();
            args.eventType = Events.GameEvent.UpdateScore;
            args.nValue = _score;
            EventDispatcher.Instance.TriggerEvent(args);
        }
    }

    void Awake()
    {
        _bStartGame = false;
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
        EventArgs_Int args = new EventArgs_Int();
        args.eventType = Events.GameEvent.GameStart;
        args.nValue = GameTotalTime;
        EventDispatcher.Instance.TriggerEvent(args);
    }

    public void GameOver()
    {
        if (_bStartGame)
        {
            _bStartGame = false;
            HistoryScoresManager.Instance.AddHistoryScore(new HistoryScore(DateTime.Now.Ticks, _score));

            EventArgs_Int args = new EventArgs_Int();
            args.eventType = Events.GameEvent.GameOver;
            args.nValue = _score;
            EventDispatcher.Instance.TriggerEvent(args);
        }
    }
}
