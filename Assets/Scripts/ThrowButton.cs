using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ThrowButton : MonoBehaviour {

    Button _btn;
    Image _maskImg;
	// Use this for initialization
	void Start ()
    {
        _maskImg = gameObject.GetComponentByPath<Image>("Mask");
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(_OnButtonClick);
    }

    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    void _OnButtonClick()
    {
        if (!GameManager.Instance.StartThrowFood())
            return;

        if (_maskImg)
        {
            // 扔的最小间隔为0.5s
            _btn.enabled = false;
            _maskImg.fillAmount = 1f;
            _maskImg.DOFillAmount(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _btn.enabled = true;
            });
        }
    }
}
