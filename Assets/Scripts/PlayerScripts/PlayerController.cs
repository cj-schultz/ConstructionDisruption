﻿using UnityEngine;

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

    void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        yellCooldownTimeLeft = 0f;
    }
	
	void Update ()
    {
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
        if (Input.GetKey(KeyCode.Space))
        {
            // Yell if the cooldown is up
            if (yellCooldownTimeLeft <= 0)
            {
                // Note(colin): This pushes back the player a little when they yell. Just testing the feel of this.
                rb.AddForce(-transform.forward * 30f, ForceMode.Impulse);

                GameObject yellBlock = Instantiate(yellPrefab, yellFirePoint.position, transform.localRotation) as GameObject;
                yellCooldownTimeLeft = yellCooldown;
            }
        }
        
        transform.position += direction * Time.deltaTime * speed;
        transform.localRotation = rot;
    }
}
