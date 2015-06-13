using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObstacleController : MonoBehaviour {

	public bool hideable = true;
	public bool hasItem = true;
	public Sprite item;
	public Image hideImage;
	
	public bool selected = false;

	private bool playerHiding = false;
	private int layerMask = 1 << 8;

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray, out hit, 100f, layerMask) && hit.collider.gameObject == gameObject)
			{
				selected = true;
				Debug.Log("Selected: " + gameObject.name);
			}
			else
			{
				selected = false;
				Debug.Log("Deselected: " + gameObject.name);
			}
		}
	}

	void OnTriggerEnter(Collider character)
	{
		if(character.tag == "Player")
		{
			if(selected)
			{
				searchForItem();
			}
			character.GetComponent<PlayerController>().target = gameObject;
		}
	}

	void OnTriggerExit(Collider character)
	{
		if(character.tag == "Player")
			character.GetComponent<PlayerController>().target = null;
	}

	public void searchForItem()
	{
		if(hasItem)
		{
			GameManager.instance.pickupItem(item);
			hasItem = false;
		}
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
