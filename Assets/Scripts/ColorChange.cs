using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [Header("≈сли пусто Ч возьмЄм Renderer с этого объекта")]
    [SerializeField] private Renderer targetRenderer;

    [Tooltip("¬кл Ч безопаснее дл€ общих материалов (без их дублировани€). ¬ыкл Ч проще.")]
    [SerializeField] private bool usePropertyBlock = false;

    private Material runtimeMaterial;               // вариант попроще (instanced material)
    private MaterialPropertyBlock mpb;              // вариант безопасный (PropertyBlock)

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        if (!targetRenderer)
        {
            Debug.LogError("[PlayerColorSwitcher] Renderer не найден.");
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
            // —оздаст экземпл€р материала только дл€ этого Renderer
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
            // ѕишем и в _BaseColor (URP/HDRP), и в _Color (Built-in) Ч что-то одно точно сработает
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
