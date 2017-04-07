// This class holds variables necessary to know the current player's state
public class GameState
{
    public int currentJobIndex;
    public int currentDayIndex;
    public float currentMoney;
    public float currentJobFoundationCompletion; // Ranges between 0 and 1

    // Constructor
    public GameState()
    {
        currentJobIndex = 0;
        currentDayIndex = 0;
        currentMoney = 0;
        currentJobFoundationCompletion = 0;
    }
}
