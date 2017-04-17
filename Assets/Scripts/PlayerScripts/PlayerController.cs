using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float turnSpeed;   
    public GameObject yellPrefab;
    public Transform yellFirePoint;

    public float rechargeDelay = 0.5f;
    public float rechargeRate = 0.5f;
    public float maxYellSeconds = 2f;

    public float coughDropMultiplier = 2f;

    [HideInInspector]
    public float yellSecondsLeft = 2f;

    private bool yelling = false;
    private bool canYell = true;
    private float timeYellingStopped;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    private Vector3 rayDirectionLeft;
    private Vector3 rayDirectionRight;

    // Run stuff
    public float runCooldown = 4f;    
    public float runMultiplier = 2f;
    public float runLength = 2f;
    [HideInInspector]
    public float currentRunCooldown;

    private bool runEnabled = false;
    private bool canRunRightNow = false;    

    void Awake ()
    {        
        rb = GetComponent<Rigidbody>();    

        boxCollider = GetComponent<BoxCollider>();
        
        rayDirectionLeft = -transform.right * yellPrefab.transform.localScale.x / 2;
        rayDirectionRight = transform.right * yellPrefab.transform.localScale.x / 2;

        yellSecondsLeft = maxYellSeconds;

        // Apply cough drop item if we have it
        if (JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.CoughDrop)] > 0)
        {
            rechargeRate *= coughDropMultiplier;
            rechargeDelay /= coughDropMultiplier;
        }

        // Allow running if we have em
        if (JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.Yeezys)] > 0)
        {
            runEnabled = true;
            canRunRightNow = true;
            currentRunCooldown = runCooldown;
        }
    }
        
	void Update ()
    {
        //
        // DEBUG DRAWING
        //
#if false
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
#endif
        //
        //
        //

        Vector3 direction = Vector3.zero;
        Quaternion rot = transform.rotation;

        float speedMulti = 1;

        // If we are currently running
        if (runEnabled && !canRunRightNow)
        {
            currentRunCooldown -= Time.deltaTime;

            if(runCooldown - currentRunCooldown <= runLength)
            {
                speedMulti = runMultiplier;
            }

            if(currentRunCooldown <= 0)
            {
                canRunRightNow = true;
                currentRunCooldown = runCooldown;
            }
        }
                
        if (canRunRightNow && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            speedMulti = runMultiplier;
            canRunRightNow = false;
            currentRunCooldown = runCooldown;
        }
        
        // Standard movement stuff, use force to move so he doesn't phase through walls
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.forward * speed * speedMulti, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rot *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime * -1f, Vector3.up);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-transform.forward * speed * speedMulti, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rot *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime * 1f, Vector3.up);
        }
        if (!yelling)
        {
            canYell = yellSecondsLeft > 0;

            if((Time.time - timeYellingStopped) > rechargeDelay)
            {
                yellSecondsLeft += Time.deltaTime * rechargeRate;
                yellSecondsLeft = Mathf.Clamp(yellSecondsLeft, 0, maxYellSeconds);
            }            

            if (canYell && Input.GetKeyDown(KeyCode.Space))
            {
                yelling = true;
                StartCoroutine("DoYelling");
            }
        }        
        else
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                yelling = false;
                timeYellingStopped = Time.time;
                StopCoroutine("DoYelling");
            }           
            else
            {
                yellSecondsLeft -= Time.deltaTime;
                
                if (yellSecondsLeft <= 0)
                {
                    yellSecondsLeft = 0;
                    yelling = false;
                    timeYellingStopped = Time.time;
                    StopCoroutine("DoYelling");
                }
            }
        }

        transform.position += direction * Time.deltaTime * speed;
        transform.localRotation = rot;
    }

    private IEnumerator DoYelling()
    {        
        while (true)
        {
            GameObject yellBlock = Instantiate(yellPrefab, yellFirePoint.position, transform.localRotation) as GameObject;
            AdjustYellInitialScale(yellBlock);
     
            yield return new WaitForSeconds(.05f);
        }
    }

    // Spawns and clips the yell block
    private void Yell()
    {
        // Note(colin): This pushes back the player a little when they yell. Just testing the feel of this.
        //rb.AddForce(-transform.forward * 20f, ForceMode.Impulse);

        GameObject yellBlock = Instantiate(yellPrefab, yellFirePoint.position, transform.localRotation) as GameObject;

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
