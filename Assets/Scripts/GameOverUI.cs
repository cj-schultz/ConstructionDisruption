using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    public GameObject newHighScoreText;

    [Header("Shop stuff")]
    public TextMeshProUGUI descriptionText;
    public Button buyButton;
    public TextMeshProUGUI coughDropsText;
    public TextMeshProUGUI constructionDisriptionText;
    public TextMeshProUGUI yeezysText;
    public TextMeshProUGUI inventoryItem1;
    public TextMeshProUGUI inventoryItem2;
    public TextMeshProUGUI inventoryItem3;

    private bool finishedLastDayOfJob;
    private bool foundationWasCompleted;

    private int currentSelectedShopItemIndex;

    public void Setup(int previousDayNumber, bool _finishedLastDayOfJob, int workersDemoralized)
    {      
        newHighScoreText.SetActive(false);

        finishedLastDayOfJob = _finishedLastDayOfJob;
        foundationWasCompleted = JobManager.CurrentGameState.currentJobFoundationCompletion >= 1;

        // Shop setup
        SetupShop();

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

        int savings = JobManager.CurrentGameState.currentMoney;
        int dailyPay = JobManager.Instance.jobBlueprint.dailyBaseSalary;
        int employeeSalary = JobManager.Instance.jobBlueprint.workerSalary * JobManager.CurrentGameState.currentWorkerCount;

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
            if (PlayerPrefs.HasKey("highScore"))
            {
                int hs = PlayerPrefs.GetInt("highScore");

                if(net > hs)
                {
                    if(net < 0)
                    {
                        PlayerPrefs.SetInt("highScore", 0);
                    }
                    else
                    {
                        // New high Score!!
                        PlayerPrefs.SetInt("highScore", net);
                        newHighScoreText.SetActive(true);
                    }
                }
            }
            else
            {
                // New high Score!!
                PlayerPrefs.SetInt("highScore", net);
                newHighScoreText.SetActive(true);
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

    private void SetupShop()
    {
        currentSelectedShopItemIndex = -1;
        descriptionText.text = "";
        buyButton.interactable = false;

        coughDropsText.color = Color.white;
        constructionDisriptionText.color = Color.white;
        yeezysText.color = Color.white;

        inventoryItem1.text = "";
        inventoryItem2.text = "";
        inventoryItem3.text = "";
        // @TODO: Setup inventory
        switch (JobManager.CurrentGameState.inventory.Count)
        {
            case 0:                                
                break;
        }
    }

    public void Btn_SelectShopItem(int itemIndex)
    {
        currentSelectedShopItemIndex = itemIndex;
        buyButton.interactable = true;

        coughDropsText.color = Color.white;
        constructionDisriptionText.color = Color.white;
        yeezysText.color = Color.white;

        switch (currentSelectedShopItemIndex)
        {
            case 1: // Cough Drops
                coughDropsText.color = Color.red;
                descriptionText.text = "Cough drops will sooth your throat. You will be able to recharge your yell meter quicker. One day use.";
                break;
            case 2: // Construction Distruption
                constructionDisriptionText.color = Color.red;
                descriptionText.text = "Disrupt your consctruction and sabotage your foundation building. This will reduce the completeness of the foundation by 10%. One day use.";
                break;
            case 3: // Yeezies
                yeezysText.color = Color.red;
                descriptionText.text = "These sick kicks will have you running like my nose when I have the flu. Press shift for a short burst of speed. One day use";
                break;
        }
    }

    public void Btn_BuySelectedShopItem()
    {
        int amountToSpend = 0;

        switch(currentSelectedShopItemIndex)
        {
            case 1: // Cough Drops
                amountToSpend = 100;
                break;
            case 2: // Construction Distruption
                amountToSpend = 200;
                break;
            case 3: // Yeezies
                amountToSpend= 150;
                break;
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
        WriteGameStateToDisk();
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
            s = "It looks like your not capable of finishing this job. I'm going to assign you to another job on the other side of town.";
        }
        else if (foundationWasCompleted)
        {
            s = "Good job! You and your workers completed the job. I'll start you on a new job bright and early tomorrow.";
        }
        else
        {
            s = "Hey, looks like your crew didn't get much work done today. I'll give you some more workers.";
        }

        return s;
    }

    private void WriteGameStateToDisk()
    {
        if (JobManager.CurrentGameState == null)
        {
            return;
        }

#if UNITY_EDITOR
        string path = Application.persistentDataPath + JobManager.EDITOR_GAME_STATE_DISK_PATH;
#else
        string path = Application.persistentDataPath + JobManager.EXE_GAME_STATE_DISK_PATH;
#endif

        Stream file = File.Open(path, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(file, JobManager.CurrentGameState);

        file.Close();
    }
}
