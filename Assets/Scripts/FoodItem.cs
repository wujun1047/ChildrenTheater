using System;

public class FoodItem
{
    public event Action CountChangeEvent;

    eFoodType _type;    // 类型
    int _count;         // 数量
    int _cdTime;        // 冷却时间(s)
    int _diffculty;     // 难度系数

    public int Count
    {
        get
        {
            return _count;
        }

        set
        {
            _count = value;
            if (CountChangeEvent != null)
                CountChangeEvent();
        }
    }

    public eFoodType Type
    {
        get
        {
            return _type;
        }
    }

    public int CdTime
    {
        get
        {
            return _cdTime;
        }
    }

    public FoodItem(eFoodType eType, int num, int cd, int d)
    {
        _type = eType;
        _count = num;
        _cdTime = cd;
        _diffculty = d;
    }

    public static string GetSpritePath(eFoodType eType)
    {
        return "Texture/Foods/" + eType.ToString();
    }

    public static string GetPrefabPath(eFoodType eType)
    {
        //TODO:
        //return "Prefabs/" + eType.ToString() + "Prefab";
        return "Prefabs/FoodsModel/ball";
    }
};
