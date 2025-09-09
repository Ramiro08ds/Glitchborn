using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthFill;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    public Camera cam;

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

        // asegurarse de que el canvas esté en World Space
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cam; // importante
        }

        if (rt != null)
        {
            // ajustar tamaño si es 0
            if (Mathf.Approximately(rt.sizeDelta.x, 0) || Mathf.Approximately(rt.sizeDelta.y, 0))
                rt.sizeDelta = new Vector2(150, 30);

            if (transform.localScale.magnitude < 0.0001f)
                transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }

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

        if (healthFill != null)
        {
            healthFill.enabled = true;
            healthFill.fillAmount = 1f;
        }

        gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // si el canvas es hijo del target, usar localPosition
        if (transform.IsChildOf(target))
            transform.localPosition = offset;
        else
            transform.position = target.position + offset;

        // siempre mirar a la cámara
        if (cam != null)
            transform.forward = transform.position - cam.transform.position;
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthFill == null) return;

        float pct = Mathf.Clamp01((float)currentHealth / maxHealth);

        if (healthFill.type == Image.Type.Filled)
            healthFill.fillAmount = pct;
        else
        {
            RectTransform bg = healthFill.transform.parent.GetComponent<RectTransform>();
            RectTransform fillRt = healthFill.rectTransform;
            if (bg != null)
            {
                float w = bg.rect.width * pct;
                fillRt.sizeDelta = new Vector2(w, fillRt.sizeDelta.y);
            }
        }
    }
}

