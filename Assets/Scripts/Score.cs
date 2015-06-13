using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	[HideInInspector]
	public static Score instance = null;

	private int score = 0;

	void Start()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void Adjust(int value)
	{
		score += value;
	}

	public int GetScore()
	{
		return score;
	}

	public void SetScore(int value)
	{
		score = value;
	}

	public void Reset()
	{
		score = 0;
	}

}
