using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

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

	//IEnumerator OnTriggerStay(Collider character)
	//{
	//	yield return new WaitForSeconds(1);
	//	if (character.tag == "Player" && Camera.main.transform.position.x != transform.position.x && Camera.main.transform.position.z != transform.position.z)
	//	{
	//		StartCoroutine(Transition());
	//	}
	//}

	void OnTriggerEnter(Collider character)
	{
		if(character.tag == "Player")
		{
			StartCoroutine(Transition());
		}
	}
}
