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
	public Text displayText;
	
	public Button accuseButton;
	public bool accuseMode = false;

	public GameObject[] Npcs;
	public GameObject[] Obstacles;
	public GameObject ObstacleContainer;
	public GameObject killer;
	public int initialScore = 1000;
	public float killRadius = 1f;
	public int killPenalty = 100;
	public int falseAccusePenalty = 200;
	public int accusations = 3;
	public float gameTimeInMinutes = 5;
	public float messageFadeDelay = 2;

	public int pointsPerItemFound = 50;

	public Image darkImage;
	public Image flashImage;

	public int itemsCollected = 0;
	public Image[] pokes;
	public int pokeIndex = 0;	// Pokedex?
	
	public Image[] itemSlots;
	public int itemIndex = 0;

	public FloatCount spawnRangeX = new FloatCount(-4f, 4f);
	public FloatCount spawnRangeZ = new FloatCount(-2.5f, 2.5f);
	
	private float initializationTimer = 2f;
	private float timer;
	private float deathInterval;
	private float deathTimer;
	private List<GameObject> victims;
	private List<GameObject> itemLocations;

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

		if(SoundManager.instance)
			SoundManager.instance.PlayGameMusic();

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
		deathInterval = timer/(Npcs.Length + 1);
		deathTimer = deathInterval;
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
		DisplayMessage("Found " + img.name.TrimEnd(new char[] {'_', '1', '2'}));
	}

	public void noItem()
	{
		DisplayMessage("No item here...");
	}

	void DisplayMessage(string message)
	{
		displayText.enabled = true;
		Color textColor = displayText.color;
		displayText.color = new Color(textColor.r, textColor.g, textColor.b, 1.0f);
		displayText.text = message;
		StartCoroutine(FadeText());
	}

	IEnumerator FadeText()
	{
		yield return new WaitForSeconds(messageFadeDelay);
		float t = 1.0f;
		while (t > 0) 
		{
			t -= Time.deltaTime;
			Color textColor = displayText.color;
			displayText.color = new Color(textColor.r, textColor.g, textColor.b, t);
			yield return 0;
		}
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
		DisplayMessage("Accusing " + character.name.TrimEnd("(Clone)".ToCharArray()));
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
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			accuse();
		}

		if(timerText)
		{
			timer -= Time.deltaTime;
			deathTimer -= Time.deltaTime;
			if(deathTimer <= 0)
			{
				StartCoroutine(ScreenFlash());
				killer.GetComponent<AIController>().Kill(victims[Random.Range(0, victims.Count)]);
				deathTimer = deathInterval;
			}
			timerText.text = "Time: " + DisplayTime();
			if(timer <= 0)
				GameOver();
		}
	}

	public void KillSuccessful(GameObject killed)
	{
		victims.Remove(killed);
		darkImage.enabled = false;
		score.Adjust(-killPenalty);
		scoreText.text = "Score: " + score.GetScore();
	}

	string DisplayTime()
	{
		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		return minutes + ":" + seconds;
	}

	void SetKiller()
	{
		GameObject[] characters = GameObject.FindGameObjectsWithTag("Victim");
		int randomIndex = Random.Range(0, Npcs.Length);
		characters[randomIndex].tag = "Killer";
		Debug.Log(characters[randomIndex].name + " is the killer.");
		killer = characters[randomIndex];

		victims = new List<GameObject>(GameObject.FindGameObjectsWithTag("Victim"));
		victims.Add(GameObject.FindGameObjectWithTag("Player"));

		ArrangeItems();
	}

	void ArrangeItems()
	{
		itemLocations = new List<GameObject>();
		for(int i = 0; i < ObstacleContainer.transform.childCount; i++)
		{
			itemLocations.Add(ObstacleContainer.transform.GetChild(i).gameObject);
		}
		//for(int i = 0; i < Obstacles.Length; i++)
		//{
		//	itemLocations.Add(Obstacles[i]);
		//}

		// Avoid placing duplicate items. Keep track of the items that we've added with a hashset.
		HashSet<string> itemsPlaced = new HashSet<string>();

		// Place the killer's items first.
		Sprite[] killerItems = killer.GetComponent<AIController>().SuspiciousItems;
		for(int i = 0; i < 3; i++)
		{
			int randomIndex = Random.Range(0, itemLocations.Count - 1);
			ObstacleController location = itemLocations[randomIndex].GetComponent<ObstacleController>();
			location.item = killerItems[i];
			location.hasItem = true;
			itemsPlaced.Add(killerItems[i].name.TrimEnd(new char[] {'_', '1', '2'}));
			itemLocations.RemoveAt(randomIndex);
		}

		// Now fill in items from the remaining characters.
		int charIndex = 0;
		GameObject[] characters = GameObject.FindGameObjectsWithTag("Victim");
		// 12 item spots - 3 killer items = 9 item slots left to fill.
		for(int i = 0; i < 9; i ++)
		{
			Sprite[] items = characters[charIndex].GetComponent<AIController>().Items;
			for(int j = 0; j < 3; j++)
			{
				string itemName = items[j].name.TrimEnd(new char[] {'_', '1', '2'});
				if(!itemsPlaced.Contains(itemName))
				{
					int randomIndex = Random.Range(0, itemLocations.Count - 1);
					ObstacleController location = itemLocations[randomIndex].GetComponent<ObstacleController>();
					location.item = items[j];
					location.hasItem = true;
					itemsPlaced.Add(itemName);
					itemLocations.RemoveAt(randomIndex);
				}
			}
			charIndex++;
		}
		Debug.Log("Empty Item Locations: " + itemLocations.Count);
	}

	IEnumerator ScreenFlash()
	{
		float offMin = 0.1f;
		float offMax = 0.2f;
		float onMin = 0.05f;
		float onMax = 0.1f;

		for(int flashes = 3; flashes > 0; flashes--)
		{
			flashImage.enabled = true;
			darkImage.enabled = false;
			yield return new WaitForSeconds(Random.Range(onMin, onMax));
			flashImage.enabled = false;
			darkImage.enabled = true;
			yield return new WaitForSeconds(Random.Range(offMin, offMax));
		}
		darkImage.enabled = false;
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

	public void GameOver()
	{
		Debug.Log("Game Over! :(");
		Debug.Log("Final Score: " + score.GetScore());
		if(SoundManager.instance)
			SoundManager.instance.PlayIntroMusic();
		Application.LoadLevel("GameOver");
	}

	void Victory()
	{
		Debug.Log("Killer identified! You win! :D");
		score.Adjust((pointsPerItemFound * itemsCollected) + (int)timer);
		if(SoundManager.instance)
			SoundManager.instance.PlayIntroMusic();
		Application.LoadLevel("WinScreen");
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}
	
}
