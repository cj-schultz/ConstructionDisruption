using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Used to upate yell cooldown and time text
public class PlayerUI : MonoBehaviour
{
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

    private int currentHour;
    private int currentMinutes;
    private bool morning;
    private float secondsElapsedForCurrentHour;

    void Start()
    {
        UpdateYellCooldown();

        // Don't use text for now, I'm not sure if I want to keep it.
        yellCooldownText.text = "";

        // Just set starting time to 9 AM for now
        currentHour = 9;
        currentMinutes = 0;
        morning = true;
        timeText.text = "9:00 AM";

        secondsElapsedForCurrentHour = 0f;
    }

    void Update()
    {        
        UpdateTimeText();
     
        UpdateYellCooldown();                
    }

    private void UpdateTimeText()
    {
        secondsElapsedForCurrentHour += Time.deltaTime;
        
        if(secondsElapsedForCurrentHour >= secondsToHourRatio)
        {
            currentHour++;
            secondsElapsedForCurrentHour = 0f;
        }                

        // @TODO(Colin): AM/PM switch should happen at 11:59, not 12:00
        if(currentHour > 12)
        {
            currentHour = 1;
            morning = !morning;
        }

        if(currentMinutes < 10)
        {
            timeText.text = currentHour + ":0" + currentMinutes + (morning ? " AM" : " PM");
        }
        else
        {
            timeText.text = currentHour + ":" + currentMinutes + (morning ? " AM" : " PM");
        }        
    }

    private void UpdateYellCooldown()
    {
        yellCooldownImage.fillAmount = (playerController.yellCooldown - playerController.yellCooldownTimeLeft) / playerController.yellCooldown;
        //yellCooldownText.text = (int)Mathf.Clamp(playerController.yellCooldownTimeLeft, 0, playerController.yellCooldownTimeLeft) + "";
    }
}
