using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    void LateUpdate()
    {
        // Make the object face the camera
        transform.LookAt(transform.position + cam.forward);
    }
}
