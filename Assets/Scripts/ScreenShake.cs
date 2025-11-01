using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.2f;

    private Vector3 originalPosition;
    private float shakeTimer;

    // Initialize original position
    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // If shaking, apply random offset to position
        if (shakeTimer > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeIntensity;
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                transform.localPosition = originalPosition;
            }
        }
    }

    // Public method to trigger screenshake
    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}