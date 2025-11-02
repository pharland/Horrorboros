using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private CorruptionManager _corruptionManager;
    [SerializeField] private TailController _TailController;
    [SerializeField] private float _amountToDecrease;

    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = FindFirstObjectByType<SoundManager>();
        _corruptionManager = FindFirstObjectByType<CorruptionManager>();
        _TailController = FindFirstObjectByType<TailController>();

        // Set this orb's SFX volume to match the audio slider
        var orbAudio = GetComponent<AudioSource>();
        if (_soundManager != null && orbAudio != null)
        {
            orbAudio.volume = _soundManager.volumeSlider.value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            // Decrease corruption
            if (_corruptionManager != null)
            {
                _corruptionManager.DecreaseCorruption(_amountToDecrease);
            }
            else
            {
                Debug.LogWarning("CorruptionManager reference is missing!");
            }

            // Add segment to player
            if (_TailController != null)
            {
                _TailController.AddSegment();
            }
            else
            {
                Debug.LogWarning("TailController reference is missing!");
            }
            
            // Play sound effect
            if (_soundManager != null)
            {
                _soundManager.LightPickupSFX();
            }
            else
            {
                Debug.LogWarning("SoundManager reference is missing!");
            }

            // Destroy parent object (and the orb itself)
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
