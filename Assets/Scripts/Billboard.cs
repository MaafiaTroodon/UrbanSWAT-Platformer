using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;

    void Awake() => cam = Camera.main ? Camera.main.transform : null;

    void LateUpdate()
    {
        if (!cam) return;
        // Face the camera: vector must be camera - object (not the other way around)
        Vector3 dir = cam.position - transform.position;
        dir.y = 0f; // keep upright
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
