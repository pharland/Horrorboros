using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CorruptionManager : MonoBehaviour
{
    public Slider corruptionBar;
    public float corruptionSpeed;
    public float musicChangeOnCorruptionPercent; // Percentage of corruption (0-1) at which music changes start
    public float bgmPitchModifierMax; // Maximum pitch increase for background music when at max corruption
    public float bgmVolumeModifierMax; // Maximum volume increase for background music when at max corruption

    // Reference to SoundManager (assign in Inspector or find at runtime)
    public SoundManager soundManager;

    private float initialMusicVolume;

    internal void DecreaseCorruption(float amountToDecrease)
    {
        corruptionBar.value -= amountToDecrease;
    }

    void Start()
    {
        if (soundManager != null && soundManager.musicSource != null)
        {
            initialMusicVolume = soundManager.musicSource.volume;
        }
        else
        {
            Debug.Log("soundManager or musicSource is null");
        }
    }

    void Update()
    {
        // increase corruption over time
        if (corruptionBar.value < corruptionBar.maxValue)
        {
            corruptionBar.value += Time.deltaTime * corruptionSpeed;
        }
        else
        {
            // Game Over
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Update music pitch and volume based on corruption (exponential increase after 50%)
        if (soundManager != null && soundManager.musicSource != null)
        {
            float corruptionPercent = corruptionBar.value / corruptionBar.maxValue;

            if (corruptionPercent < musicChangeOnCorruptionPercent)
            {
                soundManager.musicSource.pitch = 1f;
                soundManager.musicSource.volume = initialMusicVolume;
            }
            else
            {
                // Remap 0.5-1.0 to 0-1 for exponential curve
                float mappedPercent = (corruptionPercent - musicChangeOnCorruptionPercent) * 2f;
                float expPitch = Mathf.Lerp(1f, bgmPitchModifierMax, Mathf.Pow(mappedPercent, 2f));
                float expVolume = Mathf.Lerp(initialMusicVolume, bgmVolumeModifierMax, Mathf.Pow(mappedPercent, 2f));
                soundManager.musicSource.pitch = expPitch;
                soundManager.musicSource.volume = expVolume;
            }
        }
    }
}
