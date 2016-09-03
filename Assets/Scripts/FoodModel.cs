using UnityEngine;
using System.Collections;

public class FoodModel : MonoBehaviour
{
    GameObject _model;
    Projectile _projectile;

    //TODO: 加上对识别到动物模型的响应

	// Use this for initialization
	void Start () {
        _projectile = GetComponentInChildren<Projectile>();
        EventDispatcher.Instance.AddListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
        EventDispatcher.Instance.AddListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodBegin);
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodBegin);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void _OnSelectedFoodChanged(EventArgs args)
    {
        // 食物飞行过程中，不响应该消息
        if (_projectile != null && _projectile.Move)
            return;

        EventArgs_FoodType selectedFoodArgs = args as EventArgs_FoodType;
        if (selectedFoodArgs != null)
        {
            string path = FoodItem.GetPrefabPath(selectedFoodArgs.eFoodType);
            if (_model != null)
            {
                Destroy(_model);
                _model = null;
            }
            _model = Resources.Load<GameObject>(path);
            if (_model != null)
            {
                _model.transform.parent = transform;
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
        if (_projectile != null)
        {
            _projectile.StartProjectile()
        }
    }

    void _OnProjectileComplete()
    {
        // TODO: 加分(放到GameManager做)，食物模型消失
        EventDispatcher.Instance.TriggerEvent(new EventArgs(Events.GameEvent.ThrowFoodFinish));
    }
}
