using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour {

	public Text master;
	public Text music;
	public Text fx;

	void Awake()
	{
		if(SoundManager.instance)
		{
			master.text = (SoundManager.instance.masterVolume * 100).ToString();
			music.text = (SoundManager.instance.musicVolume * 100).ToString();
			fx.text = (SoundManager.instance.fxVolume * 100).ToString();
		}
	}
}
