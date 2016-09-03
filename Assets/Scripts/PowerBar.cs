using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    Slider _slider;
    float _speed;

    // Use this for initialization
    void Start () {
        _speed = 1f;
        _slider = GetComponent<Slider>();
        EventDispatcher.Instance.AddListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.SelectedFoodChanged, _OnSelectedFoodChanged);
    }
	
	// Update is called once per frame
	void Update ()
    {
        _slider.value = Mathf.PingPong(Time.time * _speed, 1f);
    }

    void _OnSelectedFoodChanged(EventArgs args)
    {
        _speed = GameManager.Instance.GetDifficulity();
    }

    public float GetPower()
    {
        return _slider.value;
    }

    float _GetFractionPart(float f)
    {
        int n = (int)f;
        return (f - n);
    }
}
