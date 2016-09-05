using UnityEngine;
using System.Collections;
using System;

public class FoodModelController : MonoBehaviour
{
    Projectile _projectile;
    eFoodType _eSelFoodType; // 当前选中的食物类型
    GameObject _foodModel;

    // Use this for initialization
    void Start () {
        EventDispatcher.Instance.AddListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
        EventDispatcher.Instance.AddListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodBegin);
        EventDispatcher.Instance.AddListener(Events.GameEvent.GameOver, _OnGameOver);
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodBegin);
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.GameOver, _OnGameOver);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void _OnSelectedFoodChanged(EventArgs args)
    {
        EventArgs_FoodType selectedFoodArgs = args as EventArgs_FoodType;
        if (selectedFoodArgs != null)
        {
            _eSelFoodType = selectedFoodArgs.eFoodType;
            string path = FoodItem.GetPrefabPath(_eSelFoodType);
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                if (_foodModel != null)
                {
                    // 如果之前的食物还没扔出去，就删除掉，再重新创建一个
                    Projectile p = _foodModel.GetComponent<Projectile>();
                    if (!p.Move)
                    {
                        Destroy(_foodModel);
                        _foodModel = null;
                    }
                }
                _foodModel = Instantiate(prefab);
                Vector3 originPos = _foodModel.transform.localPosition;
                _foodModel.transform.parent = transform;
                _foodModel.transform.localPosition = originPos;
                _projectile = _foodModel.GetComponent<Projectile>();
            }
            else
            {
                Debug.LogErrorFormat("Can NOT Load object in path: {0}", path);
            }
        }
        else
        {
            Debug.LogErrorFormat("Error: FoodModel::OnSelectedFoodChanged args type is {0}", args.GetType().ToString());
        }
    }

    void _OnThrowFoodBegin(EventArgs args)
    {
        Vector3 pos = new Vector3();
        if (!GameManager.Instance.GetAnimalPosition(ref pos))
            return;

        if (_projectile != null)
        {
            EventArgs_Float floatArgs = args as EventArgs_Float;
            if (args != null)
            {
                ThrowParam param = _projectile.gameObject.GetOrAddComponent<ThrowParam>();
                param.power = floatArgs.fValue;
                param.eType = _eSelFoodType;
                _projectile.StartProjectile(pos, _OnProjectileComplete);
            }
        }
    }

    void _OnProjectileComplete(GameObject go)
    {
        ThrowParam param = go.GetComponent<ThrowParam>();
        if (param != null)
        {
            // 通知其他模块处理（加分等）
            EventDispatcher.Instance.TriggerEvent(new EventArgs_ThrowFinish(param.eType, param.power));
        }
        Destroy(go); // 食物模型消失
    }

    void _OnGameOver(EventArgs args)
    {
        // 游戏结束，清除所有子物体
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject go = transform.GetChild(i).gameObject;
            Destroy(go);
        }
    }
}
