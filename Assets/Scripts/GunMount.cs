using UnityEngine;

public class GunMount : MonoBehaviour
{
    [Header("Assignments")]
    public Animator anim;                 // drag the SWAT Animator here (or auto-finds)
    public GameObject pistolPrefab;       // drag a pistol prefab/FBX here

    [Header("Offsets in hand (tweak in Play Mode)")]
    public Vector3 localPos = new Vector3(0.035f, -0.020f, 0.020f);
    public Vector3 localEuler = new Vector3(0f, 90f, 0f);

    [Tooltip("Uniform scale override for the pistol under the hand")]
    public float prefabScale = 0.01f;     // 0.01–0.05 usually looks right for these packs

    void Start()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
        if (!anim || !pistolPrefab) return;

        // Get humanoid right hand
        Transform hand = anim.GetBoneTransform(HumanBodyBones.RightHand);
        if (!hand) { Debug.LogWarning("RightHand bone not found"); return; }

        // Spawn pistol as a child of the hand
        var pistol = Instantiate(pistolPrefab, hand);
        pistol.transform.localPosition = localPos;
        pistol.transform.localRotation = Quaternion.Euler(localEuler);
        pistol.transform.localScale = Vector3.one * prefabScale;

        // Make sure no physics interfere
        foreach (var rb in pistol.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);
        foreach (var mc in pistol.GetComponentsInChildren<MeshCollider>())
        {
            mc.convex = true;
            mc.isTrigger = true;
        }
    }
}
