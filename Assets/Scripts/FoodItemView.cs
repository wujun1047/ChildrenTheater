using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FoodItemView : MonoBehaviour
{   
    FoodItem _item;
    Button _button;
    Image _image;
    Image _maskImg;
    Text _text;

    public FoodItem ItemData
    {
        get
        {
            return _item;
        }
    }

    void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _maskImg = gameObject.GetComponentByPath<Image>("Mask");
        _text = GetComponentInChildren<Text>();
    }

    // Use this for initialization
    void Start () {

        gameObject.GetOrAddComponent<Button>().onClick.AddListener(OnClick);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        gameObject.GetOrAddComponent<Button>().onClick.RemoveAllListeners();
    }

    public void SetItemData(eFoodType type, int num, int cd, int d)
    {
        if (_item != null)
        {
            _item.CountChangeEvent -= OnCountChangeEvent;
            _item = null;
        }

        _item = new FoodItem(type, num, cd, d);
        string path = FoodItem.GetSpritePath(_item.Type);
        Sprite s = Resources.Load<Sprite>(path);
        if (s != null)
        {
            _image.sprite = s;
        }
        else
        {
            Debug.LogError(string.Format("Sprite \"{0}\" is NOT exist !!!", FoodItem.GetSpritePath(_item.Type)));
        }
        _text.text = _item.Count.ToString();
        _item.CountChangeEvent += OnCountChangeEvent;

        _maskImg.fillAmount = 0f;
    }

    public void OnCountChangeEvent()
    {
        if (_text == null || _item == null)
            return;

        _text.text = _item.Count.ToString();

        if (_item.Count <= 0)
            _StartCooldown();
    }

    void _StartCooldown()
    {
        if (_item == null || _maskImg == null)
            return;

        _button.enabled = false;
        _maskImg.fillAmount = 1f;
        _maskImg.DOFillAmount(0, _item.CdTime).OnComplete(_OnCooldownComplete);
    }

    void _OnCooldownComplete()
    {
        if (_item == null)
            return;

        _button.enabled = true;
        EventArgs_FoodType args = new EventArgs_FoodType();
        args.eventType = Events.GameEvent.FoodItemCooldownComplete;
        args.eFoodType = _item.Type;
        EventDispatcher.Instance.TriggerEvent(args);
    }

    void OnClick()
    {
        if (_item == null)
            return;

        EventArgs_FoodType args = new EventArgs_FoodType();
        args.eventType = Events.GameEvent.SelectedFoodChanged;
        args.eFoodType = _item.Type;
        EventDispatcher.Instance.TriggerEvent(args);
    }
}
