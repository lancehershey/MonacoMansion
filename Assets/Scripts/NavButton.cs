using UnityEngine;
using System.Collections;

public class NavButton : MonoBehaviour {

	public void LoadLevel(string levelName)
	{
		Application.LoadLevel(levelName);
	}	
}
