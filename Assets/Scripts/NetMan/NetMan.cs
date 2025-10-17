using UnityEngine;
using Mirror;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NetMan : NetworkManager
{
    private bool _playerSpawned;
    private bool _playerConnected;

    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message)
    {
        GameObject go = Instantiate(playerPrefab, message.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, go);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        _playerConnected = true;
    }

    // Избегаем предупреждения про скрытие Update
    private void LateUpdate()
    {
        if (!_playerSpawned && _playerConnected && ClickedThisFrame())
        {
            ActivatePlayerSpawn();
        }
    }

    private bool ClickedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Mouse0);
#endif
    }

    private Vector3 GetMouseScreenPosition()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null
            ? (Vector3)Mouse.current.position.ReadValue()
            : Vector3.zero;
#else
        return Input.mousePosition;
#endif
    }

    public void ActivatePlayerSpawn()
    {
        Vector3 mouse = GetMouseScreenPosition();
        mouse.z = 10f; // расстояние до плоскости спавна от камеры
        Vector3 world = Camera.main.ScreenToWorldPoint(mouse);

        PosMessage msg = new PosMessage { position = world };
        NetworkClient.Send(msg);

        _playerSpawned = true;
    }
}

public struct PosMessage : NetworkMessage
{
    public Vector3 position;
}
