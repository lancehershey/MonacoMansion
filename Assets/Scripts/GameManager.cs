using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public GameManager instance = null;

	public int numberOfNpcs = 10;
	public int initialScore = 1000;

	private int score;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		score = initialScore;
	}

	int GetScore()
	{
		return score;
	}

	void AdjustScore(int adj)
	{
		score += adj;
		if(score <= 0)
			GameOver();
	}

	void GameOver()
	{

	}
}
