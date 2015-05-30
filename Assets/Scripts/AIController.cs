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

	public Count wanderRange = new Count(5, 20);
	public Count wanderSeconds = new Count(2, 5);
	public float wanderDelay = 2;

	private NavMeshAgent agent;
	private bool wandering = true;
	private float timer;

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		timer = wanderDelay;
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer -= Time.deltaTime;
		if(wandering && timer < 0)
		{
			wander();
		}
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
		agent.updateRotation = false;
		timer = Random.Range(wanderSeconds.minimum, wanderSeconds.maximum);
	}
}
