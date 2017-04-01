using UnityEngine;

public class YellBeam : MonoBehaviour
{
    public Vector3 origin;    
    public float growSpeed = 10;

    void Update()
    {
        transform.localScale += new Vector3(0, 0, Time.deltaTime * growSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player") Destroy(transform.gameObject);
    }
}
