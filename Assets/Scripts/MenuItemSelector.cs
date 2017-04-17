using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

// Controls the selection of menu items
[RequireComponent(typeof(AudioSource))]
public class MenuItemSelector : MonoBehaviour
{
    public SceneFader sceneFader;

    public TextMeshPro highScoreText;

    [Header("Sounds")]
    public AudioClip upSound;
    public AudioClip downSound;

    [Header("Item selection stuff")]
    public TextMeshPro[] menuItems;
    public Color selectionColor;
    public Color lowHighlightColor;

    private AudioSource audioSource;

    private TextMeshPro currentSelection;
    private int currentSelectionIndex;
    private Color normalColor;

    private bool acceptingInputs;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        currentSelectionIndex = 0;

        // Default the selection to the first item
        currentSelection = menuItems[currentSelectionIndex];
        normalColor = currentSelection.color;
        currentSelection.color = selectionColor;

        acceptingInputs = true;

        // Disable to continue button if there isn't any game state on disk
        if(!DoesGameStateExistOnDisk())
        {
            menuItems[1].color = Color.black;
        }

        if(PlayerPrefs.HasKey("highScore"))
        {
            highScoreText.text = "$" + PlayerPrefs.GetInt("highScore");
        }
        else
        {
            highScoreText.text = "$0";
        }

        StartCoroutine("GlowCurrentSelection");
    }

    void Update()
    {      
        if(!acceptingInputs)
        {
            return;
        }  

        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            // Wrap the selection if needed
            if (currentSelectionIndex == menuItems.Length - 1)
            {
                currentSelectionIndex = 0;
            }
            else
            {
                // Skip the continue button if it is disabled
                if(currentSelectionIndex == 0 && !DoesGameStateExistOnDisk())
                {
                    currentSelectionIndex += 2;
                }
                else
                {
                    currentSelectionIndex++;
                }                
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
                if (currentSelectionIndex == 2 && !DoesGameStateExistOnDisk())
                {
                    currentSelectionIndex -= 2;
                }
                else
                {
                    currentSelectionIndex--;
                }                    
            }

            // Play this sound quieter cause it's a little loud
            audioSource.PlayOneShot(upSound, .5f);

            SelectCurrentIndex();
        }
        else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            switch(currentSelectionIndex)
            {
                case 0: // New Game
                    JobManager.CurrentGameState = new GameState();
                    sceneFader.FadeTo("Job1");
                    acceptingInputs = false;
                    break;
                case 1: // Continue
                    LoadGameStateFromDisk();
                    sceneFader.FadeTo("Job" + JobManager.CurrentGameState.currentJobNumber);
                    break;
                case 2: // Settings
                    break;
                case 3: // Exit
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                    break;
            }           
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

    private void LoadGameStateFromDisk()
    {
        GameState state = null;
#if UNITY_EDITOR
        string path = Application.persistentDataPath + JobManager.EDITOR_GAME_STATE_DISK_PATH;
#else
        string path = Application.persistentDataPath + JobManager.EXE_GAME_STATE_DISK_PATH;
#endif

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            state = (GameState)formatter.Deserialize(file);
            
            file.Close();
        }
        else
        {
            // @TODO: Handle failure. Just like my dad always told me.
        }
        
        JobManager.CurrentGameState = state;
    }

    private void DeleteGameStateFromDisk()
    {
#if UNITY_EDITOR
        string path = Application.persistentDataPath + JobManager.EDITOR_GAME_STATE_DISK_PATH;
#else
        string path = Application.persistentDataPath + JobManager.EXE_GAME_STATE_DISK_PATH;
#endif
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private bool DoesGameStateExistOnDisk()
    {
#if UNITY_EDITOR
        string path = Application.persistentDataPath + JobManager.EDITOR_GAME_STATE_DISK_PATH;
#else
        string path = Application.persistentDataPath + JobManager.EXE_GAME_STATE_DISK_PATH;
#endif
        return File.Exists(path);
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
