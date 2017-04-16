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
    private bool isMorning;
    private float secondsElapsedForCurrentHour;
    private float secondsToQuaterHourRatio;

    private float endOfDayHour;

    void Awake()
    {
        UpdateYellCooldown();

        // Don't use text for now, I'm not sure if I want to keep it.
        yellCooldownText.text = "";        
    }

    // This is called by the GameManager
    public void StartCountingTime(int startHour, int endHour)
    {
        endOfDayHour = endHour;

        currentHour = startHour;
        currentMinutes = 0;
        isMorning = true;
        timeText.text = currentHour + ":00 AM";

        secondsElapsedForCurrentHour = 0f;

        // Since we are updating minute text every 15 minutes, we need this ratio
        secondsToQuaterHourRatio = JobManager.Instance.jobBlueprint.secondsToHourRatio / 4f;
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
        if(secondsElapsedForCurrentHour >= JobManager.Instance.jobBlueprint.secondsToHourRatio)
        {
            currentHour++;

            // @TODO(Colin): AM/PM switch should happen at 11:59, not 12:00 // could also just use military time :D
            if (currentHour == 12)
            {
                isMorning = !isMorning;
            }
            else if (currentHour > 12)
            {
                currentHour = 1;
            }

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




        // Update text
        timeText.text = currentHour + ":" + minutesText + (isMorning ? " AM" : " PM");

        // We reached the end of the day. Evoke the event 
        if (!isMorning && !(currentHour > 10) && currentHour >= endOfDayHour)
        {
            if (OnDayEnd != null)
            {
                OnDayEnd();
            }

            countingTime = false;
        }
    }

    private void UpdateYellCooldown()
    {
        yellCooldownImage.fillAmount = (playerController.yellCooldown - playerController.yellCooldownTimeLeft) / playerController.yellCooldown;
        //yellCooldownText.text = (int)Mathf.Clamp(playerController.yellCooldownTimeLeft, 0, playerController.yellCooldownTimeLeft) + "";
    }
}
