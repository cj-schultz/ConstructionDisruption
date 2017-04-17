using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Boo.Lang;

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
    public TextMeshProUGUI jobCompletionText;
    public TextMeshProUGUI currentCashText;
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
        currentCashText.text = "$" + net;
        currentCashText.color = net >= 0 ? greenMoneyColor : redMoneyColor;
        jobCompletionText.text = (JobManager.CurrentGameState.currentJobFoundationCompletion * 100f) + "%";

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

        // Shop setup
        SetupShop();
    } 

    private void SetupShop()
    {
        // "Use" any items that the user had the previous day
        List<ShopItem> itemsToRemove = new List<ShopItem>();
        for (int i = 0; i < JobManager.CurrentGameState.inventory.Count; i++)
        {
            int countIndex = JobManager.Instance.IndexOfShopItem(JobManager.CurrentGameState.inventory[i]);
            JobManager.CurrentGameState.inventoryCount[countIndex]--;
            if(JobManager.CurrentGameState.inventoryCount[countIndex] <= 0)
            {
                JobManager.CurrentGameState.inventoryCount[countIndex] = 0;
                itemsToRemove.Add(JobManager.CurrentGameState.inventory[i]);
            }
        }
        for (int i = 0; i < itemsToRemove.Count; i++)
        {
            JobManager.CurrentGameState.inventory.Remove(itemsToRemove[i]);
        }        

        currentSelectedShopItemIndex = -1;
        descriptionText.text = "";
        buyButton.interactable = false;

        coughDropsText.color = Color.white;
        constructionDisriptionText.color = Color.white;
        yeezysText.color = Color.white;

        UpdateInventoryText();
    }

    private void UpdateInventoryText()
    {
        inventoryItem1.text = "";
        inventoryItem2.text = "";
        inventoryItem3.text = "";

        TextMeshProUGUI[] texts = new TextMeshProUGUI[3];
        texts[0] = inventoryItem1;
        texts[1] = inventoryItem2;
        texts[2] = inventoryItem3;

        for (int i = 0; i < JobManager.CurrentGameState.inventory.Count; i++)
        {
            switch(JobManager.CurrentGameState.inventory[i])
            {
                case ShopItem.CoughDrop:
                    texts[i].text = "Cough Drop (" + JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.CoughDrop)] + ")";
                    break;
                case ShopItem.ConstructionDisruption:
                    texts[i].text = "Construction Disruption (" + JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.ConstructionDisruption)] + ")";
                    break;
                case ShopItem.Yeezys:
                    texts[i].text = "Yeezys (" + JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.Yeezys)] + ")";
                    break;
            }
        }        
    }

    public void Btn_SelectShopItem(int itemIndex)
    {
        currentSelectedShopItemIndex = itemIndex;

        int costOfItem = CostOfItem(currentSelectedShopItemIndex);

        if(JobManager.CurrentGameState.currentMoney >= costOfItem)
        {
            buyButton.interactable = true;
        }        
        else
        {
            buyButton.interactable = false;
        }

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
                descriptionText.text = "Disrupt your consctruction and sabotage your foundation building. This will reduce the completeness of the foundation by " + JobManager.Instance.disruptionAmount + "%. One day use.";
                break;
            case 3: // Yeezies
                yeezysText.color = Color.red;
                descriptionText.text = "These sick kicks will have you running like my nose when I have the flu. Press <size=135%>shift</size> for a short burst of speed. One day use";
                break;
        }
    }

    public void Btn_BuySelectedShopItem()
    {
        int amountToSpend = CostOfItem(currentSelectedShopItemIndex);

        JobManager.CurrentGameState.currentMoney -= amountToSpend;

        switch(currentSelectedShopItemIndex)
        {
            case 1: // Cough Drops
                if(!JobManager.CurrentGameState.inventory.Contains(ShopItem.CoughDrop))
                {
                    JobManager.CurrentGameState.inventory.Add(ShopItem.CoughDrop);
                }
                JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.CoughDrop)]++;
                break;
            case 2: // Construction Distruption
                if (!JobManager.CurrentGameState.inventory.Contains(ShopItem.ConstructionDisruption))
                {
                    JobManager.CurrentGameState.inventory.Add(ShopItem.ConstructionDisruption);
                }
                JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.ConstructionDisruption)]++;
                break;
            case 3: // Yeezies
                if (!JobManager.CurrentGameState.inventory.Contains(ShopItem.Yeezys))
                {
                    JobManager.CurrentGameState.inventory.Add(ShopItem.Yeezys);
                }
                JobManager.CurrentGameState.inventoryCount[JobManager.Instance.IndexOfShopItem(ShopItem.Yeezys)]++;                
                break;
        }

        if(amountToSpend > JobManager.CurrentGameState.currentMoney)
        {
            buyButton.interactable = false;
        }

        UpdateInventoryText();

        currentCashText.text = "$" + JobManager.CurrentGameState.currentMoney;
    }

    private int CostOfItem(int itemIndex)
    {
        int cost = 0;

        switch (currentSelectedShopItemIndex)
        {
            case 1: // Cough Drops
                cost = 100;
                break;
            case 2: // Construction Distruption
                cost = 200;
                break;
            case 3: // Yeezies
                cost = 150;
                break;
        }

        return cost;
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
