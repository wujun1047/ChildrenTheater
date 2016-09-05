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
    bl_HUDText _hudText;

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

        go = GameObject.Find("HUDText");
        if (go != null)
            _hudText = go.GetComponent<bl_HUDText>();

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

    void _OnAnimalFound(AnimalModel animal)
    {

    }

    void _OnAnimalLost(AnimalModel animal)
    {

    }

    void _OnThrowFoodFinish(EventArgs args)
    {
        // 没有动物出现就不处理
        if (_animal == null)
            return;

        EventArgs_ThrowFinish finishArgs = args as EventArgs_ThrowFinish;
        if (finishArgs == null)
        {
            Debug.LogError("_OnThrowFoodFinish: args type is ERROR !!!");
            return;
        }
        
        if (_IsHitAnimal(finishArgs.fPower))
        {
            // 命中的话
            // 判断食物是否符合动物口味
            if (_animal.IsFoodSuitable(finishArgs.eType))
            {
                // 加分
                int score = _CalculateScore(finishArgs.eType, finishArgs.fPower);
                Score += score;
                string tip = "+" + score.ToString();
                _hudText.NewText(tip, _animal.tipRoot, Color.green, 20, 20, 1, 2.2f, bl_Guidance.RightUp);
            }
            else
            {
                // 提示口味不符
                string tip = "我不喜欢吃这个~~~";
                _hudText.NewText(tip, _animal.tipRoot, Color.red, 20, 20, 1, 2.2f, bl_Guidance.Up);
            }
        }
        else
        {
            // 没有命中的话提示
            string tip = "没有接到~~~";
            _hudText.NewText(tip, _animal.tipRoot, Color.red, 20, 20, 1, 2.2f, bl_Guidance.Up);
        }
    }

    /// <summary>
    /// 根据力度判断是否命中
    /// </summary>
    /// <param name="power">力度[0f, 1f]</param>
    /// <returns></returns>
    bool _IsHitAnimal(float power)
    {
        bool bHit = (power >= 0.45f && power <= 0.55f);
        return true;
    }

    /// <summary>
    /// 计算得分。目前先将食物和动物的难度系数相加。
    /// </summary>
    /// <param name="food"></param>
    /// <param name="power"></param>
    /// <returns></returns>
    int _CalculateScore(eFoodType food, float power)
    {
        return FoodItem.GetDifficulty(food) + _animal.difficulty;
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
            float power = _powerBar.GetPower();
            EventDispatcher.Instance.TriggerEvent(new EventArgs_Float(Events.GameEvent.ThrowFoodBegin, power));
        }
        else
        {
            bThrow = false;
            string tip = "换一种食物吧~~~";
            _hudText.NewText(tip, _animal.tipRoot, Color.red, 20, 20, 1, 2.2f, bl_Guidance.Up);
        }
        return bThrow;
    }

    public float GetDifficulity()
    {
        float d = 1f;
        if (_animal == null)
            return d;

        d = _animal.difficulty + FoodItem.GetDifficulty(_bagMgr.SelFoodItem.Type);
        d = Mathf.Lerp(0.5f, 3f, d / 10);
        return d;
    }

    public bool GetAnimalPosition(ref Vector3 pos)
    {
        if (_animal == null)
            return false;

        pos = _animal.transform.position;
        return true;
    }

    public void SetAnimalModel(AnimalModel animal)
    {
        _animal = animal;
    }
}
