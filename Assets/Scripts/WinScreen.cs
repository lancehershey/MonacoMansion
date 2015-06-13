using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour {

	public Text finalScoreText;

	void Start()
	{
		finalScoreText.text = "Final Score: " + Score.instance.GetScore();
	}
}
