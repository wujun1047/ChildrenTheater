﻿using System;

public class FoodItem
{
    public event Action CountChangeEvent;

    eFoodType _type;    // 类型
    int _count;         // 数量
    int _cdTime;        // 冷却时间(s)

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

    public FoodItem(eFoodType eType, int num, int cd)
    {
        _type = eType;
        _count = num;
        _cdTime = cd;
    }

    public string GetSpritePath()
    {
        return "Texture/" + _type.ToString();
    }

    public string GetPrefabName()
    {
        return "Prefabs/" + _type.ToString() + "Prefab";
    }
};