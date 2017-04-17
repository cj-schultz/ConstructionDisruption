using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public SceneFader sceneFader;

    public Color greenMoneyColor;
    public Color redMoneyColor;

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
    private bool foundationWasCompleted;

    public void Setup(int previousDayNumber, bool _finishedLastDayOfJob, int workersDemoralized)
    {
        finishedLastDayOfJob = _finishedLastDayOfJob;
        foundationWasCompleted = JobManager.CurrentGameState.currentJobFoundationCompletion >= 1;

        // Day text
        string dayString = "";
        dayString = "Day " + previousDayNumber + " / " + JobManager.Instance.jobBlueprint.numOfDays;
        dayText.text = dayString;

        // Job text
        if (finishedLastDayOfJob || foundationWasCompleted)
        {
            jobText.text = "Job " + (JobManager.CurrentGameState.currentJobNumber - 1);            
        }
        else
        {
            jobText.text = "Job " + (JobManager.CurrentGameState.currentJobNumber);
        }

        // @TODO: Take out the hard coded base employee salary
        int savings = JobManager.CurrentGameState.currentMoney;
        int dailyPay = JobManager.Instance.jobBlueprint.dailyBaseSalary;
        int employeeSalary = 200 * JobManager.CurrentGameState.currentWorkerCount;

        int net = savings + dailyPay - employeeSalary;
        JobManager.CurrentGameState.currentMoney = net;

        // Summary text
        savingsMoneyText.text = "$" + savings;
        savingsMoneyText.color = savings >= 0 ? greenMoneyColor : redMoneyColor;
        dailyPayMoneyText.text = "$" + dailyPay;        
        employeeSalaryCountText.text = "Employee Salary(" + JobManager.CurrentGameState.currentWorkerCount + ")";
        employeeSalaryMoneyText.text = "-$" + employeeSalary;
        netMoneyText.text = "$" + net;
        netMoneyText.color = net >= 0 ? greenMoneyColor : redMoneyColor;

        // Message text
        workerQuitMessageText.text = GetWorkerQuitMessage(workersDemoralized);
        bossMessageText.text = GetBossMessage();

        // Button text
        if(JobManager.CurrentGameState.currentJobNumber > JobManager.NUMBER_OF_JOBS)
        {
            startNextButtonText.text = "return to menu";
            workerQuitMessageText.text = "";

            // Save high score to disk   
            if(net < 0)
            {
                PlayerPrefs.SetInt("highScore", 0);
            }
            else
            {
                PlayerPrefs.SetInt("highScore", net);
            }            
        }
        else if (finishedLastDayOfJob || foundationWasCompleted)
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
        if (JobManager.CurrentGameState.currentJobNumber > JobManager.NUMBER_OF_JOBS)
        {
            sceneFader.FadeTo("MainMenu");
        }
        else if (finishedLastDayOfJob || foundationWasCompleted)
        {
            // This job number is always right because we increase it in the JobManager
            sceneFader.FadeTo("Job" + JobManager.CurrentGameState.currentJobNumber);
        }
        else
        {
            sceneFader.FadeTo(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }        
    }

    public void Btn_SaveAndQuit()
    {
        sceneFader.FadeTo("MainMenu");
    }

    private string GetWorkerQuitMessage(int workersDemoralized)
    {
        string s = "";

        if(workersDemoralized == 0)
        {
            s = "None of your workers quit! They must really like you.";
        }
        else
        {
            s = "Wow, you made " + workersDemoralized + (workersDemoralized == 1 ? " worker" : " workers") + " quit. You're a shitty person, but you saved some money!";
        }

        return s;
    }

    private string GetBossMessage()
    {
        string s = "";

        if(JobManager.CurrentGameState.currentJobNumber > JobManager.NUMBER_OF_JOBS)
        {
            s = "Boss: " + "Well, I can't say it's been nice working with you. I don't have any more jobs for you. Bye.";
        }
        else if (finishedLastDayOfJob)
        {
            s = "Boss: " + "It looks like your not capable of finishing this job. I'm going to assign you to another job on the other side of town.";
        }
        else if (foundationWasCompleted)
        {
            s = "Boss: " + "Good job! You and your workers completed the job. I'll start you on a new job bright and early tomorrow.";
        }
        else
        {
            s = "Boss: " + "Hey, looks like your crew didn't get much work done today. I'll give you some more workers.";
        }

        return s;
    }
}
