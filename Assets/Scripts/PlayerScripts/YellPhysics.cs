using UnityEngine;

public class YellPhysics : MonoBehaviour
{
    public float speed;
    public float yellExpansion;
    public float lengthOfYell; // in seconds

    private float centerOffsetY;
    private Rigidbody rb = null;
    private float secondsElapsed = 0;

	void Start ()
    {
        rb = this.GetComponent<Rigidbody>();
        centerOffsetY = transform.localScale.y / 2;
        transform.position = new Vector3(transform.position.x, transform.position.y + centerOffsetY, transform.position.z);
    }  
	
	void Update ()
    {
        Vector3 direction = Vector3.zero;
        Debug.Log(secondsElapsed);
        if (secondsElapsed < lengthOfYell) // the yell will stay active for lengthOfYell amount of seconds
        {
            transform.position = Vector3.Lerp(transform.position, transform.forward + transform.position, Time.deltaTime * speed);
        }
        else
        {
            
            Destroy(gameObject);
        }

        secondsElapsed += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
