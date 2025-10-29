using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadController : MonoBehaviour
{
    // Game Over when colliding with tail
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tail"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Collided with Head - Game Over");
        }
    }
}
