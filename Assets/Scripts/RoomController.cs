using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

	public List<GameObject> charactersInRoom;
	public float cameraSize = 4f;

	void OnTriggerEnter(Collider character)
	{
		charactersInRoom.Add(character.gameObject);
		if(character.tag == "Player")
		{
			Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
			Camera.main.orthographicSize = cameraSize;
		}
	}

	void OnTriggerExit(Collider character)
	{
		charactersInRoom.Remove(character.gameObject);
	}

	void Start()
	{
		charactersInRoom.Clear();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
