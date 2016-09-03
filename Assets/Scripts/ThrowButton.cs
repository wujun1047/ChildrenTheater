using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ThrowButton : MonoBehaviour {

    Button _btn;
	// Use this for initialization
	void Start () {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(_OnButtonClick);
        EventDispatcher.Instance.AddListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodEnd);
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.ThrowFoodBegin, _OnThrowFoodEnd);
    }
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void _OnButtonClick()
    {
        _btn.enabled = false;
        EventDispatcher.Instance.TriggerEvent(new EventArgs(Events.GameEvent.ThrowFoodBegin));
    }
    void _OnThrowFoodEnd(EventArgs args)
    {
        _btn.enabled = true;
    }
}
