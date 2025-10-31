using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource bloodSFXSource;
    public AudioClip backgroundMusic;
    public AudioClip[] lightOrbSFX;
    public AudioClip[] tailSFX;
    public AudioClip[] bloodSFX;
    public float bloodSFXinterval; // Interval in seconds for blood sfx loop

    [Header("Blood SFX Pitch Range")]
    public float bloodSFXMinPitch;
    public float bloodSFXMaxPitch;

    private int lastLightOrbSFXIndex = -1; // To avoid repeating the last played sound effect
    private int lastTailSFXIndex = -1; // To avoid repeating the last played sound effect
    private int lastBloodSFXIndex = -1; // To avoid repeating the last played sound effect

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
        StopAllCoroutines();
        StartCoroutine(BloodSFXCoroutine());
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
            yield return new WaitForSeconds(bloodSFXinterval);
        }
    }

    /// <summary>
    /// Stop the blood sfx loop
    /// </summary>
    public void StopBloodSFXLoop()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Play a random sound effect from the given list, never repeating the last played
    /// </summary>
    /// <param name="clips">Array of AudioClips to choose from</param>
    /// <param name="lastPlayedIndex">Reference to last played index for this list</param>
    public void PlayRandomSFX(AudioClip[] clips, ref int lastPlayedIndex)
    {
        if (clips == null || clips.Length == 0) return;

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
}
