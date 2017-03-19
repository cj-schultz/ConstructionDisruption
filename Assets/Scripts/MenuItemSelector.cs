using System.Collections;
using TMPro;
using UnityEngine;

// Controls the selection of menu items
[RequireComponent(typeof(AudioSource))]
public class MenuItemSelector : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip upSound;
    public AudioClip downSound;

    [Header("Item selection stuff")]
    [SerializeField]
    private TextMeshPro[] menuItems;
    [SerializeField]
    private Color selectionColor;
    [SerializeField]
    private Color lowHighlightColor;

    private AudioSource audioSource;

    private TextMeshPro currentSelection;
    private int currentSelectionIndex;
    private Color normalColor;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        currentSelectionIndex = 0;

        // Default the selection to the first item
        currentSelection = menuItems[currentSelectionIndex];
        normalColor = currentSelection.color;
        currentSelection.color = selectionColor;

        StartCoroutine("GlowCurrentSelection");
    }

    void Update()
    {        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            // Wrap the selection if needed
            if (currentSelectionIndex == menuItems.Length - 1)
            {
                currentSelectionIndex = 0;
            }
            else
            {
                currentSelectionIndex++;
            }

            audioSource.PlayOneShot(downSound);

            SelectCurrentIndex();
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            // Wrap the selection if needed
            if(currentSelectionIndex == 0)
            {
                currentSelectionIndex = menuItems.Length - 1;
            }
            else
            {
                currentSelectionIndex--;
            }

            // Play this sound quieter cause it's a little loud
            audioSource.PlayOneShot(upSound, .5f);

            SelectCurrentIndex();
        }
    }

    private void SelectCurrentIndex()
    {
        StopCoroutine("GlowCurrentSelection");

        currentSelection.color = normalColor;
        currentSelection = menuItems[currentSelectionIndex];
        currentSelection.color = selectionColor;

        StartCoroutine("GlowCurrentSelection");
    }

    // Lerps between selection color and normal color
    private IEnumerator GlowCurrentSelection()
    {
        // Target color starts as the low highlight color because we 
        // already set the current selection to the selection color.
        Color targetColor = lowHighlightColor;

        while(true)
        {
            while (!ColorDifferenceLessThan(targetColor, currentSelection.color, 0.1f))
            {
                currentSelection.color = Color.Lerp(currentSelection.color, targetColor, Time.deltaTime * 3f);
                yield return null;
            }

            // Change the target color
            if (targetColor == lowHighlightColor)
            {
                targetColor = selectionColor;
            }
            else
            {
                targetColor = lowHighlightColor;
            }
        }
    }

    // Returns true if the difference between color a and b is less than amount
    private bool ColorDifferenceLessThan(Color a, Color b, float amount)
    {
        float sumDifference = 0;

        sumDifference += Mathf.Abs(a.r - b.r);
        sumDifference += Mathf.Abs(a.g - b.g);
        sumDifference += Mathf.Abs(a.b - b.b);

        return sumDifference < amount;
    }
}
