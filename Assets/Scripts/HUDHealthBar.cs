using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDHealthBar : MonoBehaviour
{
    public Health target;
    public Slider slider;        // min=0, max=1
    public TMP_Text percentText; // optional

    void Start()
    {
        if (!target) target = FindObjectOfType<PlayerControl>()?.GetComponent<Health>();
        Sync();
        if (target)
        {
            target.onDamaged.AddListener(Sync);
            target.onHealed.AddListener(Sync);
        }
    }

    void OnDestroy()
    {
        if (target)
        {
            target.onDamaged.RemoveListener(Sync);
            target.onHealed.RemoveListener(Sync);
        }
    }

    void Sync()
    {
        if (!target || !slider) return;
        float f = Mathf.Clamp01((float)target.currentHP / Mathf.Max(1, target.maxHP));
        slider.value = f;
        if (percentText) percentText.text = Mathf.RoundToInt(f * 100f) + "%";
    }
}

