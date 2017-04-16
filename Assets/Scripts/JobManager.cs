using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance;

    public static GameState CurrentGameState;
    public static string GAME_STATE_DISK_PATH = "/saved_user_info.dat";

    public GameObject enemyPrefab;
    public GameObject[] spawnPoints;    

    [Header("HOLY SHIT IT'S A JOB BLUEPRINT")]
    public JobBlueprint jobBlueprint;        

    [Header("Component references")]
    public SceneFader sceneFader;
    public PlayerUI playerUI;
    public GameOverUI gameOverUI;

    private GameObject[] resources;
    private GameObject[] foundations;

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
    }

    void OnDisable()
    {
        PlayerUI.OnDayEnd -= HandleDayEnd;
    }

    public void StartDay()
    {
        List<GameObject> availableSpawnPoints = new List<GameObject>(spawnPoints);

        // Spawn Enemies
        int enemiesToSpawn = 4;
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
            WorkerAI worker = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<WorkerAI>();
            worker.targetResource = resources[Random.Range(0, resources.Length)];
            worker.targetFoundation = foundations[Random.Range(0, foundations.Length)];
        }     

        gameOverUI.gameObject.SetActive(false);

        if (CurrentGameState == null)
        {
            CurrentGameState = new GameState();
        }

        // Start the day
        playerUI.StartCountingTime(jobBlueprint.startHour, jobBlueprint.endHour);
    }

    public GameObject GetRandomResource()
    {
        return resources[Random.Range(0, resources.Length)];
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
        Time.timeScale = 0; // added this, testing
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
