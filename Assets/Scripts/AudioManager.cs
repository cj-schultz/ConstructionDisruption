using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip menuLoopMusic;
    public AudioClip gameLoopMusic;

    private AudioSource audioSource;

    private enum CurrentLoopMusic { None, Menu, Game };
    private CurrentLoopMusic currentLoop;

    void Awake()
    {
        // @Note(colin): This is a singleton, google it or something
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            enabled = false;
            return;
        }
        else if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        currentLoop = CurrentLoopMusic.None;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayGameLoopMusic()
    {
        // We have this check so we don't restart the game music if it is already playing
        if(currentLoop != CurrentLoopMusic.Game)
        {
            audioSource.clip = gameLoopMusic;
            audioSource.Play();
            currentLoop = CurrentLoopMusic.Game;
        }
    }

    public void PlayMenuLoopMusic()
    {
        if (currentLoop != CurrentLoopMusic.Menu)
        {
            audioSource.clip = menuLoopMusic;
            audioSource.Play();
            currentLoop = CurrentLoopMusic.Menu;
        }
    }
}
