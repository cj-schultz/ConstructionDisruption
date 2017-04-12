using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    SceneFader sceneFader;

    [SerializeField]
    private TextMeshProUGUI jobText;
    [SerializeField]
    private TextMeshProUGUI dayText;

    void Start()
    {
        // Add + 1 because the indexes are based at 0
        jobText.text = "Job " + (JobManager.CurrentGameState.currentJobIndex + 1);

        if(JobManager.CurrentGameState.currentDayIndex >= JobManager.Instance.jobBlueprint.numOfDays)
        {
            dayText.text = "Day " + (JobManager.CurrentGameState.currentDayIndex) + "\nJob Finished!";
        }
        else
        {
            // We don't add plus one, because we already incremented the current day index in the Job Manager before this script is called
            dayText.text = "Day " + (JobManager.CurrentGameState.currentDayIndex);
        }        
    }

    public void StartNextDay()
    {
		// @Note(colin): This is assuming we are loading the same job as last time
		sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
}
