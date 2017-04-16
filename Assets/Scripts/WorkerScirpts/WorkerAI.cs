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
    public GameObject resourceGraphic;        
    //How long (in seconds) it takes to collect or drop off resources
    public float collectTime;
    public float depositTime;
    public GameObject ui;

    //References to the resource and the foundation that the worker will move between
    [HideInInspector]
    public GameObject targetResource;
    [HideInInspector]
    public GameObject targetFoundation;

    //0 for not moving, 1 for moving to resource, 2 for moving to foundation
    private enum NavDestination { NotMoving, MovingToResource, MovingToFoundation }
    private NavDestination navDest;
    //How much resource the worker currently holds
    private int resourceCount;

    private NavMeshAgent agent;
    private Rigidbody rb;

    private GameObject player;
        
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindWithTag("Player");

        //Worker will initially move to resource
        resourceCount = 0;
        resourceGraphic.SetActive(false);
        navDest = NavDestination.MovingToResource;
    }

    void Update()
    {
        if(player)
        {
            ui.transform.LookAt(player.transform);
            float yrot = ui.transform.eulerAngles.y;
            ui.transform.eulerAngles = Vector3.zero;
            ui.transform.eulerAngles += Vector3.up * yrot;
        }
       
        // Determine if the worker reached his destination
        if (resourceCount > 0 && Vector3.Distance(transform.position, agent.pathEndPosition) <= 0.4f)
        {            
            if (navDest == NavDestination.MovingToFoundation)
            {
                navDest = NavDestination.MovingToResource;
                Invoke("ExpendResource", depositTime);
                targetResource = JobManager.Instance.GetRandomResource();
            }
            else if (navDest == NavDestination.MovingToResource)
            {
                navDest = NavDestination.MovingToFoundation;
                Invoke("AcquireResource", collectTime);
            }
        }

        //Set NavMeshAgent to correct destination based on mode
        if (navDest == NavDestination.NotMoving)
        {
            agent.destination = gameObject.transform.position;
        }
        if (navDest == NavDestination.MovingToResource)
        {
            agent.destination = targetResource.transform.position;
        }
        if (navDest == NavDestination.MovingToFoundation)
        {
            agent.destination = targetFoundation.transform.position;
        }
    }

    public void HitByYell(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Force);
    }

    void AcquireResource()
    {
        resourceGraphic.SetActive(true);
        if(targetResource.GetComponent<ResourceHandler>())
        {
            resourceCount = targetResource.GetComponent<ResourceHandler>().getResource();
        }        
        navDest = NavDestination.MovingToFoundation;
    }

    void ExpendResource()
    {
        resourceGraphic.SetActive(false);
        if(targetFoundation.GetComponent<FoundationHandler>())
        {
            targetFoundation.GetComponent<FoundationHandler>().GiveResource(resourceCount);
        }        
        navDest = NavDestination.MovingToResource;
    }
}
