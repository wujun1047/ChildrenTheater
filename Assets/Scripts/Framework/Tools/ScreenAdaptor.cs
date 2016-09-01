using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScreenAdaptor : MonoBehaviour 
{
	public Vector2 _Original;
	public RectTransform _Rect;
	public bool isEnable = false;

	private Transform _myTransform;

	void Start () {
		_myTransform = this.gameObject.transform;
		Refersh ();
		
	}

	#if UNITY_EDITOR
	void Update(){
		Refersh ();
	}
	#endif

	
	private void Refersh()
	{
		if (_Rect == null || _Original.x == 0 || _Original.y == 0) 
		{
			return;		
		}
		float scaleW = Screen.width / _Original.x;
		float scaleH = Screen.height / _Original.y;
		float scale, temp;
		float posX, posY;
		float sdx, sdy;
		if (isEnable) 
		{
			if (scaleW < scaleH) {
				scale = scaleW;

				temp = (1f - scale) * Screen.height;
				posX = (1f - scale) / 2 * Screen.width;
				posY = temp / 2f;

				sdx = _Original.x - Screen.width;
				sdy = temp / scale;
			} else {
				scale = scaleH;
				
				temp = (1f - scale) * Screen.width;
				posY = (1 - scale) / 2 * Screen.height;
				posX = temp / 2f;

				sdx = temp / scale;
				sdy = _Original.y - Screen.height;
			}		
		} else {
			scale = 1;
			posX = 0;
			posY = 0;
			sdx = 0;
			sdy = 0;
		}

		_myTransform.localScale = new Vector3 (scale, scale, 1);
		_myTransform.localPosition = new Vector3 (posX, posY, 0);
		_Rect.sizeDelta = new Vector2 (sdx, sdy);
		
	}
}
