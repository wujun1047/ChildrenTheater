using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 背包管理，负责背包内物体的初始化和修改
/// </summary>
public class BagManager : MonoBehaviour
{
    public int radishDefault = 10; // 萝卜
    public int boneDefault = 10;   // 骨头
    public int meatDefault = 10;   // 肉
    public int fishDefault = 10;   // 鱼
    public int bananaDefault = 10; // 香蕉
    public int vegetableDefault = 10;// 蔬菜

    Dictionary<eFoodType, int> _initDict; // 各种食物的初始值
    List<FoodItemView> _btnList; // 食物格子列表
    FoodItemView _selFoodBtn; // 当前选中的食物格子

    public FoodItem SelFoodItem
    {
        get
        {
            return _selFoodBtn.ItemData;
        }
    }

    // Use this for initialization
    void Start () {
        _initDict = new Dictionary<eFoodType, int>();
        _initDict.Add(eFoodType.radish, radishDefault);
        _initDict.Add(eFoodType.bone, boneDefault);
        _initDict.Add(eFoodType.meat, meatDefault);
        _initDict.Add(eFoodType.fish, fishDefault);
        _initDict.Add(eFoodType.banana, bananaDefault);
        _initDict.Add(eFoodType.vegetable, vegetableDefault);

        EventDispatcher.Instance.AddListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
        EventDispatcher.Instance.AddListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodBegin);
        EventDispatcher.Instance.AddListener(Events.GameEvent.FoodItemCooldownComplete, _OnFoodItemCooldownComplete);
        _InitFoodBtnList();
	}

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodBegin);
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.FoodItemCooldownComplete, _OnFoodItemCooldownComplete);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void _InitFoodBtnList()
    {
        FoodItemView[] btns = GetComponentsInChildren<FoodItemView>();
        _btnList = new List<FoodItemView>(btns);
        int i = 0;
        _btnList[i++].SetItemData(eFoodType.radish, radishDefault);
        _btnList[i++].SetItemData(eFoodType.bone, boneDefault);
        _btnList[i++].SetItemData(eFoodType.meat, meatDefault);
        _btnList[i++].SetItemData(eFoodType.fish, fishDefault);
        _btnList[i++].SetItemData(eFoodType.banana, bananaDefault);
        _btnList[i++].SetItemData(eFoodType.vegetable, vegetableDefault);

        // 将待扔的食物初始化为第一个食物
        EventDispatcher.Instance.TriggerEvent(new EventArgs_FoodType(Events.GameEvent.SelectedFoodChanged, _btnList[0].ItemData.Type));
    }

    FoodItemView _GetFootBtn(eFoodType eType)
    {
        FoodItemView btn = null;
        if (_btnList == null || _btnList.Count <= 0)
            return btn;

        for (int i = 0; i < _btnList.Count; ++i)
        {
            if (eType == _btnList[i].ItemData.Type)
            {
                btn = _btnList[i];
                break;
            }
        }
        return btn;
    }

    void _OnSelectedFoodChanged(EventArgs args)
    {
        EventArgs_FoodType selectedFoodArgs = args as EventArgs_FoodType;
        if (selectedFoodArgs != null)
        {
            _selFoodBtn = _GetFootBtn(selectedFoodArgs.eFoodType);
        }
        else
        {
            Debug.LogErrorFormat("Error: BagManager::OnSelectedFoodChanged args type is {0}", args.GetType().ToString());
        }
    }

    void _OnThrowFoodBegin(EventArgs args)
    {
        if (_selFoodBtn != null)
        {
            _selFoodBtn.ItemData.Count--;
        }
    }

    void _OnFoodItemCooldownComplete(EventArgs args)
    {
        EventArgs_FoodType foodTypeArgs = args as EventArgs_FoodType;
        if (foodTypeArgs != null)
        {
            FoodItemView btn = _GetFootBtn(foodTypeArgs.eFoodType);
            if (btn != null)
            {
                btn.ItemData.Count = _initDict[foodTypeArgs.eFoodType];
            }
        }
        else
        {
            Debug.LogErrorFormat("Error: _OnFoodItemCooldownComplete args type is {0}", args.GetType().ToString());
        }
    }
}
