using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 1;
	public float runSpeed = 3;

	//[HideInInspector]
	public GameObject target = null;

	private NavMeshAgent agent;
	private bool hiding = false;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, 100))
			{
				agent.SetDestination(hit.point);
				//agent.updateRotation = false;
			}
		}

		if(!hiding && Input.GetKeyDown(KeyCode.LeftShift))
			agent.speed = runSpeed;
		if(!hiding && Input.GetKeyUp(KeyCode.LeftShift))
			agent.speed = walkSpeed;

		if(Input.GetKeyDown(KeyCode.E) && target)
		{
			if(!GameManager.instance.accuseMode)
				target.GetComponent<ObstacleController>().searchForItem();
			else
				GameManager.instance.accuse(target);
		}

		if(Input.GetKeyDown(KeyCode.LeftControl) && target)
		{
			target.GetComponent<ObstacleController>().hide();
			agent.speed = 0;
			hiding = true;
		}
		if(Input.GetKeyUp(KeyCode.LeftControl) && target)
		{
			target.GetComponent<ObstacleController>().unhide();
			agent.speed = walkSpeed;
			hiding = false;
		}
	}
}
