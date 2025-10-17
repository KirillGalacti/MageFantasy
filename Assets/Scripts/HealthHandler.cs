using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private Image[] _icons;
    private int _i;

    void Update()
    {
        if (!isLocalPlayer) return;               // жмём H только на своём клиенте
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (_i < _icons.Length) Destroy(_icons[_i++].gameObject);
            if (_i >= _icons.Length) CmdKillMe(); // просим сервер удалить игрока
        }
    }

    [Command]
    void CmdKillMe()
    {
        NetworkServer.Destroy(gameObject);        // удаляет объект на сервере → у всех клиентов
    }
}