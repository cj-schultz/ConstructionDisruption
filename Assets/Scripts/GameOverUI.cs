using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    SceneFader sceneFader;

    [SerializeField]
    private TextMeshProUGUI jobText;
    [SerializeField]
    private TextMeshProUGUI dayText;
    
    public void Setup(bool finishedLastDayOfJob)
    {
        string dayString = "";
        if (JobManager.CurrentGameState.currentDayNumber == 1)
        {
            if(finishedLastDayOfJob)
            {
                dayString = "Day " + JobManager.Instance.jobBlueprint.numOfDays + " / " + JobManager.Instance.jobBlueprint.numOfDays;
            }
            else
            {
                dayString = "Day 1 / " + JobManager.Instance.jobBlueprint.numOfDays;
            }            
        }
        else
        {
            dayString = "Day " + (JobManager.CurrentGameState.currentDayNumber - 1) + " / " + JobManager.Instance.jobBlueprint.numOfDays;
        }


        if (finishedLastDayOfJob)
        {
            jobText.text = "Job " + (JobManager.CurrentGameState.currentJobNumber - 1);            
            dayText.text = dayString;
        }
        else
        {
            jobText.text = "Job " + (JobManager.CurrentGameState.currentJobNumber);
            dayText.text = dayString;
        }        
    }

    public void StartNextDay()
    {
		// @Note(colin): This is assuming we are loading the same job as last time
		sceneFader.FadeTo("Job" + JobManager.CurrentGameState.currentJobNumber);
    }
}
