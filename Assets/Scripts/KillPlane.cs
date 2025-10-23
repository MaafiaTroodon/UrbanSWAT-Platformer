using UnityEngine;

public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // If it's the player
        var lives = other.GetComponentInParent<PlayerLives>();
        if (lives)
        {
            lives.KillAllLivesNow();
            return;
        }

        // For enemies/objects that just need to die
        var h = other.GetComponentInParent<Health>();
        if (h) h.TakeDamage(9999);
    }
}
