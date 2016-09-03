using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static readonly int GameTotalTime = 15 * 60; // 游戏总时间（s）

    bool _bStartGame;
    int _score;     // 分数
    AnimalModel _animal; // 当前动物
    BagManager _bagMgr;
    PowerBar _powerBar;

    private static GameManager _inst = null;
    public static GameManager Instance
    {
        get
        {
            if (_inst == null)
            {
                GameObject go = GameObject.Find("GameManager");
                if (go != null)
                {
                    _inst = go.GetComponent<GameManager>();
                }
                return _inst;
            }
            return _inst;
        }
    }

    public int Score
    {
        get { return _score; }

        set
        {
            _score = value;
            EventDispatcher.Instance.TriggerEvent(new EventArgs_Int(Events.GameEvent.UpdateScore, _score));
        }
    }

    void Awake()
    {
        _bStartGame = false;
        Score = 0;
    }

    void Start()
    {
        GameObject go = GameObject.Find("Bag");
        if (go != null)
            _bagMgr = go.GetComponent<BagManager>();

        go = GameObject.Find("PowerBar");
        if (go != null)
            _powerBar = go.GetComponent<PowerBar>();

        EventDispatcher.Instance.AddListener(Events.GameEvent.ThrowFoodFinish, _OnThrowFoodFinish);
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.ThrowFoodFinish, _OnThrowFoodFinish);
        GameOver();
    }

    void _OnThrowFoodFinish(EventArgs args)
    {
        // 没有动物出现就不处理
        if (_animal == null)
            return;

        EventArgs_ThrowFinish finishArgs = args as EventArgs_ThrowFinish;

        bool bHit = true;// TODO: 先判断是否命中
        if (bHit)
        {
            // 命中的话
            // 判断食物是否符合动物口味
            if (_animal.IsFoodSuitable(finishArgs.eType))
            {
                // 加分
            }
            else
            {
                // 提示口味不符
            }
        }
        else
        {
            // 没有命中的话提示
        }
    }

    public void StartGame()
    {
        _bStartGame = true;
        EventDispatcher.Instance.TriggerEvent(new EventArgs_Int(Events.GameEvent.GameStart, GameTotalTime));
    }

    public void GameOver()
    {
        if (_bStartGame)
        {
            _bStartGame = false;
            HistoryScoresManager.Instance.AddHistoryScore(new HistoryScore(DateTime.Now.Ticks, _score));
            EventDispatcher.Instance.TriggerEvent(new EventArgs_Int(Events.GameEvent.GameOver, _score));
            // TODO: 弹出结算界面
        }
    }

    public bool StartThrowFood()
    {
        bool bThrow = true;
        if (_bagMgr.SelFoodItem.Count > 0)
        {
            float power = 5f;//TODO: 获取蓄力条的力度
            EventDispatcher.Instance.TriggerEvent(new EventArgs_Float(Events.GameEvent.ThrowFoodBegin, power));
        }
        else
        {
            bThrow = false;
            // TODO: 提示选择的食物已经没有了
        }
        return bThrow;
    }

    public float GetDifficulity()
    {
        float d = 1f;
        if (_animal == null)
            return d;

        d = _animal.difficulty + FoodItem.GetDifficulty(_bagMgr.SelFoodItem.Type);
        return d;
    }

    public bool GetAnimalPosition(ref Vector3 pos)
    {
        if (_animal == null)
            return false;

        pos = _animal.transform.position;
        return true;
    }
}
