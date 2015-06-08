using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

	// This class makes it easier to specify a range in the inspector.
	[Serializable]
	public class FloatCount
	{
		public float minimum;
		public float maximum;
		
		public FloatCount(float min, float max)
		{
			minimum = min;
			maximum = max;
		}
	}

	[HideInInspector]
	public static GameManager instance = null;

	public GameObject[] Npcs;
	public int initialScore = 1000;
	public float killRadius = 1f;
	public int killPenalty = 100;
	public int falseAccusePenalty = 200;
	public int accusations = 3;

	public FloatCount spawnRangeX = new FloatCount(-4f, 4f);
	public FloatCount spawnRangeZ = new FloatCount(-2.5f, 2.5f);

	private int score;
	private float initializationTimer = 2f;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

	}

	public void StartGame()
	{
		foreach(GameObject npc in Npcs)
		{
			Instantiate(npc, new Vector3(Random.Range(spawnRangeX.minimum, spawnRangeX.maximum), 0, 
			                             Random.Range(spawnRangeZ.minimum, spawnRangeZ.maximum)), Quaternion.identity);
		}

		Invoke("SetKiller", initializationTimer);

		score = initialScore;
	}

	void SetKiller()
	{
		GameObject[] characters = GameObject.FindGameObjectsWithTag("Victim");
		int randomIndex = Random.Range(0, Npcs.Length);
		characters[randomIndex].tag = "Killer";
		Debug.Log(characters[randomIndex].name + " is the killer.");
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
		killed.GetComponent<AudioSource>().Play();
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
		LoadLevel("GameOver");
	}

	public void LoadLevel(string levelName)
	{
		if(levelName.StartsWith("Mansion"))
			Invoke("StartGame", initializationTimer);
		Application.LoadLevel(levelName);
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}
}
