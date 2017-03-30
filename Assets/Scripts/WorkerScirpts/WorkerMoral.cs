using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMoral : MonoBehaviour
{
    public int currentMoral;
    public int moralLossPerHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Yell")
        {
            currentMoral -= moralLossPerHit;
            //Debug.Log(currentMoral);
        }

        //At end of Day
            //if(currentMoral <= 0)
            //{
            //    Destroy(gameObject);
            //}
    }
}
