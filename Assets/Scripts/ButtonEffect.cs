using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ButtonEffect : MonoBehaviour
{
    [Header("Искры")]
    [SerializeField] private ParticleSystem[] _sparks;   // можно одну или несколько
    [SerializeField] private Transform _pointSparks;      // точка появления (опционально)

    [Header("Фильтр активации")]
    [SerializeField] private string _playerTag = "Player";

    private int _playerOverlapCount = 0;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;

        StopAllEffects(); // на старте искры выключены
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        _playerOverlapCount++;
        if (_playerOverlapCount == 1)
            PlayAllEffects();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        _playerOverlapCount = Mathf.Max(0, _playerOverlapCount - 1);
        if (_playerOverlapCount == 0)
            StopAllEffects();
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag(_playerTag);
    }

    private void PlayAllEffects()
    {
        if (_sparks == null) return;

        foreach (var ps in _sparks)
        {
            if (ps == null) continue;

            // Если задана точка — телепортируем эмиттеры в неё
            if (_pointSparks != null)
                ps.transform.position = _pointSparks.position;

            var emission = ps.emission;
            emission.enabled = true;

            if (!ps.isPlaying)
                ps.Play(true);
        }
    }

    private void StopAllEffects()
    {
        if (_sparks == null) return;

        foreach (var ps in _sparks)
        {
            if (ps == null) continue;

            // мгновенно гасим и чистим «хвост»
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var emission = ps.emission;
            emission.enabled = false;
        }
    }
}
