using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

	public List<GameObject> charactersInRoom;
	public float cameraSize = 4f;
	public float transitionDuration = 0.5f;

	private GameObject killer;
	private Sprite normalSprite;

	IEnumerator Transition()
	{
		float t = 0.0f;
		Vector3 startPos = Camera.main.transform.position;
		Vector3 endPos = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
		float startSize = Camera.main.orthographicSize;
		float endSize = cameraSize;
		while (t < 1.0f)
		{ 
			t += Time.deltaTime * (Time.timeScale/transitionDuration);
			Camera.main.transform.position = Vector3.Lerp(startPos, endPos, t);
			Camera.main.orthographicSize = Mathf.Lerp(startSize, endSize, t);
			yield return 0;
		}
	}

	void OnTriggerEnter(Collider character)
	{
		charactersInRoom.Add(character.gameObject);
		if(character.tag == "Player")
		{
			StartCoroutine(Transition());
		}
		else if(character.tag == "Killer" && charactersInRoom.Count > 2)
		{
				character.GetComponent<AIController>().StopChasing();
		}
	}

	void OnTriggerExit(Collider character)
	{
		charactersInRoom.Remove(character.gameObject);
		if (character.tag == "Player")
		{
			//Camera.main.transform.position = new Vector3(0, 10, 0);
			//Camera.main.orthographicSize = 9;
		}
	}

	void Start()
	{
		charactersInRoom.Clear();
	}

	void Update()
	{
		if(charactersInRoom.Count == 2)
		{
			foreach(GameObject c in charactersInRoom)
			{
				if(c.tag == "Killer")
				{
					int target;
					int killerIndex = charactersInRoom.IndexOf(c);
					if(killerIndex == 0)
						target = 1;
					else
						target = 0;
					c.GetComponent<AIController>().Kill(charactersInRoom[target]);
					charactersInRoom.RemoveAt(target);
				}
			}
		}
	}
}
