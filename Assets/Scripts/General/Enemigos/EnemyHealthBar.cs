using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public Image healthFill;
    public Transform target;
    public Camera cam;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 4f, 0);

    private Canvas canvas;
    private RectTransform rt;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (cam == null) cam = Camera.main;

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cam;
        }

        if (rt != null)
        {
            if (rt.sizeDelta == Vector2.zero)
                rt.sizeDelta = new Vector2(150, 30);

            transform.localScale = Vector3.one * 0.01f;
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
            healthFill.fillAmount = 1f;
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        // Posición sobre la cabeza con offset
        transform.position = target.position + offset;

        // Que siempre mire a la cámara
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthFill == null) return;

        float pct = Mathf.Clamp01((float)currentHealth / maxHealth);
        healthFill.fillAmount = pct;
    }
}
