﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimalModel : MonoBehaviour
{
    public int difficulty;  // 难度
    public eAnimalType animalType; // 类型
    public eFoodType[] suitableFoods; // 匹配的食物

    List<eFoodType> _foodsList; // 有效的食物列表

    // Use this for initialization
    void Start ()
    {
        // 初始化
        if (suitableFoods != null)
        {
            _foodsList = new List<eFoodType>(suitableFoods);
        }
	}
	
	// Update is called once per frame
	void Update () {

    }

    public bool IsFoodSuitable(eFoodType eFood)
    {
        return _foodsList.Contains(eFood);
    }
}
