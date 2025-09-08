using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthFill;         // arrastrá aquí la Image "Fill"
    public Transform target;         // enemy o empty sobre la cabeza
    public Vector3 offset = new Vector3(0, 2f, 0);
    public Camera cam;               // opcional, si lo dejás vacío usa Camera.main

    // ajustes por si algo está mal en el Canvas
    public Vector2 defaultSize = new Vector2(150, 30);
    public Vector3 defaultScale = new Vector3(0.01f, 0.01f, 0.01f);

    Canvas canvas;
    RectTransform rt;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (cam == null) cam = Camera.main;

        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>(true);

        if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning("[EnemyHealthBar] Canvas no estaba en World Space. Lo cambio automáticamente.");
            canvas.renderMode = RenderMode.WorldSpace;
        }

        if (rt != null)
        {
            if (Mathf.Approximately(rt.sizeDelta.x, 0) || Mathf.Approximately(rt.sizeDelta.y, 0))
            {
                rt.sizeDelta = defaultSize;
                Debug.Log("[EnemyHealthBar] RectTransform size ajustado: " + defaultSize);
            }

            if (transform.localScale.magnitude < 0.0001f)
            {
                transform.localScale = defaultScale;
                Debug.Log("[EnemyHealthBar] Scale del Canvas estaba demasiado chica. La ajusté a " + defaultScale);
            }
        }

        // intentar auto-asignar healthFill si no está puesta
        if (healthFill == null)
        {
            Image[] imgs = GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                if (img.name.ToLower().Contains("fill") || img.type == Image.Type.Filled)
                {
                    healthFill = img;
                    break;
                }
            }
            if (healthFill == null && imgs.Length > 0)
                healthFill = imgs[0];
        }

        if (healthFill == null)
            Debug.LogError("[EnemyHealthBar] healthFill NO asignada. Arrastrala en el Inspector.");
        else
        {
            // asegurar que sea visible al inicio
            healthFill.enabled = true;
            if (healthFill.type != Image.Type.Filled)
                Debug.LogWarning("[EnemyHealthBar] healthFill no es 'Filled'. El script tiene fallback, pero recomendable cambiar a 'Filled' (Image Type).");

            healthFill.fillAmount = 1f;
        }

        gameObject.SetActive(true); // que esté activo desde el arranque
    }

    void LateUpdate()
    {
        if (target == null) return;

        // si el canvas es hijo del target, usamos localPosition (más estable)
        if (transform.IsChildOf(target))
            transform.localPosition = offset;
        else
            transform.position = target.position + offset;

        if (cam != null)
            transform.forward = transform.position - cam.transform.position; // siempre mirar a la cámara
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthFill == null) return;

        float pct = Mathf.Clamp01((float)currentHealth / maxHealth);

        if (healthFill.type == Image.Type.Filled)
        {
            healthFill.fillAmount = pct;
        }
        else
        {
            // fallback: modificar el ancho del Fill si no está en 'Filled'
            RectTransform bg = healthFill.transform.parent.GetComponent<RectTransform>();
            RectTransform fillRt = healthFill.rectTransform;
            if (bg != null)
            {
                float w = bg.rect.width * pct;
                fillRt.sizeDelta = new Vector2(w, fillRt.sizeDelta.y);
            }
        }
        Debug.Log($"[EnemyHealthBar] Update -> {currentHealth}/{maxHealth} ({pct * 100:0.0}%)");
    }
}
