using UnityEngine;
using UnityEngine.AI;

/*
 Note: This state machine assumes 1 type of resource. It will need changes if we go with multiple types (Such as wood/concrete/glass,etc.)

This component can be placed on workers to allow them to move to and from different objectives
Workers also need a NavMeshAgent placed on them for this to function. A NavMesh must also be baked into the level.
 */

public class WorkerAI : MonoBehaviour
{
    // The resource graphic that the worker "holds", this should be a child of the Enemy prefab.
    public GameObject ResourceGraphic;
    //References to the resource and the foundation that the worker will move between
    public GameObject ResourceObject;
    public GameObject FoundationObject;
    //The maximum distance the worker can be and still interact with the object
    public float ResourceMaxDistance;
    public float FoundationMaxDistance;
    //How long (in seconds) it takes to collect or drop off resources
    public float CollectTime;
    public float DepositTime;

    //0 for not moving, 1 for moving to resource, 2 for moving to foundation
    private enum NavDestination { NotMoving, MovingToResource, MovingToFoundation }    
    private NavDestination NavDest;
    //How much resource the worker currently holds
    private int ResourceCount;

    //Worker will initially move to resource
    void Start()
    {
        ResourceCount = 0;
        ResourceGraphic.SetActive(false);
        NavDest = NavDestination.MovingToResource;        
    }

    //Update is called once per frame
    void Update()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float RDistance = Vector3.Distance(gameObject.transform.position, ResourceObject.transform.position);
        float FDistance = Vector3.Distance(gameObject.transform.position, FoundationObject.transform.position);

        //Check if agent has arrived at destination
        //If yes, then set mode to 0, and load/unload resources after a set time						~Note: (may need a better way to time this, maybe an independent timer? what if worker gets pushed away during transfer)
        if (NavDest == NavDestination.MovingToResource & RDistance < ResourceMaxDistance)
        {
            NavDest = 0;
            Invoke("AcquireResource", CollectTime);
        }
        else if (NavDest == NavDestination.MovingToFoundation & FDistance < FoundationMaxDistance)
        {
            NavDest = 0;
            Invoke("ExpendResource", DepositTime);
        }

        //Set NavMeshAgent to correct destination based on mode
        if (NavDest == NavDestination.NotMoving)
        {
            agent.destination = gameObject.transform.position;
        }
        if (NavDest == NavDestination.MovingToResource)
        {
            agent.destination = ResourceObject.transform.position;
        }
        if (NavDest == NavDestination.MovingToFoundation)
        {
            agent.destination = FoundationObject.transform.position;
        }
    }

    void AcquireResource()
    {
        ResourceGraphic.SetActive(true);
        ResourceCount = ResourceObject.GetComponent<ResourceHandler>().getResource();
        NavDest = NavDestination.MovingToFoundation;
    }

    void ExpendResource()
    {
        ResourceGraphic.SetActive(false);
        FoundationObject.GetComponent<FoundationHandler>().giveResource(ResourceCount);
        NavDest = NavDestination.MovingToResource;
    }
}
