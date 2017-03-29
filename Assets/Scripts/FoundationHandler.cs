using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

//This component is placed on a foundation and keeps track of its progress towards being completed. It also allows the foundation to accept resources from AI units.
public class FoundationHandler : MonoBehaviour
{
    // Note(Colin): I changed these to float because we need to divide with them to get ratios
    //How many resources are needed for the foundation to be finished different stages
    float Stage1Count;
    float Stage2Count;
    float Stage3Count;
    public float FinalCount;

    [Header("Foundation Status UI")]
    public Image statusFillBar;
    public TextMeshProUGUI statusText;
    public float statusBarFillSpeed;

    //Keeps track of how many resources the foundation has
    private float CurrentCount;

    // Use this for initialization
    void Start()
    {
        CurrentCount = 0;

        // Start with 0% status for now
        statusFillBar.fillAmount = 0f;
        statusText.text = "0%";   
    }    

    // Note(Colin): I moved the CurrentCount checking to giveResource, just cause we don't need to check it every frame,
    //              we will only need to check it when the resource count changes
    //Called by a worker's AI when it wants to give resources to this foundation
    public void giveResource(int Given)
    {
        CurrentCount = CurrentCount + Given;

        // Update UI
        StopCoroutine("UpdateStatusGUI");
        StartCoroutine("UpdateStatusGUI");
                
        if (CurrentCount >= Stage1Count & CurrentCount < Stage2Count)
        {
            //Change artwork here
        }
        else if (CurrentCount >= Stage2Count & CurrentCount < Stage3Count)
        {
            //Change artwork here
        }
        else if (CurrentCount >= Stage3Count & CurrentCount < FinalCount)
        {
            //Change artwork here
        }
        else if (CurrentCount >= FinalCount)
        {
            //Change artwork here
            //Also run any code connected to this foundation being finished (Ex. any end game conditions)
        }
    }

    private IEnumerator UpdateStatusGUI()
    {
        float completionRatio = CurrentCount / FinalCount;

        if(completionRatio >= 1)
        {
            // Lose the game
            statusFillBar.fillAmount = 1f;
            statusText.text = "100%";
        }
        else
        {
            // Set the status text and animate the loading bar
            statusText.text = Mathf.Round(completionRatio * 100f) + "%";

            while(statusFillBar.fillAmount != completionRatio)
            {
                statusFillBar.fillAmount = Mathf.MoveTowards(statusFillBar.fillAmount, completionRatio, Time.deltaTime * statusBarFillSpeed);
                yield return null;
            }         
        }
        
        yield return null;
    }
}
