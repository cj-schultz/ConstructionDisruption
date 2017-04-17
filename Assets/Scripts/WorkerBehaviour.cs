using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(WorkerMovement))]
public class WorkerBehaviour : MonoBehaviour
{
    public float maxMoral = 200;
    public float moralLostOnHit = 1;
    public GameObject ui;
    public Image moralBar;
    public SkinnedMeshRenderer[] meshes;
    public Material enragedWorkerMaterial;

    private Rigidbody rb;
    private WorkerMovement workerMovement;

    private Material originalMaterial;

    private GameObject player;

    private float currentMoral;
    private bool lostAllMoral;    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        workerMovement = GetComponent<WorkerMovement>();

        originalMaterial = meshes[0].material;        

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

        if (enraged)
        {
            enrageTimer += Time.deltaTime;
            if (enrageTimer >= enrageLength)
            {
                workerMovement.DeEnrage();
                currentSuppresionLength = 0;
                lastHitTime = 0;
                enraged = false;

                for (int i = 0; i < meshes.Length; i++)
                {
                    meshes[i].material = originalMaterial;
                }
            }
        }
    }

    // @TODO(colin): Make these editor-facing
    private float currentSuppresionLength;
    private float lastHitTime = 0;

    private float suppresseionToleranceAmount = 3f;
    private float suppresionLengthUntilEnrage = 4;

    private float enrageTimer;
    private float enrageLength = 3f;
    private bool enraged = false;

    public void HitByYell(Vector3 force)
    {                                  
        // Only allow the worker to be pushed around if hes not enraged
        if(!enraged)
        {
            // Enrage stuff
            if (Time.time - lastHitTime < suppresseionToleranceAmount)
            {
                currentSuppresionLength += Time.time - lastHitTime;
            }
            else
            {
                currentSuppresionLength = 0;
            }

            if (currentSuppresionLength >= suppresionLengthUntilEnrage)
            {
                enraged = true;
                enrageTimer = 0;
                workerMovement.StartEnragedMovement();

                for (int i = 0; i < meshes.Length; i++)
                {
                    meshes[i].material = enragedWorkerMaterial;
                }
            }
            lastHitTime = Time.time;

            // Hit physics
            rb.AddForce(force, ForceMode.Force);

            // Moral stuff
            if (!lostAllMoral)
            {
                currentMoral -= moralLostOnHit;

                if (currentMoral <= 0)
                {
                    lostAllMoral = true;
                    currentMoral = 0;

                    // Set the moral bar to all red to indicated this guy lost hope
                    moralBar.fillAmount = 1f;
                    moralBar.color = Color.black;

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
}
