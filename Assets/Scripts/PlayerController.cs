using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float corruptionSpeedMultiplier = 3f;
    public float turnSmoothTime = 0.15f;
    public InputActionReference move;
    public Rigidbody rb;
    public CorruptionManager corruptionManager; // Assign in Inspector

    private Vector2 _moveDirection = Vector2.up;
    private Vector2 _currentDirection = Vector2.up;
    private Vector2 _lastInputDirection = Vector2.up;

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

            if (proposedDirection != -_lastInputDirection)
            {
                _moveDirection = proposedDirection;
                _lastInputDirection = proposedDirection;
            }
        }

        _currentDirection = Vector2.Lerp(_currentDirection, _moveDirection, Time.deltaTime / turnSmoothTime);
        if (_currentDirection.sqrMagnitude > 0.001f)
            _currentDirection.Normalize();
    }

    private void FixedUpdate()
    {
        float corruptionPercent = corruptionManager != null && corruptionManager.corruptionBar != null
            ? corruptionManager.corruptionBar.value / corruptionManager.corruptionBar.maxValue
            : 0f;
        float speedMultiplier = Mathf.Lerp(1f, corruptionSpeedMultiplier, corruptionPercent);
        float effectiveMoveSpeed = moveSpeed * speedMultiplier;

        Vector3 forward = new(_currentDirection.x, 0f, _currentDirection.y);
        rb.MovePosition(rb.position + effectiveMoveSpeed * Time.fixedDeltaTime * forward);

        if (_currentDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime / turnSmoothTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tail"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Collided with Tail - Game Over");
        }
    }
}
