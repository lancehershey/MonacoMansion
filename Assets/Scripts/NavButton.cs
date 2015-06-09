using UnityEngine;
using System.Collections;

public class NavButton : MonoBehaviour {

	public void LoadLevel(string levelName)
	{
		if(levelName.StartsWith("Mansion"))
			GameManager.instance.LoadLevel(levelName);
		else
			Application.LoadLevel(levelName);
	}	
}
