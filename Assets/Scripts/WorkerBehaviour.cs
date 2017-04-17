using UnityEngine;
using UnityEngine.UI;

public class WorkerBehaviour : MonoBehaviour
{
    public float maxMoral = 200;
    public float moralLostOnHit = 1;
    public GameObject ui;
    public Image moralBar;

    private Rigidbody rb;
    private GameObject player;

    private float currentMoral;
    private bool lostAllMoral;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");

        currentMoral = maxMoral;
        lostAllMoral = false;
        moralBar.fillAmount = 1;
    }

    void Update()
    {
        if (player)
        {
            ui.transform.LookAt(player.transform);
            float yrot = ui.transform.eulerAngles.y;
            ui.transform.eulerAngles = Vector3.zero;
            ui.transform.eulerAngles += Vector3.up * yrot;
        }
    }

    public void HitByYell(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Force);

        if(!lostAllMoral)
        {
            currentMoral -= moralLostOnHit;

            if(currentMoral <= 0)
            {
                lostAllMoral = true;
                currentMoral = 0;

                // Set the moral bar to all red to indicated this guy lost hope
                moralBar.fillAmount = 1f;
                moralBar.color = Color.red;

                // Send event to job manager
                JobManager.Instance.EnemyLostMoral();
            }
            else
            {
                // Update UI
                moralBar.fillAmount = currentMoral / maxMoral;
                moralBar.color = Color.Lerp(moralBar.color, Color.red, moralLostOnHit / maxMoral);
            }            
        }        
    }
}
