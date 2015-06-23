using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 1;
	public float runSpeed = 3;

	//[HideInInspector]
	public GameObject target = null;

	private NavMeshAgent agent;
	private bool hiding = false;
	private float speed = 1f;
	private bool clickToMove = false;
	//private GameObject[] selectableObjects;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		if(!clickToMove)
		{
			agent.enabled = false;
		}
		//selectableObjects = GameObject.FindGameObjectsWithTag("Selectable");
	}

	void Update()
	{
		//transform.position = new Vector3(transform.position.x, 0, transform.position.z);

		if(!clickToMove && !hiding)
		{
			if(Input.GetKey(KeyCode.A))
			{
				//agent.SetDestination(new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z));
				transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
			}
			if(Input.GetKey(KeyCode.D))
			{
				//agent.SetDestination(new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z));
				transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
			}
			if(Input.GetKey(KeyCode.W))
			{
				//agent.SetDestination(new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime));
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime);
			}
			if(Input.GetKey(KeyCode.S))
			{
				//agent.SetDestination(new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime));
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime);
			}
		}

		if(clickToMove && Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, 100))
			{
				agent.SetDestination(hit.point);
			}
		}

		if(!hiding && Input.GetKeyDown(KeyCode.LeftShift))
		{
			//agent.speed = runSpeed;
			speed = runSpeed;
		}
		if(!hiding && Input.GetKeyUp(KeyCode.LeftShift))
		{
			//agent.speed = walkSpeed;
			speed = walkSpeed;
		}

		if(Input.GetKeyDown(KeyCode.E) && target != null)
		{
			ObstacleController obs = target.GetComponent<ObstacleController>();
			AIController aic = target.GetComponent<AIController>();
			if(obs)
				target.GetComponent<ObstacleController>().searchForItem();
			else if(aic && GameManager.instance.accuseMode)
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

	public bool isHiding()
	{
		return hiding;
	}
}
