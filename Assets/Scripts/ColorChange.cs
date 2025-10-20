using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [Header("���� ����� � ������ Renderer � ����� �������")]
    [SerializeField] private Renderer targetRenderer;

    [Tooltip("��� � ���������� ��� ����� ���������� (��� �� ������������). ���� � �����.")]
    [SerializeField] private bool usePropertyBlock = false;

    private Material runtimeMaterial;               // ������� ������� (instanced material)
    private MaterialPropertyBlock mpb;              // ������� ���������� (PropertyBlock)

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        if (!targetRenderer)
        {
            Debug.LogError("[PlayerColorSwitcher] Renderer �� ������.");
            enabled = false;
            return;
        }

        if (usePropertyBlock)
        {
            mpb = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(mpb);
        }
        else
        {
            // ������� ��������� ��������� ������ ��� ����� Renderer
            runtimeMaterial = targetRenderer.material;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SetColor(Color.red);
        if (Input.GetKeyDown(KeyCode.G)) SetColor(Color.green);
        if (Input.GetKeyDown(KeyCode.B)) SetColor(Color.blue);
    }

    private void SetColor(Color c)
    {
        if (usePropertyBlock)
        {
            // ����� � � _BaseColor (URP/HDRP), � � _Color (Built-in) � ���-�� ���� ����� ���������
            mpb.SetColor(BaseColorID, c);
            mpb.SetColor(ColorID, c);
            targetRenderer.SetPropertyBlock(mpb);
        }
        else
        {
            runtimeMaterial.color = c;
        }
    }
}
