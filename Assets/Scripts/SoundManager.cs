using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;// Singleton instance
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip backgroundMusic;

    void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    /// <summary>
    /// Play background music
    /// </summary>
    /// <param name="clip">Cannot be null</param>
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>
    /// Play a sound effect
    /// </summary>
    /// <param name="clip">Cannot be null</param>
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
