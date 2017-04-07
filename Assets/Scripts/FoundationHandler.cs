using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

//This component is placed on a foundation and keeps track of its progress towards being completed. It also allows the foundation to accept resources from AI units.
public class FoundationHandler : MonoBehaviour
{
    // Note(Colin): I changed these to float because we need to divide with them to get ratios
    //How many resources are needed for the foundation to be finished different stages
    float stage1Count;
    float stage2Count;
    float stage3Count;
    public int finalCount = 100;

    [Header("Foundation Status UI")]
    public Image statusFillBar;
    public TextMeshProUGUI statusText;
    public float statusBarFillSpeed;

    //Keeps track of how many resources the foundation has
    private int currentCount;

    void Start()
    {
        // Derive the current count from the game state's foundation fill percentage
        currentCount = (int)(JobManager.CurrentGameState.currentJobFoundationCompletion * finalCount);

        StartCoroutine("UpdateStatusGUI"); 
    }    

    // Note(Colin): I moved the CurrentCount checking to giveResource, just cause we don't need to check it every frame,
    //              we will only need to check it when the resource count changes
    //Called by a worker's AI when it wants to give resources to this foundation
    public void GiveResource(int Given)
    {
        currentCount = currentCount + Given;

        // Update UI
        StopCoroutine("UpdateStatusGUI");
        StartCoroutine("UpdateStatusGUI");
                
        if (currentCount >= stage1Count & currentCount < stage2Count)
        {
            //Change artwork here
        }
        else if (currentCount >= stage2Count & currentCount < stage3Count)
        {
            //Change artwork here
        }
        else if (currentCount >= stage3Count & currentCount < finalCount)
        {
            //Change artwork here
        }
        else if (currentCount >= finalCount)
        {
            //Change artwork here
            //Also run any code connected to this foundation being finished (Ex. any end game conditions)
        }
    }

    private IEnumerator UpdateStatusGUI()
    {
        float completionRatio = (float)currentCount / (float)finalCount;
        completionRatio = Mathf.Clamp(completionRatio, 0f, 1f);

        // Update game state
        JobManager.CurrentGameState.currentJobFoundationCompletion = completionRatio;        

        // Set the status text 
        statusText.text = Mathf.Round(completionRatio * 100f) + "%";

        // Animate the completion status bar
        while (statusFillBar.fillAmount != completionRatio)
        {
            statusFillBar.fillAmount = Mathf.MoveTowards(statusFillBar.fillAmount, completionRatio, Time.deltaTime * statusBarFillSpeed);
            yield return null;
        }                    
    }
}
