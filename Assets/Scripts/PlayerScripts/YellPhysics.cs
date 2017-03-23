using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellPhysics : MonoBehaviour
{
    public GameObject yellBlock;
    public float speed;
    public float yellExpansion;
    public int lengthOfYell;
    private Transform t = null;
    private Rigidbody r = null;
    int x = 0; // frame counter

	void Start ()
    {
        t = this.GetComponent<Transform>();
        r = this.GetComponent<Rigidbody>();
    }  
	
	void Update ()
    {
        Vector3 direction = Vector3.zero;
        if (x < lengthOfYell) // the yell will stay active for lengthOfYell amount of frames
        {
            r.AddForce(t.forward * speed, ForceMode.VelocityChange);
            yellBlock.transform.localScale += new Vector3(yellExpansion, yellExpansion, 0);
        }
        else
        {
            Destroy(yellBlock);
        }
        t.position += direction * Time.deltaTime * speed;
        x++;
	}

    private void OnCollisionEnter(Collision collision)
    {
        /*
          if yell hits worker
            use addforce to puch him back
          otherwise
            stop the yell
            so you can't yell through walls
        */
    }
}
