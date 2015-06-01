using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 1;
	public float runSpeed = 3;

	private NavMeshAgent agent;

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
				agent.updateRotation = false;
			}
		}

		if(Input.GetKeyDown(KeyCode.LeftShift))
			agent.speed = runSpeed;
		if(Input.GetKeyUp(KeyCode.LeftShift))
			agent.speed = walkSpeed;
	}
}
