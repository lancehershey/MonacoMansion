using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour {

	public Text finalScoreText;

	void Start()
	{
		if(GameManager.instance)
			finalScoreText.text = "Final Score: " + GameManager.instance.GetScore();
	}
}
