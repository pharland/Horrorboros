using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadController : MonoBehaviour
{
    [SerializeField] private ParticleSystem lightOrbParticleEffectPrefab;
    [SerializeField] private ScreenShake screenShake; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tail"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Collided with Head - Game Over");
        }
        else if (other.CompareTag("LightOrb"))
        {
            if (lightOrbParticleEffectPrefab != null)
            {
                Debug.Log("Collided with LightOrb - Play Particle Effect at collision");
                ParticleSystem effect = Instantiate(
                    lightOrbParticleEffectPrefab,
                    other.transform.position,
                    Quaternion.identity
                );
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
            }
            if (screenShake != null)
            {
                screenShake.Shake();
            }
        }
    }
}
