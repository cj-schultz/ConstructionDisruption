using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Component references")]
    [SerializeField]
    private SceneFader sceneFader;
    [SerializeField]
    private PlayerUI playerUI;

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
        sceneFader.FadeTo("MainMenu");
    }
}
