using System;

public class FoodItem
{
    public event Action CountChangeEvent;

    eFoodType _type;    // 类型
    int _count;         // 数量

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

    public FoodItem(eFoodType eType, int num)
    {
        _type = eType;
        _count = num;
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
    
    // 冷却时间(s)
    public static int GetCDTime(eFoodType eType)
    {
        int cd = 30;
        switch (eType)
        {
            case eFoodType.banana:
                cd = 30;
                break;
            case eFoodType.bone:
                cd = 30;
                break;
            case eFoodType.fish:
                cd = 30;
                break;
            case eFoodType.meat:
                cd = 30;
                break;
            case eFoodType.radish:
                cd = 30;
                break;
            case eFoodType.vegetable:
                cd = 30;
                break;
            default:
                break;
        }
        return cd;
    }
    
    // 难度系数
    public static int GetDifficulty(eFoodType eType)
    {
        int d = 3;
        switch (eType)
        {
            case eFoodType.banana:
                d = 3;
                break;
            case eFoodType.bone:
                d = 1;
                break;
            case eFoodType.fish:
                d = 2;
                break;
            case eFoodType.meat:
                d = 5;
                break;
            case eFoodType.radish:
                d = 2;
                break;
            case eFoodType.vegetable:
                d = 4;
                break;
            default:
                break;
        }
        return d;
    }
};
