// This class holds variables necessary to know the current player's state
using System.Collections.Generic;

[System.Serializable]
public class GameState
{
    public int currentWorkerCount;
    public int currentJobNumber;
    public int currentDayNumber;
    public int currentMoney;
    public float currentJobFoundationCompletion; // Ranges between 0 and 1

    public List<ShopItem> inventory;
    public int[] inventoryCount;

    // Constructor
    public GameState()
    {
        currentWorkerCount = 0;
        currentJobNumber = 1;
        currentDayNumber = 1;
        currentMoney = 0;
        currentJobFoundationCompletion = 0;
        inventory = new List<ShopItem>();
        inventoryCount = new int[3];
        inventoryCount[0] = inventoryCount[1] = inventoryCount[2] = 0;
    }
}

[System.Serializable]
public enum ShopItem { CoughDrop, ConstructionDisruption, Yeezys }

