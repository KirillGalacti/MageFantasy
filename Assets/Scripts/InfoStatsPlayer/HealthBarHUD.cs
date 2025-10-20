using UnityEngine;
using UnityEngine.UI;

public class HealthBarHUD : MonoBehaviour
{
    [SerializeField] private Image _fillImage;   // Image: Type = Filled
    [SerializeField] private float _smooth = 0f; // 0 — без анимации

    private PlayerHealth _target;
    private float _shown;

    public void Bind(PlayerHealth target)
    {
        _target = target;
        _shown = _target != null ? _target.NormalizedHP : 0f;
        if (_fillImage) _fillImage.fillAmount = _shown;
        // Debug.Log("HUD bound to " + (_target ? _target.name : "null"));
    }

    void Update()
    {
        if (_target == null || _fillImage == null) return;

        float t = _target.NormalizedHP;
        _shown = (_smooth > 0f) ? Mathf.MoveTowards(_shown, t, _smooth * Time.deltaTime) : t;
        _fillImage.fillAmount = _shown;
    }
}
