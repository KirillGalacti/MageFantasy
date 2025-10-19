using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [Header("World-Space UI (иконка/полоса над игроком)")]
    [SerializeField] private UnityEngine.UI.Image _fillImage; // можно оставить пустым, если над головой нет полосы

    [Header("Health Settings")]
    [SerializeField] private float _maxHealthPoint = 100f;
    [SerializeField] private float _damagePerPress = 10f;

    // ВЕРНУЛИ hook, чтобы обновлять world-space полосу у всех клиентов
    [SyncVar(hook = nameof(OnHealthChanged))]
    private float _currentHealthPoint;

    public float NormalizedHP => _maxHealthPoint > 0f ? _currentHealthPoint / _maxHealthPoint : 0f;

    public override void OnStartServer()
    {
        _currentHealthPoint = _maxHealthPoint;
    }

    public override void OnStartLocalPlayer()
    {
        // Привязываем локальный HUD на камере
        var hud = FindObjectOfType<HealthBarHUD>(true);
        if (hud) hud.Bind(this);
    }

    public override void OnStartClient()
    {
        // При подключении клиента сразу обновим world-space полосу (если она есть)
        OnHealthChanged(_currentHealthPoint, _currentHealthPoint);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.H))
            CmdTakeDamage(_damagePerPress);
    }

    [Command]
    private void CmdTakeDamage(float amount)
    {
        _currentHealthPoint = Mathf.Clamp(_currentHealthPoint - amount, 0f, _maxHealthPoint);
    }

    // Хук вызывается на ВСЕХ клиентах при изменении _currentHealthPoint
    private void OnHealthChanged(float oldValue, float newValue)
    {
        if (_fillImage != null)
            _fillImage.fillAmount = _maxHealthPoint > 0f ? newValue / _maxHealthPoint : 0f;
    }
}
