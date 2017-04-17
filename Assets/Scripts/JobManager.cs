using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance;

    public static int NUMBER_OF_JOBS = 3;
    public static GameState CurrentGameState;
    public static string EXE_GAME_STATE_DISK_PATH = "/exe_saved_user_info.dat";
    public static string EDITOR_GAME_STATE_DISK_PATH = "/editor_saved_user_info.dat";

    public GameObject workerPrefab;
    public GameObject[] spawnPoints;    

    [Header("HOLY SHIT IT'S A JOB BLUEPRINT")]
    public JobBlueprint jobBlueprint;        

    [Header("Component references")]
    public SceneFader sceneFader;
    public PlayerUI playerUI;
    public GameOverUI gameOverUI;

    private GameObject[] resources;
    private GameObject[] foundations;

    private int workersDemoralized;

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

        resources = GameObject.FindGameObjectsWithTag("Resource");
        foundations = GameObject.FindGameObjectsWithTag("Foundation");

        StartDay();
    }

    void OnEnable()
    {
        PlayerUI.OnDayEnd += HandleDayEnd;
        FoundationHandler.OnFoundationCompleted += HandleDayEnd;
    }

    void OnDisable()
    {
        PlayerUI.OnDayEnd -= HandleDayEnd;
        FoundationHandler.OnFoundationCompleted -= HandleDayEnd;
    }

    public void StartDay()
    {        
        gameOverUI.gameObject.SetActive(false);
        if (CurrentGameState == null)
        {
            CurrentGameState = new GameState();
        }

        workersDemoralized = 0;

        // Clear the current worker count and the fill bar amount if it is the first day of a new job
        if (CurrentGameState.currentDayNumber == 1)
        {
            CurrentGameState.currentWorkerCount = 0;
            CurrentGameState.currentJobFoundationCompletion = 0;
        }

        // Spawn Enemies
        List<GameObject> availableSpawnPoints = new List<GameObject>(spawnPoints);
        int enemiesToSpawn = CurrentGameState.currentWorkerCount + jobBlueprint.workersToAddEveryDay;
        CurrentGameState.currentWorkerCount = enemiesToSpawn;     
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Just incase we have more workers than spawn points
            if(availableSpawnPoints.Count == 0)
            {
                availableSpawnPoints = new List<GameObject>(spawnPoints);
            }

            // Pick a spawn point
            GameObject spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            availableSpawnPoints.Remove(spawnPoint);

            // Spawn the worker and assign values
            WorkerMovement worker = Instantiate(workerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<WorkerMovement>();
            worker.targetResource = GetClosestResource(worker.transform.position);
            worker.targetFoundation = foundations[Random.Range(0, foundations.Length)];
        }

        AudioManager.Instance.PlayGameLoopMusic();
        
        // Start the day
        playerUI.StartCountingTime(jobBlueprint.startHour, jobBlueprint.endHour);
    }

    public GameObject GetClosestResource(Vector3 posToCheck)
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestResource = null;

        for (int i = 0; i < resources.Length; i++)
        {
            float dist = Vector3.Distance(posToCheck, resources[i].transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestResource = resources[i];
            }
        }

        return closestResource;
    }

    public GameObject GetRandomResource()
    {
        return resources[Random.Range(0, resources.Length)];
    }

    public void EnemyLostMoral()
    {
        workersDemoralized++;
        CurrentGameState.currentWorkerCount--;
    }

    private void HandleDayEnd()
    {
        bool finishedLastDayOfJob = false;
        int previousDayNumber = CurrentGameState.currentDayNumber;

        // Update game state        
        if (CurrentGameState.currentJobFoundationCompletion < 1)
        {
            if (CurrentGameState.currentDayNumber >= jobBlueprint.numOfDays)
            {
                CurrentGameState.currentJobNumber++;
                CurrentGameState.currentDayNumber = 1;
                finishedLastDayOfJob = true;
            }
            else
            {
                CurrentGameState.currentDayNumber++;
            }
        }
        else
        {
            // @Note(colin): If the foundation gets completed, just go to the next day
            CurrentGameState.currentJobNumber++;
            CurrentGameState.currentDayNumber = 1;
        }                

        if(CurrentGameState.currentJobNumber > NUMBER_OF_JOBS)
        {
            DeleteGameStateFromDisk();
        }

        gameOverUI.gameObject.SetActive(true);
        Time.timeScale = 0; // Stop background activity while the game over UI is up
        gameOverUI.Setup(previousDayNumber, finishedLastDayOfJob, workersDemoralized);
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

#if UNITY_EDITOR
        string path = Application.persistentDataPath + JobManager.EDITOR_GAME_STATE_DISK_PATH;
#else
        string path = Application.persistentDataPath + JobManager.EXE_GAME_STATE_DISK_PATH;
#endif

        Stream file = File.Open(path, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(file, CurrentGameState);

        file.Close();
    }

    private void DeleteGameStateFromDisk()
    {
#if UNITY_EDITOR
        string path = Application.persistentDataPath + JobManager.EDITOR_GAME_STATE_DISK_PATH;
#else
        string path = Application.persistentDataPath + JobManager.EXE_GAME_STATE_DISK_PATH;
#endif
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
