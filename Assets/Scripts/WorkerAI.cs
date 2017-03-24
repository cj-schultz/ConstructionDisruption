using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 Note: This state machine assumes 1 type of resource. It will need changes if we go with multiple types (Such as wood/concrete/glass,etc.)

This component can be placed on workers to allow them to move to and from different objectives
Workers also need a NavMeshAgent placed on them for this to function. A NavMesh must also be baked into the level.
 */

public class WorkerAI : MonoBehaviour {

	//References to the resource and the foundation that the worker will move between
	GameObject ResourceObject;
	GameObject FoundationObject;
	//The maximum distance the worker can be and still interact with the object
	float ResourceMaxDistance;
	float FoundationMaxDistance;
	//How long (in seconds) it takes to collect or drop off resources
	float CollectTime;
	float DepositTime;

	//0 for not moving, 1 for moving to resource, 2 for moving to foundation
	private int NavDest;
	//How much resource the worker currently holds
	private int ResourceCount;

	//Worker will initially move to resource
	void Start () {
		ResourceCount = 0;
		NavDest = 1;
	}
	
	//Update is called once per frame
	void Update () {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		float RDistance = Vector3.Distance(gameObject.transform.position, ResourceObject.transform.position);
		float FDistance = Vector3.Distance(gameObject.transform.position, FoundationObject.transform.position);

		//Check if agent has arrived at destination
		//If yes, then set mode to 0, and load/unload resources after a set time						~Note: (may need a better way to time this, maybe an independent timer? what if worker gets pushed away during transfer)
		if (NavDest == 1 & RDistance < ResourceMaxDistance)
		{
			NavDest = 0;
			Invoke("AcquireResource", CollectTime);
		}
		else if (NavDest == 2 & FDistance < FoundationMaxDistance)
		{
			NavDest = 0;
			Invoke("ExpendResource", DepositTime);
		}

		//Set NavMeshAgent to correct destination based on mode
		if (NavDest == 0)
		{
			agent.destination = gameObject.transform.position;
		}
		if (NavDest == 1)
		{
			agent.destination = ResourceObject.transform.position;
		}
		if (NavDest == 2)
		{
			agent.destination = FoundationObject.transform.position;
		}
	}

	void AcquireResource()
	{
		ResourceCount = ResourceObject.GetComponent<ResourceHandler>().getResource();
		NavDest = 2;
	}

	void ExpendResource()
	{
		FoundationObject.GetComponent<FoundationHandler>().giveResource(ResourceCount);
		NavDest = 1;
	}
}
