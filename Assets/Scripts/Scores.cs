using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scores : MonoBehaviour {

    Text _scoresText;

	// Use this for initialization
	void Start () {
        _scoresText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void _OnUpdateScores(EventArgs args)
    {
        EventArgs_Int intArgs = args as EventArgs_Int;
        if (intArgs != null)
        {
            _scoresText.text = intArgs.nValue.ToString();
        }
    }
}
