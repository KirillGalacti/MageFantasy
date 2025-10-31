using UnityEngine;
using Mirror;

[RequireComponent(typeof(Collider))]
public class ButtonEffect : NetworkBehaviour
{
    [Header("Искры")]
    [SerializeField] private GameObject _sparks;

    [Header("Точка появления (опционально)")]
    [SerializeField] private Transform _pointSparks;

    private void Awake()
    {
        if (!_sparks)
        {
            Debug.LogError("[ButtonEffect] Particle System не установлен. Скрипт отключен.");
            enabled = false;
            return;
        }

        if (!_pointSparks)
        {
            Debug.LogWarning("[ButtonEffect] Точка создания не задана. Будет использована позиция персонажа.");
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E)) Trigger();
    }

    private void Trigger()
    {
        Vector3 spawnPos = _pointSparks ? _pointSparks.position : transform.position;

        if (isServer)
        {
            // Хост: сразу рассылаем всем
            PlaySpark(spawnPos);
        }
        else
        {
            // Клиент: просим сервер разослать всем
            CmdRequestEffect(spawnPos);
        }
    }

    [Command]
    private void CmdRequestEffect(Vector3 worldPos)
    {
        // Сервер -> всем клиентам
        PlaySpark(worldPos);
    }

    [Server]
    private void PlaySpark(Vector3 pos)
    {
        if (_sparks == null) return;

        GameObject sparkSpawn = Instantiate(
            original: _sparks, 
            position: pos, 
            rotation: Quaternion.identity
            );

        NetworkServer.Spawn(sparkSpawn);

    }
}
