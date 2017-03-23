using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public float turnSpeed;
    public int lengthOfYell;
    public GameObject yell;
    Vector3 yellOffset = new Vector3(0, 7, 0);
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
        Quaternion rot = t.rotation;

        // Standard movement stuff, use force to move so he doesn't phase through walls
        if (Input.GetKey(KeyCode.W))
        {
            r.AddForce(t.forward * speed, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rot *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime * -1f, Vector3.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            r.AddForce(t.forward * speed * -1f, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rot *= Quaternion.AngleAxis(turnSpeed * Time.deltaTime * 1f, Vector3.up);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (x == 0) // only yell if no other yell is active
            { 
                GameObject yellBlock = Instantiate(yell, t.position + yellOffset,
                                        t.localRotation) as GameObject;
                x++;
            }
        }
        if(x != 0) // counts frames if there is an active yell
        {
            x++;
            if(x == lengthOfYell) // allows for another yell if frame count is hit
            {
                x = 0;
            }
        }

        t.position += direction * Time.deltaTime * speed;
        t.localRotation = rot;
    }
}
