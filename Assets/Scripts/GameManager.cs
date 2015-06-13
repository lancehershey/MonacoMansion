using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

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
	[HideInInspector]
	public Score score;

	public Text scoreText;
	public Text timerText;
	
	public Button accuseButton;
	public bool accuseMode = false;

	public GameObject[] Npcs;
	public GameObject killer;
	public int initialScore = 1000;
	public float killRadius = 1f;
	public int killPenalty = 100;
	public int falseAccusePenalty = 200;
	public int accusations = 3;
	public float gameTimeInMinutes = 10;

	public int pointsPerItemFound = 50;

	public int itemsCollected = 0;
	public Image[] pokes;
	public int pokeIndex = 0;	// Pokedex?
	
	public Image[] itemSlots;
	public int itemIndex = 0;

	public FloatCount spawnRangeX = new FloatCount(-4f, 4f);
	public FloatCount spawnRangeZ = new FloatCount(-2.5f, 2.5f);
	
	private float initializationTimer = 2f;
	private float timer;


//	void Awake()
//	{
//		if(instance == null)
//			instance = this;
//		else if(instance != this)
//			Destroy(gameObject);
//
//		DontDestroyOnLoad(gameObject);
//
//	}

	void Start()
	{
		instance = this;

		score = Score.instance;

		foreach(GameObject npc in Npcs)
		{
			GameObject clone = Instantiate(npc, new Vector3(Random.Range(spawnRangeX.minimum, spawnRangeX.maximum), 0, 
			                             Random.Range(spawnRangeZ.minimum, spawnRangeZ.maximum)), Quaternion.identity) as GameObject;
			clone.layer = 9;
		}

		Invoke("SetKiller", initializationTimer);

		score.SetScore(initialScore);
		scoreText.text = "Score: " + score.GetScore();

		timer = gameTimeInMinutes * 60;
		timerText.text = "Time: " + DisplayTime();

		accuseButton.onClick.AddListener(() => accuse());
	}
	
	public void pickupItem(Sprite img)
	{
		itemsCollected++;
		itemSlots[itemIndex].sprite = img;
		Color opaque = itemSlots[itemIndex].color;
		opaque.a = 1.0f;
		itemSlots[itemIndex++].color = opaque;
	}

	public void accuse()
	{
		accuseMode = !accuseMode;
		if(accuseMode)
			accuseButton.image.color = Color.red;
		else
			accuseButton.image.color = Color.blue;
	}

	public void accuse(GameObject character)
	{
		if(character.tag == "Killer")
		{
			Victory();
		}
		else
		{	
			pokes[pokeIndex++].color = Color.red;
			score.Adjust(-falseAccusePenalty);
			scoreText.text = "Score: " + score.GetScore();
			if(--accusations <= 0)
			{
				GameOver();
			}
		}
	}

	void Update()
	{
		if(timerText)
		{
			timer -= Time.deltaTime;
			timerText.text = "Time: " + DisplayTime();
			if(timer <= 0)
				GameOver();
		}
	}

	string DisplayTime()
	{
		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		if(timer <= (9*60) && seconds == "00")
			killer.GetComponent<AIController>().killingMood = true;
		return minutes + ":" + seconds;
	}

	void SetKiller()
	{
		GameObject[] characters = GameObject.FindGameObjectsWithTag("Victim");
		int randomIndex = Random.Range(0, Npcs.Length);
		characters[randomIndex].tag = "Killer";
		Debug.Log(characters[randomIndex].name + " is the killer.");
		killer = characters[randomIndex];
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
		score.Adjust(-killPenalty);
		scoreText.text = "Score: " + score.GetScore();
		Debug.Log("Character killed. New score: " + score.GetScore());
	}

	void GameOver()
	{
		Debug.Log("Game Over! :(");
		Debug.Log("Final Score: " + score.GetScore());
		Application.LoadLevel("GameOver");
	}

	void Victory()
	{
		Debug.Log("Killer identified! You win! :D");
		score.Adjust((pointsPerItemFound * itemsCollected) + (int)timer);
		Application.LoadLevel("WinScreen");
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}
	
}
