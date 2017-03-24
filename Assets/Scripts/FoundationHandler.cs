using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This component is placed on a foundation and keeps track of its progress towards being completed. It also allows the foundation to accept resources from AI units.
public class FoundationHandler : MonoBehaviour {

	//How many resources are needed for the foundation to be finished different stages
	int Stage1Count;
	int Stage2Count;
	int Stage3Count;
	int FinalCount;

	//Keeps track of how many resources the foundation has
	private int CurrentCount;

	// Use this for initialization
	void Start () {
		CurrentCount = 0;
	}
	
	// Update is called once per frame
	void Update () {

		//Note: Also need to update the GUI progress bar in this space

		if (CurrentCount >= Stage1Count & CurrentCount < Stage2Count)
		{
			//Change artwork here
		}
		else if (CurrentCount >= Stage2Count & CurrentCount < Stage3Count)
		{
			//Change artwork here
		}
		else if (CurrentCount >= Stage3Count & CurrentCount < FinalCount)
		{
			//Change artwork here
		}
		else if (CurrentCount >= FinalCount)
		{
			//Change artwork here
			//Also run any code connected to this foundation being finished (Ex. any end game conditions)
		}
	}

	//Called by a worker's AI when it wants to give resources to this foundation
	public void giveResource(int Given)
	{
		CurrentCount = CurrentCount + Given;
	}
}
