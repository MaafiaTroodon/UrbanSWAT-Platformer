using UnityEngine;

public class SpinBob : MonoBehaviour
{
    public float spinSpeed = 120f;
    public float bobAmplitude = 0.12f;
    public float bobSpeed = 2f;

    Vector3 startLocalPos;

    void Start() => startLocalPos = transform.localPosition;

    void Update()
    {
        // spin in world so it’s always visible
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

        // gentle up-down
        var p = startLocalPos;
        p.y += Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = p;
    }
}
