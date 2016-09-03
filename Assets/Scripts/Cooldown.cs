using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class Cooldown : MonoBehaviour {
    
    Text _cdText;
    TimerBehaviour _timer;

	// Use this for initialization
	void Start () {
        _timer = TimerManager.GetTimer(gameObject);
        _cdText = GetComponent<Text>();
        EventDispatcher.Instance.AddListener(Events.GameEvent.GameStart, _OnGameStart);
	}

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(Events.GameEvent.GameStart, _OnGameStart);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void _OnGameStart(EventArgs args)
    {
        EventArgs_Int intArgs = args as EventArgs_Int;
        if (intArgs != null)
        {
            _timer.StartTimer((uint)intArgs.nValue, _OnSecondTimer, _OnCooldownCompelete);
        }
    }

    void _OnSecondTimer(uint seconds)
    {
        uint min = (seconds / 60);
        uint sec = seconds % 60;
        StringBuilder sb = new StringBuilder();
        sb.Append(min.ToString("00"));
        sb.Append(":");
        sb.Append(sec.ToString("00"));
        _cdText.text = sb.ToString();
    }

    void _OnCooldownCompelete()
    {
        GameManager.GetInst.GameOver();
    }
}
