// Each job has a blueprint. The blueprint holds the neccessary information about the job
[System.Serializable]
public class JobBlueprint
{
    // The number of real-time seconds that should map to in-game hours
    public float secondsToHourRatio = 4f;
    // The hour the current day should start at
    // Note: This is always assumed to be AM
    public int startHour;
    // The hour that the current day should end at
    // Note: This is always assumed to be PM
    public int endHour;
   
    public int numOfDays;
    public int dailyBaseSalary;

    public int workersToAddEveryDay;
    public int workerSalary;
}
