using UnityEngine;
public class Billboardportal : MonoBehaviour {
    Camera cam; void Start(){ cam = Camera.main; }
    void LateUpdate(){ if (cam) transform.LookAt(transform.position + cam.transform.forward); }
}
