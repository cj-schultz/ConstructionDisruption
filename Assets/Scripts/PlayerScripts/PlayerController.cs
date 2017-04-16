using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float turnSpeed;   
    public GameObject yellPrefab;
    public Transform yellFirePoint;

    public float yellCooldown = 2; // in seconds
    // Make this public to read, but privite to write
    public float yellCooldownTimeLeft { get; private set; }

    private Rigidbody rb;
    private BoxCollider boxCollider;

    private Vector3 rayDirectionLeft;
    private Vector3 rayDirectionRight;

    void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        yellCooldownTimeLeft = 0f;

        boxCollider = GetComponent<BoxCollider>();
        
        rayDirectionLeft = -transform.right * yellPrefab.transform.localScale.x / 2;
        rayDirectionRight = transform.right * yellPrefab.transform.localScale.x / 2;
    }

    private bool yelling = false;

	void Update ()
    {
        //
        // DEBUG DRAWING
        //
        /*
        RaycastHit leftHit;
        RaycastHit rightHit;

        float leftLength = yellPrefab.transform.localScale.x / 2;
        float rightLength = yellPrefab.transform.localScale.x / 2;

        Color leftRayColor = Color.blue;
        Color rightRayColor = Color.blue;

        // left
        if (Physics.Raycast(yellFirePoint.transform.position, -transform.right * yellPrefab.transform.localScale.x / 2, out leftHit, yellPrefab.transform.localScale.x / 2))
        {
            float hitDistance = leftHit.distance;
            leftLength = hitDistance;
            leftRayColor = Color.red;
        }
        // right
        if (Physics.Raycast(yellFirePoint.transform.position, transform.right * yellPrefab.transform.localScale.x / 2, out rightHit, yellPrefab.transform.localScale.x / 2))
        {
            float hitDistance = rightHit.distance;
            rightLength = hitDistance;
            rightRayColor = Color.red;
        }

        Debug.DrawRay(yellFirePoint.transform.position, -transform.right * leftLength, leftRayColor);
        Debug.DrawRay(yellFirePoint.transform.position, transform.right * rightLength, rightRayColor);
        */
        //
        //
        //

        Vector3 direction = Vector3.zero;
        Quaternion rot = transform.rotation;

        if(yellCooldownTimeLeft >= 0)
        {
            yellCooldownTimeLeft -= Time.deltaTime;
        }        

        // Standard movement stuff, use force to move so he doesn't phase through walls
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rot *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime * -1f, Vector3.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(transform.forward * speed * -1f, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rot *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime * 1f, Vector3.up);
        }

#if true
        if (Input.GetKey(KeyCode.Space))
        {
            // Yell if the cooldown is up
            if (yellCooldownTimeLeft <= 0)
            {
                Yell();                
            }
        }
#else
        if(Input.GetKeyDown(KeyCode.Space) && !yelling)
        {
            yelling = true;
            StartCoroutine("DoYelling");
        }
        else if(Input.GetKeyUp(KeyCode.Space) && yelling)
        {
            yelling = false;
            StopCoroutine("DoYelling");
        }
#endif

        transform.position += direction * Time.deltaTime * speed;
        transform.localRotation = rot;
    }

    private IEnumerator DoYelling()
    {
        while(true)
        {
            GameObject yellBlock = Instantiate(yellPrefab, yellFirePoint.position, transform.localRotation) as GameObject;
            AdjustYellInitialScale(yellBlock);

            yield return new WaitForSeconds(.1f);
        }
    }

    // Spawns and clips the yell block
    private void Yell()
    {
        // Note(colin): This pushes back the player a little when they yell. Just testing the feel of this.
        //rb.AddForce(-transform.forward * 20f, ForceMode.Impulse);

        GameObject yellBlock = Instantiate(yellPrefab, yellFirePoint.position, transform.localRotation) as GameObject;
        yellCooldownTimeLeft = yellCooldown;

        AdjustYellInitialScale(yellBlock);
    }

    private void AdjustYellInitialScale(GameObject yellBlock)
    {        
        // Just a little padding to scale the yell block down more.
        float scaleEpsilon = 0.1f;

        RaycastHit leftHit;
        RaycastHit rightHit;

        float halfLengthOfYellBlock = yellPrefab.transform.localScale.x / 2;

        Vector3 oldScale = yellBlock.transform.localScale;

        // Raycast to the left of the yell fire point
        if (Physics.Raycast(yellFirePoint.transform.position, -transform.right * yellPrefab.transform.localScale.x / 2, out leftHit, yellPrefab.transform.localScale.x / 2))
        {         
            // A wall on the left was found, scale down and reposition the block.   
            float amountToScaleDown = halfLengthOfYellBlock - leftHit.distance + scaleEpsilon;
            yellBlock.transform.localScale = new Vector3(oldScale.x - amountToScaleDown, oldScale.y, oldScale.z);
            yellBlock.transform.position += transform.right * amountToScaleDown / 2;
        }

        // Reset the "old" scale
        oldScale = yellBlock.transform.localScale;

        // Raycast to the left of the yell fire point
        if (Physics.Raycast(yellFirePoint.transform.position, transform.right * yellPrefab.transform.localScale.x / 2, out rightHit, yellPrefab.transform.localScale.x / 2))
        {
            // A wall on the right was found, scale down and reposition the block.
            float amountToScaleDown = halfLengthOfYellBlock - rightHit.distance + scaleEpsilon;
            yellBlock.transform.localScale = new Vector3(oldScale.x - amountToScaleDown, oldScale.y, oldScale.z);
            yellBlock.transform.position -= transform.right * amountToScaleDown / 2;
        }
    }
}
