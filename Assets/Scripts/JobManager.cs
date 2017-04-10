using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance;
    public static GameState CurrentGameState;

    [Header("HOLY SHIT IT'S A JOB BLUEPRINT")]
    public JobBlueprint jobBlueprint;        

    [Header("Component references")]
    [SerializeField]
    private SceneFader sceneFader;
    [SerializeField]
    private PlayerUI playerUI;
    [SerializeField]
    private GameObject gameOverUI;

    void Awake()
    {
        // @Note(colin): This is a singleton, google it or something
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            enabled = false;
            return;
        }
        else if (Instance == null)
        {
            Instance = this;
        }            

        gameOverUI.SetActive(false);

        if(CurrentGameState == null)
        {
            CurrentGameState = new GameState();
        }        

        // Start the day
        playerUI.StartCountingTime(jobBlueprint.startHour, jobBlueprint.endHour);
    }

    void OnEnable()
    {
        PlayerUI.OnDayEnd += HandleDayEnd;
    }

    void OnDisable()
    {
        PlayerUI.OnDayEnd -= HandleDayEnd;
    }

    private void HandleDayEnd()
    {
        // Update game state
        CurrentGameState.currentDayIndex++;

        gameOverUI.SetActive(true);
    }
}
