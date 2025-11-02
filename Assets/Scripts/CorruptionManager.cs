using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CorruptionManager : MonoBehaviour
{
    public Slider corruptionBar;
    public float corruptionSpeed;
    public float musicChangeOnCorruptionPercent;
    public float bgmPitchModifierMax;
    public float bgmVolumeModifierMax;

    public SoundManager soundManager;
    public UIManager uiManager;

    [Header("Corruption Bar Colors (Editable in Inspector)")]
    public Color corruptionStartColor = Color.red;
    public Color corruptionEndColor = Color.yellow;

    [SerializeField] private Image corruptionBarFill; // Assign the fill Image in Inspector

    private float initialMusicVolume;
    private bool gameOverTriggered = false;

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
        else if (!gameOverTriggered)
        {
            uiManager.EnableGameOverUI();
            gameOverTriggered = true;
        }

        float corruptionPercent = corruptionBar.value / corruptionBar.maxValue;

        // Transition slider color from 50% corruption onwards
        if (corruptionBarFill != null)
        {
            float lerpPercent = Mathf.InverseLerp(0.5f, 1f, corruptionPercent);
            corruptionBarFill.color = Color.Lerp(corruptionStartColor, corruptionEndColor, lerpPercent);
        }

        // Update music pitch and volume based on corruption
        if (soundManager != null && soundManager.musicSource != null)
        {
            float sliderVolume = soundManager.musicVolume;

            if (corruptionPercent < musicChangeOnCorruptionPercent)
            {
                soundManager.musicSource.pitch = 1f;
                soundManager.musicSource.volume = sliderVolume;
            }
            else
            {
                float mappedPercent = (corruptionPercent - musicChangeOnCorruptionPercent) * 2f;
                float expPitch = Mathf.Lerp(1f, bgmPitchModifierMax, Mathf.Pow(mappedPercent, 2f));
                float expVolume = Mathf.Lerp(1f, bgmVolumeModifierMax, Mathf.Pow(mappedPercent, 2f));
                soundManager.musicSource.pitch = expPitch;
                soundManager.musicSource.volume = sliderVolume * expVolume;
            }
        }
    }
}
