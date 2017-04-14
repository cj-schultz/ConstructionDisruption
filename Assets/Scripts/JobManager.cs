using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance;

    public static GameState CurrentGameState;
    public static string GAME_STATE_DISK_PATH = "/saved_user_info.dat";

    [Header("HOLY SHIT IT'S A JOB BLUEPRINT")]
    public JobBlueprint jobBlueprint;        

    [Header("Component references")]
    [SerializeField]
    private SceneFader sceneFader;
    [SerializeField]
    private PlayerUI playerUI;
    [SerializeField]
    private GameOverUI gameOverUI;

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

        gameOverUI.gameObject.SetActive(false);

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
        bool finishedLastDayOfJob = false;

        // Update game state        
        if(CurrentGameState.currentDayNumber >= jobBlueprint.numOfDays)
        {
            CurrentGameState.currentJobNumber++;
            CurrentGameState.currentDayNumber = 1;
            finishedLastDayOfJob = true;
        }
        else
        {
            CurrentGameState.currentDayNumber++;
        }
        
        gameOverUI.gameObject.SetActive(true);
        gameOverUI.Setup(finishedLastDayOfJob);
    }

    void OnApplicationQuit()
    {
        // Save state
        WriteGameStateToDisk();
    }

    private void WriteGameStateToDisk()
    {
        if (CurrentGameState == null)
        {
            return;
        }

        string path = Application.persistentDataPath + JobManager.GAME_STATE_DISK_PATH;
        Stream file = File.Open(path, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(file, CurrentGameState);

        file.Close();
    }

}
