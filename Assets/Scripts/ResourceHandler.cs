using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This component is placed on a resource station/generator and keeps track of available resources while allowing AI units to take resources
public class ResourceHandler : MonoBehaviour {

	//How many resources this station starts with
	int ResourceStart;
	//How many resources to give each worker per visit
	int ResourcesPerVisit;

	private int ResourceCount;

	// Use this for initialization
	void Start () {
		ResourceCount = ResourceStart;
	}
	
	// Update is called once per frame
	void Update () {
		//If we ever want to generate more resources every once in awhile, here is the place to do it
	}

	//Called by a worker's AI when it wants to take resources from this resource station
	public int getResource()
	{
		ResourceCount = ResourceCount - ResourcesPerVisit;
		return ResourcesPerVisit;
	}
}
