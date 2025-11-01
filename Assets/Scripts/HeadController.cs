using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private ParticleSystem lightOrbParticleEffectPrefab;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float FovBaseIncrease = 2f;
    [SerializeField] private float FovMax = 70f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tail"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (other.CompareTag("LightOrb"))
        {
            if (lightOrbParticleEffectPrefab != null)
            {
                ParticleSystem particleEffect = Instantiate(
                    lightOrbParticleEffectPrefab,
                    other.transform.position,
                    Quaternion.identity
                );
                particleEffect.Play();
                Destroy(particleEffect.gameObject, particleEffect.main.duration + particleEffect.main.startLifetime.constantMax);
            }

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }

            // Logarithmic FOV increase
            if (cinemachineCamera != null)
            {
                float currentFov = cinemachineCamera.Lens.FieldOfView;
                float distanceToMax = FovMax - currentFov;
                if (distanceToMax > 0.01f)
                {
                    // Logarithmic scaling: the closer to max, the smaller the increase
                    float logScale = Mathf.Log10(distanceToMax + 1); // +1 to avoid log(0)
                    float fovIncrease = Mathf.Clamp(FovBaseIncrease * logScale, 0.01f, FovBaseIncrease);
                    float newFov = Mathf.Min(currentFov + fovIncrease, FovMax);
                    cinemachineCamera.Lens.FieldOfView = newFov;
                }
            }
        }
    }
}
