using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour {

	// This class makes it easier to specify a range in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public Sprite[] Items;
	public Sprite[] SuspiciousItems;

	public Count wanderRange = new Count(5, 20);
	public Count wanderSeconds = new Count(2, 5);
	public float wanderDelay = 2;
	public int wanderSpeed = 1;
	public Sprite killerSprite;
	public int runSpeed = 2;

	private NavMeshAgent agent;
	private bool wandering = true;
	private float timer;
	private GameObject killTarget = null;
	private SpriteRenderer spr;
	private Sprite normalSprite;

	private int layerMask = 1 << 9;
	private bool beingAccused = false;
	private bool dead = false;

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		timer = wanderDelay;
		spr = GetComponentInChildren<SpriteRenderer>();
		normalSprite = spr.sprite;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetMouseButtonDown(0) && GameManager.instance.accuseMode)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out hit, 100f, layerMask) && hit.collider.gameObject == gameObject)
			{
				wandering = false;
				agent.SetDestination(transform.position);
				beingAccused = true;
				Debug.Log("Accusing: " + gameObject.name);
			}
			else
			{
				wandering = true;
				beingAccused = false;
			}
		}

		timer -= Time.deltaTime;
		if(wandering && timer < 0)
		{
			wander();
		}
		else if(killTarget)
		{
			agent.SetDestination(killTarget.transform.position);
			if(Vector3.Distance(killTarget.transform.position, transform.position) < GameManager.instance.killRadius)
			{
				killTarget.GetComponent<AudioSource>().Play();
				killTarget.GetComponent<AIController>().Dead();
				GameManager.instance.KillSuccessful(killTarget);
				StopChasing();
			}
		}
	}

	void OnTriggerEnter(Collider character)
	{

		if(!dead && character.tag == "Player")
		{
//			if(beingAccused)
//			{
//				GameManager.instance.accuse(gameObject);
//			}
			character.GetComponent<PlayerController>().target = gameObject;
		}
	}

	void OnTriggerExit(Collider character)
	{
		if(character.tag == "Player")
			character.GetComponent<PlayerController>().target = null;
	}

	// Picks a random point in range and walks to it.
	void wander()
	{
		float walkRadius = Random.Range(wanderRange.minimum, wanderRange.maximum);
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;

		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
		Vector3 finalPosition = hit.position;

		agent.SetDestination(finalPosition);
		//agent.updateRotation = false;
		timer = Random.Range(wanderSeconds.minimum, wanderSeconds.maximum);
	}

	public void Dead()
	{
		agent.speed = 0;
		agent.SetDestination(transform.position);
		wandering = false;
		GetComponentInChildren<SpriteRenderer>().color = Color.red;
		dead = true;
	}

	public void Kill(GameObject target)
	{
		wandering = false;
		killTarget = target;
		spr.sprite = killerSprite;
		agent.speed = runSpeed;
	}

	public void StopChasing()
	{
		wandering = true;
		killTarget = null;
		agent.SetDestination(-transform.position);
		spr.sprite = normalSprite;
		agent.speed = wanderSpeed;
	}
}
