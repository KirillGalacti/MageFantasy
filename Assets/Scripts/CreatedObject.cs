using UnityEngine;
using Mirror;

public class CreatedObject : NetworkBehaviour
{
    [Header("Объект, который спавним")]
    [SerializeField] private GameObject _createdObject;

    [Header("Точка, где объект появится")]
    [SerializeField] private Transform _pointSpawn;

    private void Awake()
    {
        if (!_createdObject)
        {
            Debug.LogError("[CreatedObject] Создаваемый объект не задан. Скрипт отключён.");
            enabled = false;
            return;
        }

        if (!_pointSpawn)
            Debug.LogWarning("[CreatedObject] Точка создания не задана. Будет использован transform игрока.");
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.T)) TriggerObject();
    }

    private void TriggerObject()
    {
        Vector3 pos = _pointSpawn ? _pointSpawn.position : transform.position;

        if (isServer)
        {
            ServerSpawn(pos);
        }
        else
        {
            CmdRequestCreated(pos);
        }
    }

    [Command]
    private void CmdRequestCreated(Vector3 pos)
    {
        ServerSpawn(pos);
    }

    [Server] // только на сервере
    private void ServerSpawn(Vector3 pos)
    {
        if (_createdObject == null) return;

        GameObject cube = Instantiate(
            original: _createdObject, 
            position: pos, 
            rotation: Quaternion.identity
            );
        NetworkServer.Spawn(cube); // рассылается всем, поздние клиенты тоже получат
    }
}
