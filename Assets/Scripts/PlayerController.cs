using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSmoothTime = 0.15f; // How quickly to turn
    public InputActionReference move;
    public Rigidbody rb;

    private Vector2 _moveDirection = Vector2.up; // Target direction for smooth turning
    private Vector2 _currentDirection = Vector2.up; // Smoothed direction for turning
    private Vector2 _lastInputDirection = Vector2.up; // Last input direction

    void Update()
    {
        Vector2 input = move.action.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.001f)
        {
            Vector2 proposedDirection;
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                proposedDirection = new Vector2(Mathf.Sign(input.x), 0f);
            }
            else
            {
                proposedDirection = new Vector2(0f, Mathf.Sign(input.y));
            }

            // Only update if not opposite of last input direction
            if (proposedDirection != -_lastInputDirection)
            {
                _moveDirection = proposedDirection;
                _lastInputDirection = proposedDirection;
            }
        }

        // Smoothly curve towards the target direction
        _currentDirection = Vector2.Lerp(_currentDirection, _moveDirection, Time.deltaTime / turnSmoothTime);
        if (_currentDirection.sqrMagnitude > 0.001f)
            _currentDirection.Normalize();
    }

    private void FixedUpdate()
    {
        // Move forward in the current direction
        Vector3 forward = new(_currentDirection.x, 0f, _currentDirection.y);
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * forward);

        // When turning, smoothly rotate towards movement direction
        if (_currentDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime / turnSmoothTime));
        }
    }

    // Game Over when colliding with tail
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tail"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Collided with Tail - Game Over");
        }
    }
}
