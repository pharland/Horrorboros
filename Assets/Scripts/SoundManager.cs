using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource bloodSFXSource;
    public AudioSource crawlingSFXSource;
    public AudioSource pantingSFXSource;
    public AudioClip backgroundMusic;
    public AudioClip[] gameOverSFX;
    public AudioClip[] lightOrbSFX;
    public AudioClip[] tailSFX;
    public AudioClip[] bloodSFX;
    public AudioClip[] crawlingSFX;
    public AudioClip pantingSFX;
    public float bloodSFXInterval; // Interval in seconds for blood sfx loop
    public Slider volumeSlider;

    [Header("Blood SFX Pitch Range")]
    public float bloodSFXMinPitch;
    public float bloodSFXMaxPitch;

    private int lastLightOrbSFXIndex = -1; // To avoid repeating the last played sound effect
    private int lastTailSFXIndex = -1; // To avoid repeating the last played sound effect
    private int lastBloodSFXIndex = -1; // To avoid repeating the last played sound effect
    private int lastCrawlingSFXIndex = -1; // To avoid repeating the last played crawling SFX

    public float musicVolume = 0.1f;

    private const string VolumeSliderKey = "AudioSliderValue";
    private Coroutine bloodSFXCoroutine;

    void Awake()
    {
        // Load saved slider value (default to 1 if not set)
        float savedSliderValue = PlayerPrefs.GetFloat(VolumeSliderKey, 0.7f);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedSliderValue;
            volumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(); });
        }
        AdjustVolume();
    }

    void Start()
    {
        PlayMusic(backgroundMusic);

        if (sfxSource != null)
            sfxSource.volume = 0.08f;
        if (bloodSFXSource != null)
            bloodSFXSource.volume = 0.08f;

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != musicSource)
                audioSource.volume = 0.08f;
        }

        // Start crawling SFX loop
        if (crawlingSFXSource != null && crawlingSFX != null && crawlingSFX.Length > 0)
        {
            StartCoroutine(CrawlingSFXCoroutine());
        }

        // Start panting loop
        if (pantingSFXSource != null && pantingSFX != null)
        {
            pantingSFXSource.clip = pantingSFX;
            pantingSFXSource.loop = true;
            pantingSFXSource.Play();
        }
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
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play a blood sound effect at a random pitch
    /// </summary>
    /// <param name="clip">Cannot be null</param>
    public void PlayBloodSFX(AudioClip clip)
    {
        if (bloodSFXSource == null) return;

        bloodSFXSource.pitch = UnityEngine.Random.Range(bloodSFXMinPitch, bloodSFXMaxPitch);
        bloodSFXSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play one of the light pickup sound effects
    /// </summary>
    public void LightPickupSFX()
    {
        PlayRandomSFX(lightOrbSFX, ref lastLightOrbSFXIndex);
        BloodSFXLoop();
    }

    /// <summary>
    /// Play one of the tail sound effects
    /// </summary>
    public void TailSFX()
    {
        PlayRandomSFX(tailSFX, ref lastTailSFXIndex);
    }

    /// <summary>
    /// Loop through all of the blood sfx clips constantly
    /// </summary>
    public void BloodSFXLoop()
    {
        if (bloodSFXCoroutine != null)
            StopCoroutine(bloodSFXCoroutine);
        bloodSFXCoroutine = StartCoroutine(BloodSFXCoroutine());
    }

    /// <summary>
    /// Plays a random blood sound effect at regular intervals, without playing the same one twice in a row
    /// </summary>
    /// <param name="bloodSFXinterval">The time, in seconds, to wait between playing each sound effect. Must be greater than zero.</param>
    /// <returns>An enumerator that controls the coroutine execution.</returns>
    private System.Collections.IEnumerator BloodSFXCoroutine()
    {
        while (true)
        {
            PlayRandomBloodSFX(bloodSFX, ref lastBloodSFXIndex);
            yield return new WaitForSeconds(bloodSFXInterval);
        }
    }

    /// <summary>
    /// Stop the blood sfx loop
    /// </summary>
    public void StopBloodSFXLoop()
    {
        if (bloodSFXCoroutine != null)
        {
            StopCoroutine(bloodSFXCoroutine);
            bloodSFXCoroutine = null;
        }
    }

    /// <summary>
    /// Play a random sound effect from the given list, never repeating the last played
    /// </summary>
    /// <param name="clips">Array of AudioClips to choose from</param>
    /// <param name="lastPlayedIndex">Reference to last played index for this list</param>
    public void PlayRandomSFX(AudioClip[] clips, ref int lastPlayedIndex)
    {
        if (clips == null || clips.Length == 0) return;

        // Select a random index different from the last played
        int index;
        if (clips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do
            {
                index = UnityEngine.Random.Range(0, clips.Length);
            } while (index == lastPlayedIndex);
        }

        // Update last played index and play
        lastPlayedIndex = index;
        PlaySFX(clips[index]);
    }

    /// <summary>
    /// Play a random blood sound effect at a random pitch, never repeating the last played
    /// </summary>
    /// <param name="clips">Array of AudioClips to choose from</param>
    /// <param name="lastPlayedIndex">Reference to last played index for this list</param>
    public void PlayRandomBloodSFX(AudioClip[] clips, ref int lastPlayedIndex)
    {
        if (clips == null || clips.Length == 0) return;

        // Select a random index different from the last played
        int index;
        if (clips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do
            {
                index = UnityEngine.Random.Range(0, clips.Length);
            } while (index == lastPlayedIndex);
        }

        // Update last played index and play
        lastPlayedIndex = index;
        PlayBloodSFX(clips[index]);
    }

    /// <summary>
    /// Adjust volume based on slider
    /// </summary>
    public void AdjustVolume()
    {
        float sliderValue = volumeSlider.value;

        // Save slider value
        PlayerPrefs.SetFloat(VolumeSliderKey, sliderValue);
        PlayerPrefs.Save();

        // Convert slider value to decibels
        float dB = Mathf.Lerp(-40f, 0f, sliderValue);
        float volume = Mathf.Pow(10f, dB / 20f);

        // Store the music volume for corruption adjustments
        musicVolume = volume;

        // Update all audio source volumes
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.volume = volume;
        }

        // Explicitly update musicSource volume
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    internal void StopAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.Stop();
        }
    }

    internal void PlayGameOverSFX()
    {
        // Play each game over SFX simultaneously
        foreach (AudioClip clip in gameOverSFX)
        {
            PlaySFX(clip);
        }
    }

    /// <summary>
    /// Coroutine to play crawling SFX clips at random, never repeating the last played
    /// </summary>
    private System.Collections.IEnumerator CrawlingSFXCoroutine()
    {
        while (true)
        {
            // Select a random index different from the last played
            int index;
            if (crawlingSFX.Length == 1)
            {
                index = 0;
            }
            else
            {
                do
                {
                    index = UnityEngine.Random.Range(0, crawlingSFX.Length);
                } while (index == lastCrawlingSFXIndex);
            }
            lastCrawlingSFXIndex = index;

            crawlingSFXSource.clip = crawlingSFX[index];
            crawlingSFXSource.Play();

            // Wait for the clip to finish before playing the next one
            yield return new WaitForSeconds(crawlingSFXSource.clip.length);
        }
    }
}
