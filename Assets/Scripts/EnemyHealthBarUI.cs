using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBarUI : MonoBehaviour
{
    public Health target;
    public Slider slider;             // 0..1
    public TMP_Text percentText;      // optional
    public Canvas worldCanvas;        // world-space

    public bool hideWhenFull = true;
    public float faceCameraLerp = 12f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (!worldCanvas) worldCanvas = GetComponentInChildren<Canvas>(true);
        if (target)
        {
            target.onDamaged.AddListener(Sync);
            target.onHealed.AddListener(Sync);
        }
        Sync();
    }

    void LateUpdate()
    {
        if (!cam || !worldCanvas) return;
        // billboard
        worldCanvas.transform.rotation = Quaternion.Slerp(
            worldCanvas.transform.rotation,
            Quaternion.LookRotation(worldCanvas.transform.position - cam.transform.position),
            Time.deltaTime * faceCameraLerp
        );
    }

    void Sync()
    {
        if (!target || !slider) return;
        float f = Mathf.Clamp01((float)target.currentHP / Mathf.Max(1, target.maxHP));
        slider.value = f;
        if (percentText) percentText.text = Mathf.RoundToInt(f * 100f) + "%";
        if (worldCanvas && hideWhenFull) worldCanvas.enabled = f < 1f && !target.IsDead;
    }
}
