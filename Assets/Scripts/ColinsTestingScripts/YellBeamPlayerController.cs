using UnityEngine;

public class YellBeamPlayerController : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public GameObject yellPrefab;
    public Transform yellFirePoint;

    public float yellCooldown = 2; // in seconds
    // Make this public to read, but privite to write
    public float yellCooldownTimeLeft { get; private set; }

    private Transform t = null;
    private Rigidbody r = null;

    void Awake()
    {
        t = this.GetComponent<Transform>();
        r = this.GetComponent<Rigidbody>();
        yellCooldownTimeLeft = 0f;
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;
        Quaternion rot = t.rotation;

        if (yellCooldownTimeLeft >= 0)
        {
            yellCooldownTimeLeft -= Time.deltaTime;
        }

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
            // Yell if the cooldown is up
            if (yellCooldownTimeLeft <= 0)
            {
                r.AddForce(-transform.forward*2f, ForceMode.Impulse);
                GameObject yellBlock = Instantiate(yellPrefab, yellFirePoint.position, t.localRotation) as GameObject;
                yellBlock.GetComponent<YellBeam>().origin = yellFirePoint.position;
                yellCooldownTimeLeft = yellCooldown;
            }
        }

        t.position += direction * Time.deltaTime * speed;
        t.localRotation = rot;
    }
}
