using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private CorruptionManager _corruptionManager;
    [SerializeField] private TailController _TailController;
    [SerializeField] private float _amountToDecrease;
    public bool isTutorialOrb = false;

    private SoundManager _soundManager;
    private UIManager _UIManager;

    private void Start()
    {
        _soundManager = FindFirstObjectByType<SoundManager>();
        _UIManager = FindFirstObjectByType<UIManager>();
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

            // Trigger tutorial message if this is a tutorial orb, but only the first time
            if (isTutorialOrb && _corruptionManager != null)
            {
                if (PlayerPrefs.GetInt("HasSeenWarning", 0) == 0)
                {
                    _UIManager.EnableWarningUI();
                    PlayerPrefs.SetInt("HasSeenWarning", 1);
                    PlayerPrefs.Save();
                }
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
