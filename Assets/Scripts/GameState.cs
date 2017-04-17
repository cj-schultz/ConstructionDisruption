// This class holds variables necessary to know the current player's state
[System.Serializable]
public class GameState
{
    public int currentWorkerCount;
    public int currentJobNumber;
    public int currentDayNumber;
    public float currentMoney;
    public float currentJobFoundationCompletion; // Ranges between 0 and 1

    // Constructor
    public GameState()
    {
        currentWorkerCount = 0;
        currentJobNumber = 1;
        currentDayNumber = 1;
        currentMoney = 0;
        currentJobFoundationCompletion = 0;
    }
}
