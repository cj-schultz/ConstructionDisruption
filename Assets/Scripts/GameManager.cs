using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState CurrentGameState;

    [Header("Component references")]
    [SerializeField]
    private SceneFader sceneFader;
    [SerializeField]
    private PlayerUI playerUI;
    [SerializeField]
    private GameObject gameOverUI;

    [Header("Game Variables")]
    // The number of real-time seconds that should map to in-game hours
    public float secondsToHourRatio = 4f;
    // The hour the current day should start at
    // Note: This is always assumed to be AM
    [SerializeField]
    private int startHour; 
    // The hour that the current day should end at
    // Note: This is always assumed to be AM
    [SerializeField]
    private int endHour;
    
    void Awake()
    {
        gameOverUI.SetActive(false);

        if(CurrentGameState == null)
        {
            CurrentGameState = new GameState();
        }        

        // Start the day
        playerUI.StartCountingTime(startHour, endHour);
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

        //sceneFader.FadeTo("MainMenu");
    }
}
