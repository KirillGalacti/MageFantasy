using UnityEngine;
using Mirror;

public class ColorChange : NetworkBehaviour
{
    [SerializeField] private Renderer _targetRenderer;

    [SerializeField] private Material _material;

    // Сервер хранит правду про цвет и рассылает всем.
    [SyncVar(hook = nameof(OnColorChanged))]
    private Color syncedColor = Color.white;

    private void Awake()
    {
        _targetRenderer = GetComponent<Renderer>();
        if (!_targetRenderer)
        {
            Debug.LogError("[ChangeColor] Renderer не найден. Скрипт отключен.");
            enabled = false;
            return;
        }
        _material = _targetRenderer.material; // свой инстанс материала для этого Renderer
    }

    public override void OnStartClient()
    {
        Apply(syncedColor); // при подключении сразу поставить актуальный цвет
    }

    private void Update()
    {
        if (!isLocalPlayer) return; // слушаем клавиши только у владельца

        if (Input.GetKeyDown(KeyCode.R)) SetColor(Color.red);
        if (Input.GetKeyDown(KeyCode.G)) SetColor(Color.green);
        if (Input.GetKeyDown(KeyCode.B)) SetColor(Color.blue);
    }

    // если мы сервер — пишем SyncVar, иначе шлём команду.
    private void SetColor(Color c)
    {
        if (isServer)
        {
            syncedColor = c;
        }
        else
        {
            CmdSetColor(c);
        }
    }

    [Command]
    private void CmdSetColor(Color c)
    {
        syncedColor = c;
    }

    // Mirror вызывает на всех, когда syncedColor поменялся.
    private void OnColorChanged(Color _, Color newC)
    {
        Apply(newC);
    }

    private void Apply(Color c)
    {
        _material.color = c;
    }
}
