using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public InputActionReference move;
    public Rigidbody rb;

    private Vector2 _moveDirection;

    void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        //Move the player based on input
        Vector3 movement = new Vector3(_moveDirection.x, 0f, _moveDirection.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate to face movement direction if moving
        if (_moveDirection.sqrMagnitude > 0.001f)
        {
            Vector3 lookDirection = new(_moveDirection.x, 0f, _moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            rb.MoveRotation(targetRotation);
        }
    }
}
