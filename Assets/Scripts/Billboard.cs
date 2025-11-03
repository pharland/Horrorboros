using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform camera;

    void LateUpdate()
    {
        transform.LookAt(transform.position + camera.forward);
    }
}
