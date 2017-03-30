using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Used to upate yell cooldown and time text
public class PlayerUI : MonoBehaviour
{
    public delegate void TimeThresholdHit();
    public static event TimeThresholdHit OnDayEnd;

    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private float secondsToHourRatio = 4f; // The number of real-time seconds that should map to in-game hours

    [Header("UI Components")]
    [SerializeField]
    private TextMeshProUGUI yellCooldownText;
    [SerializeField]
    private Image yellCooldownImage;
    [SerializeField]
    private TextMeshProUGUI timeText;

    private bool countingTime = false;

    private int currentHour;
    private int currentMinutes;
    private bool morning;
    private float secondsElapsedForCurrentHour;
    private float secondsToQuaterHourRatio;

    void Awake()
    {
        UpdateYellCooldown();

        // Don't use text for now, I'm not sure if I want to keep it.
        yellCooldownText.text = "";        
    }

    // This is called by the GameManager
    public void StartCountingTime(int startHour, int endHour)
    {
        // Just set starting time to 9 AM for now
        currentHour = startHour;
        currentMinutes = 0;
        morning = true;
        timeText.text = "9:00 AM";

        secondsElapsedForCurrentHour = 0f;

        // Since we are updating minute text every 15 minutes, we need this ratio
        secondsToQuaterHourRatio = secondsToHourRatio / 4f;
        countingTime = true;
    }

    void Update()
    {        
        if(countingTime)
        {
            UpdateTimeText();
        }        
     
        UpdateYellCooldown();                
    }

    private void UpdateTimeText()
    {
        secondsElapsedForCurrentHour += Time.deltaTime;
        
        // Update hour
        if(secondsElapsedForCurrentHour >= secondsToHourRatio)
        {
            currentHour++;
            secondsElapsedForCurrentHour = 0f;
        }

        string minutesText = "";
                 
        // Update minutes
        if(secondsElapsedForCurrentHour < secondsToQuaterHourRatio) // 0 - 14 minutes
        {
            minutesText = "00";
        }
        else if (secondsElapsedForCurrentHour >= secondsToQuaterHourRatio && secondsElapsedForCurrentHour < secondsToQuaterHourRatio * 2) // 15 - 29 minutes
        {
            minutesText = "15";
        }
        else if (secondsElapsedForCurrentHour >= secondsToQuaterHourRatio * 2 && secondsElapsedForCurrentHour < secondsToQuaterHourRatio * 3) // 30 - 44 minutes
        {
            minutesText = "30";
        }
        else // 45 - 59 minutes
        {
            minutesText = "45";
        }

        // @TODO(Colin): AM/PM switch should happen at 11:59, not 12:00
        if (currentHour > 12)
        {
            currentHour = 1;
            morning = !morning;
        }

        // Update text
        timeText.text = currentHour + ":" + minutesText + (morning ? " AM" : " PM");       
    }

    private void UpdateYellCooldown()
    {
        yellCooldownImage.fillAmount = (playerController.yellCooldown - playerController.yellCooldownTimeLeft) / playerController.yellCooldown;
        //yellCooldownText.text = (int)Mathf.Clamp(playerController.yellCooldownTimeLeft, 0, playerController.yellCooldownTimeLeft) + "";
    }
}
