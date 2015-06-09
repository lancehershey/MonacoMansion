using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObstacleController : MonoBehaviour {

	public bool hideable = true;
	public bool hasItem = true;
	public Sprite item;
	public Image hideImage;

	private bool playerHiding = false;

	void OnTriggerEnter(Collider character)
	{
		if(character.tag == "Player")
			character.GetComponent<PlayerController>().target = gameObject;
	}

	void OnTriggerExit(Collider character)
	{
		if(character.tag == "Player")
			character.GetComponent<PlayerController>().target = null;
	}

	public void searchForItem()
	{
		if(hasItem)
			GameManager.instance.AddItem(item);
		Debug.Log("Picked up item: " + item.name);
	}

	public void hide()
	{
		if(hideable)
		{
			playerHiding = true;
			hideImage.enabled = true;
			Debug.Log("You are hiding!");
		}
	}

	public void unhide()
	{
		if(playerHiding)
		{
			playerHiding = false;
			hideImage.enabled = false;
			Debug.Log("No longer hiding!");
		}
	}
}
