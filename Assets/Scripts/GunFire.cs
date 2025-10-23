using UnityEngine;

public class GunFire : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;                  // Main Camera
    public Transform firePoint;         // small child at muzzle
    public ParticleSystem muzzleFlash;  // particle system
    public AudioSource shotAudio;       // audio with a shot clip

    [Header("Ballistics")]
    public float damage = 25f;
    public float range = 150f;
    public LayerMask hitMask = ~0;      // Everything by default
    public float fireRate = 7f;         // shots per second
    public float bulletSpread = 0.5f;   // degrees

    [Header("Impact (optional)")]
    public GameObject hitImpactPrefab;  // decal / spark

    float _nextShootTime;

    void Update()
    {
        if (Input.GetButton("Fire1") /* left mouse by default */ && Time.time >= _nextShootTime)
        {
            Shoot();
            _nextShootTime = Time.time + 1f / Mathf.Max(0.01f, fireRate);
        }
    }

    void Shoot()
    {
        if (muzzleFlash) muzzleFlash.Play(true);
        if (shotAudio)   shotAudio.Play();

        // Direction from camera, with tiny random spread
        Vector3 dir = cam.transform.forward;
        if (bulletSpread > 0f)
        {
            Vector2 r = Random.insideUnitCircle * bulletSpread * Mathf.Deg2Rad;
            dir = (cam.transform.rotation * Quaternion.Euler(r.y * Mathf.Rad2Deg, r.x * Mathf.Rad2Deg, 0f)) * Vector3.forward;
        }

        // Raycast from the camera for aim, draw a debug line from the gun
        Vector3 start = (firePoint ? firePoint.position : cam.transform.position);
        Vector3 end   = cam.transform.position + dir * range;

        if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;

            if (hitImpactPrefab)
                Instantiate(hitImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));

            // TODO: apply damage if the hit object has a health script
            // var hp = hit.collider.GetComponent<Health>();
            // if (hp) hp.Take(damage);
        }

        Debug.DrawLine(start, end, Color.yellow, 0.2f);
    }
}
