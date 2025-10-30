using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private CorruptionManager _corruptionManager;
    [SerializeField] private TailController _TailController;
    [SerializeField] private float _amountToDecrease;

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

            Destroy(gameObject);
        }
    }
}
