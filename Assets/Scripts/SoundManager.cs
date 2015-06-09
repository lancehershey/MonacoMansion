using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	[HideInInspector]
	public static SoundManager instance = null;

	public float masterVolume = 1f;
	public float musicVolume = 1f;
	public float fxVolume = 1f;
	public float step = 0.1f;

	private AudioSource music;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		music = GetComponent<AudioSource>();
	}

	public void adjustMaster(int input)
	{
		if(input == -1)
		{
			masterVolume -= step;
			if(masterVolume < 0)
				masterVolume = 0;
		}
		if(input == 0)
		{
			masterVolume = 0;
		}
		if(input == 1)
		{
			masterVolume += step;
			if(masterVolume > 1)
				masterVolume = 1;
		}
	}

	public void adjustMusic(int input)
	{
		if(input == -1)
		{
			musicVolume -= step;
			if(musicVolume < 0)
				musicVolume = 0;
		}
		if(input == 0)
		{
			musicVolume = 0;
		}
		if(input == 1)
		{
			musicVolume += step;
			if(musicVolume > masterVolume)
				musicVolume = masterVolume;
			else if(musicVolume > 1)
				musicVolume = 1;
		}

		music.volume = musicVolume;
	}

	public void adjustFx(int input)
	{
		if(input == -1)
		{
			fxVolume -= step;
			if(fxVolume < 0)
				fxVolume = 0;
		}
		if(input == 0)
		{
			fxVolume = 0;
		}
		if(input == 1)
		{
			fxVolume += step;
			if(fxVolume > masterVolume)
				fxVolume = masterVolume;
			else if(fxVolume > 1)
				fxVolume = 1;
		}
	}
}
