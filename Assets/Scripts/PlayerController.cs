using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public InputActionReference move;
    public Rigidbody rb;

    private Vector2 _moveDirection = Vector2.up; // Default facing "up" (Z+)

    void Update()
    {
        Vector2 input = move.action.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.001f)
        {
            // Snap to cardinal directions only
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                _moveDirection = new Vector2(Mathf.Sign(input.x), 0f);
            }
            else
            {
                _moveDirection = new Vector2(0f, Mathf.Sign(input.y));
            }
        }
    }

    private void FixedUpdate()
    {
        // Always move forward in the facing direction
        Vector3 forward = new(_moveDirection.x, 0f, _moveDirection.y);
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * forward);

        // Rotate to face the current direction
        if (_moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            rb.MoveRotation(targetRotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tail"))
        {
            // Game Over
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Collided with Tail - Game Over");
        }
    }
}
