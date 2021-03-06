﻿using System.Collections.Generic;
using UnityEngine;

public class Yell : MonoBehaviour
{
    public float speed;
    public float yellExpansion;
    public float yellStrength;
    public float lengthOfYell; // in seconds

    public bool oneHitWonder = true; // is true when it should only hit a given worker once

    public LayerMask wallClippers;

    private float centerOffsetY;    
    private float secondsElapsed = 0;

    private Rigidbody rb;
    private MeshRenderer mesh;

    private List<GameObject> hitEnemies;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponentInChildren<MeshRenderer>();

        centerOffsetY = transform.localScale.y / 2;
        transform.position = new Vector3(transform.position.x, transform.position.y + centerOffsetY, transform.position.z);

        hitEnemies = new List<GameObject>();
    }
    
    void Update()
    {
        if (Time.timeScale != 0) {
            // Calculate the amount of movement the yell block will do this frame
            Vector3 deltaPosition = transform.forward * Time.deltaTime * speed;
            gameObject.transform.localScale += new Vector3(yellExpansion, yellExpansion, 0);

            //
            // Yell block clipping
            //        
            float maxDistance = 5f;
            float scaleEpsilon = .5f;
            float rayEpsilon = .05f;

            Vector3 center = transform.position;
            Vector3 halfExtents = transform.localScale / 2;
            Vector3 boxcastDir = transform.forward;
            Quaternion orientation = transform.localRotation;

            RaycastHit[] hitsInfo;
            hitsInfo = Physics.BoxCastAll(center, halfExtents, boxcastDir, orientation, maxDistance, wallClippers);            
            //ExtDebug.DrawBoxCastBox(center, halfExtents, orientation, boxcastDir, maxDistance, Color.red);

            if (hitsInfo.Length > 0)
            {
                //ExtDebug.DrawBoxCastOnHit(center, halfExtents, orientation, boxcastDir, hitsInfo[0].distance, Color.magenta);

                // @TODO(colin): Account for multiple hit points, so it can squeeze through a doorway or something
                //for (int i = 0; i < hitsInfo.Length; i++)
                for (int i = 0; i < 1; i++)
                {
                    RaycastHit hitInfo;

                    bool collisionPointFoundOnYellBlock = false;

                    // Try to find the point on the yell block that will be colliding. We need 3 rays to test against, one
                    // coming out of the exact predicted contact point, and two other ones with padding on either side.
                    //Debug.DrawRay(hitsInfo[i].point, -transform.forward * maxDistance, Color.black);
                    Ray exactRay = new Ray(hitsInfo[i].point, -transform.forward * maxDistance);
                    collisionPointFoundOnYellBlock = Physics.Raycast(exactRay, out hitInfo, maxDistance);                    

                    if(!collisionPointFoundOnYellBlock || (collisionPointFoundOnYellBlock && hitInfo.transform != transform))
                    {
                        //Debug.DrawRay(hitsInfo[i].point + -transform.right * rayEpsilon, -transform.forward * maxDistance, Color.cyan);
                        Ray leftPadRay = new Ray(hitsInfo[i].point + -transform.right * rayEpsilon, -transform.forward * maxDistance);
                        collisionPointFoundOnYellBlock = Physics.Raycast(leftPadRay, out hitInfo, maxDistance);
                    }

                    if (!collisionPointFoundOnYellBlock || (collisionPointFoundOnYellBlock && hitInfo.transform != transform))
                    {
                        //Debug.DrawRay(hitsInfo[i].point + transform.right * rayEpsilon, -transform.forward * maxDistance, Color.yellow);
                        Ray rightPadRay = new Ray(hitsInfo[i].point + transform.right * rayEpsilon, -transform.forward * maxDistance);
                        collisionPointFoundOnYellBlock = Physics.Raycast(rightPadRay, out hitInfo, maxDistance);
                    }

                    // If we found the collision on the yell block, proceed
                    if (collisionPointFoundOnYellBlock && hitInfo.transform == transform)
                    {
                        // Find the center of the wall with the same height as the collision point
                        Vector3 centerOfWall = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z) + transform.forward * halfExtents.z;

                        // Draws the triangle of god, note: draws one frame late
                        Debug.DrawLine(hitsInfo[i].point, hitInfo.point, Color.yellow);
                        Debug.DrawLine(hitsInfo[i].point, centerOfWall, Color.cyan);
                        Debug.DrawLine(centerOfWall, hitInfo.point, Color.blue);

                        // Only do the transformations if the wall would have hit 5 frames from now
                        if (hitsInfo[0].distance <= Vector3.Distance(transform.position, transform.position + deltaPosition * 5))
                        {
                            // Find the amount we need to scale down based on where the yell block will collide                        
                            float amountToScaleDown = halfExtents.x - Vector3.Distance(centerOfWall, hitInfo.point) + scaleEpsilon;

                            // Do the scale down
                            transform.localScale = new Vector3(transform.localScale.x - amountToScaleDown, transform.localScale.y, transform.localScale.z);

                            // Find the normalized vector of the difference between the collision hit point and the center of the wall
                            Vector3 differenceDir = (hitInfo.point - centerOfWall).normalized;

                            // Shift the wall left or right, depending on the differenceDir
                            if (differenceDir == transform.right.normalized)
                            {
                                transform.position -= transform.right * amountToScaleDown / 2;
                            }
                            else
                            {
                                transform.position += transform.right * amountToScaleDown / 2;
                            }
                        }
                    }
                }
            }
            //
            //
            //

            Vector3 direction = Vector3.zero;

            // Fade out the material over time
            Color c = mesh.material.color;
            c.a = 1 - (secondsElapsed / lengthOfYell);
            mesh.material.color = c;

            if (secondsElapsed < lengthOfYell) // the yell will stay active for lengthOfYell amount of seconds
            {
                transform.position += deltaPosition;
            }
            else
            {
                Destroy(gameObject);
            }

            secondsElapsed += Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if (oneHitWonder)
            {
                if(!hitEnemies.Contains(other.gameObject))
                {
                    hitEnemies.Add(other.gameObject);
                    other.GetComponent<WorkerBehaviour>().HitByYell(transform.forward * yellStrength);
                }
            }
            else
            {
                other.GetComponent<WorkerBehaviour>().HitByYell(transform.forward * yellStrength);
            }            
        }
        else if(other.tag != "Yell")
        {
            Destroy(gameObject);
        }
    }
}
