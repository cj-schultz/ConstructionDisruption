using UnityEngine;
using UnityEngine.AI;

/*
 Note: This state machine assumes 1 type of resource. It will need changes if we go with multiple types (Such as wood/concrete/glass,etc.)

This component can be placed on workers to allow them to move to and from different objectives
Workers also need a NavMeshAgent placed on them for this to function. A NavMesh must also be baked into the level.
 */

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class WorkerAI : MonoBehaviour
{
    // The resource graphic that the worker "holds", this should be a child of the Enemy prefab.
    public GameObject ResourceGraphic;
    //References to the resource and the foundation that the worker will move between
    public GameObject ResourceObject;
    public GameObject FoundationObject;
    //How long (in seconds) it takes to collect or drop off resources
    public float CollectTime;
    public float DepositTime;

    public GameObject ui;

    //0 for not moving, 1 for moving to resource, 2 for moving to foundation
    private enum NavDestination { NotMoving, MovingToResource, MovingToFoundation }
    private NavDestination NavDest;
    //How much resource the worker currently holds
    private int ResourceCount;

    private NavMeshAgent agent;
    private Rigidbody rb;

    private GameObject player;

    private int hitStreakCount = 0;
    private float lastHitTime = 0;

    //Worker will initially move to resource
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindWithTag("Player");

        ResourceCount = 0;
        ResourceGraphic.SetActive(false);
        NavDest = NavDestination.MovingToResource;
    }

    void Update()
    {
        ui.transform.LookAt(player.transform);
        float yrot = ui.transform.eulerAngles.y;
        ui.transform.eulerAngles = Vector3.zero;
        ui.transform.eulerAngles += Vector3.up * yrot;

        // Determine if the worker reached his destination
        if (Vector3.Distance(transform.position,agent.pathEndPosition) <= 0.4f)
        {
            if (NavDest == NavDestination.MovingToFoundation)
            {
                NavDest = NavDestination.MovingToResource;
                Invoke("ExpendResource", DepositTime);
            }
            else if (NavDest == NavDestination.MovingToResource)
            {
                NavDest = NavDestination.MovingToFoundation;
                Invoke("AcquireResource", CollectTime);
            }
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

    private float hitEnrageTime = 2f;
    public void HitByYell(Vector3 force)
    {
        float hitTimeDifference = Time.time - lastHitTime;
        rb.AddForce(force, ForceMode.Force);
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
        FoundationObject.GetComponent<FoundationHandler>().GiveResource(ResourceCount);
        NavDest = NavDestination.MovingToResource;
    }
}
