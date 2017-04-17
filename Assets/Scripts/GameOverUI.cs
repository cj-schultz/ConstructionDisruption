using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public SceneFader sceneFader;

    public TextMeshProUGUI jobText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI savingsMoneyText;
    public TextMeshProUGUI dailyPayMoneyText;
    public TextMeshProUGUI employeeSalaryMoneyText;
    public TextMeshProUGUI employeeSalaryCountText;
    public TextMeshProUGUI netMoneyText;
    public TextMeshProUGUI workerQuitMessageText;
    public TextMeshProUGUI bossMessageText;
    public TextMeshProUGUI startNextButtonText;

    private bool finishedLastDayOfJob;

    public void Setup(bool _finishedLastDayOfJob, int workersDemoralized)
    {
        finishedLastDayOfJob = _finishedLastDayOfJob;

        // Day text
        string dayString = "";
        if (JobManager.CurrentGameState.currentDayNumber == 1)
        {
            if (finishedLastDayOfJob)
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

        // Job text
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

        // @TODO: Take out the hard coded base employee salary
        int savings = JobManager.CurrentGameState.currentMoney;
        int dailyPay = JobManager.Instance.jobBlueprint.dailyBaseSalary;
        int employeeSalary = 200 * JobManager.CurrentGameState.currentWorkerCount;

        int net = savings + dailyPay - employeeSalary;
        JobManager.CurrentGameState.currentMoney = net;

        // Summary text
        savingsMoneyText.text = "$" + savings;
        dailyPayMoneyText.text = "$" + dailyPay;
        employeeSalaryMoneyText.text = "$" + employeeSalary;
        employeeSalaryCountText.text = "Employee Salary(" + JobManager.CurrentGameState.currentWorkerCount + ")";
        netMoneyText.text = "$" + net;

        // Message text
        workerQuitMessageText.text = "Wow, you made " + workersDemoralized + " workers quit. You're a shitty person, but you saved some money!";
        // @TODO: boss message

        // Button text
        if (finishedLastDayOfJob)
        {
            startNextButtonText.text = "start next job";
        }
        else
        {
            startNextButtonText.text = "start next day";
        }
    } 

    public void Btn_StartNextDay()
    {
        if(finishedLastDayOfJob)
        {
            // This job number is always right because we increase it in the JobManager
            sceneFader.FadeTo("Job" + JobManager.CurrentGameState.currentJobNumber);
        }
        else
        {
            sceneFader.FadeTo(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }        
    }
}
