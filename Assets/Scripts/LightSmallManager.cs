using UnityEngine;

public class LightSmallManager : MonoBehaviour
{
    [SerializeField] private CorruptionManager _corruptionManager;
    [SerializeField] private float _amountToDecrease;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_corruptionManager != null)
            {
                _corruptionManager.DecreaseCorruption(_amountToDecrease);
                Debug.Log("Corruption decreased by " + _amountToDecrease);
            }
            else
            {
                Debug.LogWarning("CorruptionManager reference is missing!");
            }

            Destroy(gameObject);
        }
    }
}
