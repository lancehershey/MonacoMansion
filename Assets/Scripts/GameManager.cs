using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public static GameManager instance = null;

	public GameObject[] Npcs;
	public int initialScore = 1000;
	public float killRadius = 1f;
	public int killPenalty = 100;

	private int score;
	private float initializationTimer = 5f;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		score = initialScore;

	}

	void Start()
	{
		foreach(GameObject npc in Npcs)
		{
			npc.tag = "Victim";
		}
		
		Invoke("SetKiller", initializationTimer);
	}

	void SetKiller()
	{
		int randomIndex = Random.Range(0, Npcs.Length);
		Npcs[randomIndex].tag = "Killer";
		Debug.Log(Npcs[randomIndex].name + " is the killer.");
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

	public void killCharacter(GameObject killed)
	{
		if(killed.tag == "Player")
		{
			Destroy(killed);
			GameOver();
		}
		Destroy(killed);
		AdjustScore(-killPenalty);
		Debug.Log("Character killed. New score: " + score);
	}

	void GameOver()
	{
		Debug.Log("Player killed. Game Over! :(");
		Debug.Log("Final Score: " + score);
	}
}
