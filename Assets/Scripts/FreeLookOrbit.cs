using UnityEngine;

public class FreeLookOrbit : MonoBehaviour
{
    public Transform target;                  // your Player transform
    public Vector2 sensitivity = new(180,120); // deg/sec for X/Y
    public float minPitch = -30f, maxPitch = 70f;

    [Header("Distance / Zoom")]
    public float distance = 6f;
    public float minDistance = 2f, maxDistance = 10f;
    public float zoomSpeed = 5f;

    float yaw, pitch;

    void Start()
    {
        if (!target) target = FindObjectOfType<PlayerControl>()?.transform;
        var e = transform.rotation.eulerAngles;
        yaw = e.y; pitch = e.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // mouse delta
        yaw   += Input.GetAxis("Mouse X") * sensitivity.x * Time.unscaledDeltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity.y * Time.unscaledDeltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        // zoom with wheel
        float dz = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance + dz, minDistance, maxDistance);

        // place camera
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rot * new Vector3(0, 0, -distance);
        Vector3 desired = target ? target.position + offset : transform.position;

        transform.SetPositionAndRotation(desired, rot);
    }
}
