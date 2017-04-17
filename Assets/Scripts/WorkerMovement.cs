using UnityEngine;
using UnityEngine.AI;

/*
 Note: This state machine assumes 1 type of resource. It will need changes if we go with multiple types (Such as wood/concrete/glass,etc.)

This component can be placed on workers to allow them to move to and from different objectives
Workers also need a NavMeshAgent placed on them for this to function. A NavMesh must also be baked into the level.
 */

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerMovement : MonoBehaviour
{
    // The resource graphic that the worker "holds", this should be a child of the Enemy prefab.
    public GameObject resourceGraphic;        
    //How long (in seconds) it takes to collect or drop off resources
    public float collectTime;
    public float depositTime;
    
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

    private float originalSpeed;
    private float originalAcceleration;
    private bool enraged;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        originalAcceleration = agent.acceleration;

        //Worker will initially move to resource
        resourceCount = 0;
        resourceGraphic.SetActive(false);
        navDest = NavDestination.MovingToResource;

        AssignDestination();
    }    

    void Update()
    {      
        if(enraged)
        {
            if(Vector3.Distance(transform.position, agent.pathEndPosition) <= 0.2f)
            {
                DeEnrage();
            }
        }
        else
        {
            // Determine if the worker reached his destination
            if (!agent.pathPending && Vector3.Distance(transform.position, agent.pathEndPosition) <= 0.2f)
            {
                if (navDest == NavDestination.MovingToFoundation)
                {
                    navDest = NavDestination.NotMoving;
                    Invoke("ExpendResource", depositTime);
                }
                else if (navDest == NavDestination.MovingToResource)
                {
                    navDest = NavDestination.NotMoving;
                    Invoke("AcquireResource", collectTime);
                }
            }
            else
            {
                AssignDestination();
            }
        }                
    }
    
    public void StartEnragedMovement()
    {
        enraged = true;

        Vector3 point = Vector3.zero;

        // Iterate 60 times to try to pick the best point, yeah it's really inefficient, but you can't notice
        for (int i = 0; i < 60; i++)
        {
            // Pick random point from a 40 unit radius from us
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 40;            
            NavMeshHit hit;

            // Make sure the point is a valid nav mesh position
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                point = hit.position;

                // Make sure the point is closer to the current agent destination
                if (Vector3.Distance(randomPoint, agent.destination) < Vector3.Distance(transform.position, agent.destination))
                {                                   
                    break;
                }                    
            }

            if(i == 29 && point == Vector3.zero)
            {
                // Debug.LogError("NOOOOOOO");
            }
        }

        Debug.DrawRay(point, Vector3.up, Color.blue, 3.0f);    

        agent.destination = point;
        agent.speed += 5f;
        agent.acceleration += 20f;
    }                   

    public void DeEnrage()
    {
        if(enraged)
        {
            agent.speed = originalSpeed;
            agent.acceleration = originalAcceleration;

            enraged = false;
            AssignDestination();
        }        
    }

    private void AssignDestination()
    {
        //Set NavMeshAgent to correct destination based on mode
        if (navDest == NavDestination.NotMoving)
        {
            agent.destination = gameObject.transform.position;
        }
        else if (navDest == NavDestination.MovingToResource)
        {
            agent.destination = GetClosestCornerOnObject(targetResource);
        }
        else if (navDest == NavDestination.MovingToFoundation)
        {
            agent.destination = GetClosestCornerOnObject(targetFoundation);
        }
    }

    private Vector3 GetClosestCornerOnObject(GameObject thing)
    {        
        BoxCollider thingCollider = thing.GetComponent<BoxCollider>();
        Vector3 closestPosition = thingCollider.bounds.ClosestPoint(transform.position);

        return closestPosition;
    }

    private void AcquireResource()
    {
        resourceGraphic.SetActive(true);
        
        if(targetResource.GetComponent<ResourceHandler>())
        {
            resourceCount = targetResource.GetComponent<ResourceHandler>().getResource();
        }        

        navDest = NavDestination.MovingToFoundation;
        AssignDestination();
    }

    private void ExpendResource()
    {        
        resourceGraphic.SetActive(false);

        if(targetFoundation.GetComponent<FoundationHandler>())
        {
            targetFoundation.GetComponent<FoundationHandler>().GiveResource(resourceCount);
        }

        targetResource = JobManager.Instance.GetRandomResource();
        navDest = NavDestination.MovingToResource;
        AssignDestination();
    }
}
