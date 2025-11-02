using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [SerializeField] private Image corruptionBarFill;

    [Header("Vignette Settings")]
    public Volume postProcessVolume;
    public float minVignetteIntensity;
    public float maxVignetteIntensity;

    private Vignette vignetteEffect;
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

        // Get the Vignette effect from the Volume
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out Vignette vignette))
        {
            vignetteEffect = vignette;
        }
        else
        {
            Debug.LogWarning("Vignette effect not found in Volume profile.");
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

        // Update vignette intensity based on corruption (exponential increase after 50%)
        if (vignetteEffect != null)
        {
            float vignetteIntensity = minVignetteIntensity;
            if (corruptionPercent >= 0.5f)
            {
                // Remap corruptionPercent from [0.5, 1] to [0, 1]
                float remappedPercent = Mathf.InverseLerp(0.5f, 1f, corruptionPercent);
                // Exponential mapping (slow at first, ramps up quickly)
                float expPercent = Mathf.Pow(remappedPercent, 2f); // You can adjust the exponent for curve shape
                vignetteIntensity = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, expPercent);
            }
            vignetteEffect.intensity.value = vignetteIntensity;
        }
    }
}
