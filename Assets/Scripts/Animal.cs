using System.Collections.Generic;

public class Animal
{
    eAnimalType _animalType; // 类型
    int _diffculty; // 难度系数
    List<eFoodType> _foodsList; // 有效的食物列表
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eType">类型</param>
    /// <param name="d">难度系数</param>
    public Animal(eAnimalType eType, int d)
    {
        _animalType = eType;
        _diffculty = d;
        _foodsList = new List<eFoodType>();
    }

    public eAnimalType AnimalType
    {
        get
        {
            return _animalType;
        }
    }

    public void AddFoodType(eFoodType eType)
    {
        if (!_foodsList.Contains(eType))
        {
            _foodsList.Add(eType);
        }
    }

    public bool IsFoodSuitable(eFoodType eFood)
    {
        return _foodsList.Contains(eFood);
    }
}
