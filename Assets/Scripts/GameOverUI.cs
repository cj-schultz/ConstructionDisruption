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

    void Start()
    {
        // Add + 1 because the indexes are based at 0
        jobText.text = "Job " + (GameManager.CurrentGameState.currentJobIndex + 1);
        // We don't add plus one, because we already incremented the current day index in the Game Manager before this script is called
        dayText.text = "Day " + (GameManager.CurrentGameState.currentDayIndex);
    }

    public void StartNextDay()
    {
        sceneFader.FadeTo("Level1");
    }
}
