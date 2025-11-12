using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Efecto de hover avanzado con trail de brillo y glow final
/// Recrea el efecto del HTML donde el brillo deja estela y el botón brilla al final
/// </summary>
public class MenuButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias")]
    [Tooltip("RectTransform del botón")]
    public RectTransform buttonRect;

    [Tooltip("Imagen del botón (background)")]
    public Image buttonImage;

    [Tooltip("Imagen del ícono")]
    public Image iconImage;

    [Header("Movimiento")]
    [Tooltip("Desplazamiento en hover")]
    public float moveDistance = 10f;

    [Tooltip("Velocidad de movimiento")]
    public float moveSpeed = 10f;

    [Header("Colores")]
    [Tooltip("Color normal del ícono")]
    public Color normalIconColor = new Color(0.18f, 0.8f, 0.44f, 1f);

    [Tooltip("Color del ícono en hover")]
    public Color hoverIconColor = Color.white;

    [Tooltip("Color del brillo trail")]
    public Color shineColor = new Color(1f, 1f, 1f, 0.2f);

    [Tooltip("Color del brillo final")]
    public Color finalGlowColor = new Color(1f, 1f, 1f, 0.3f);

    [Header("Efecto de Brillo")]
    [Tooltip("Duración del barrido")]
    public float shineDuration = 0.5f;

    [Tooltip("Duración del fade out final")]
    public float fadeOutDuration = 0.3f;

    [Tooltip("Ancho de la línea de brillo móvil")]
    public float shineLineWidth = 50f;

    private Vector2 originalPosition;
    private Vector2 targetPosition;

    // Overlays para el efecto
    private GameObject shineLineObject;
    private Image shineLineImage;

    private GameObject glowOverlay;
    private Image glowOverlayImage;

    void Start()
    {
        if (buttonRect == null)
            buttonRect = GetComponent<RectTransform>();

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (iconImage == null)
            iconImage = transform.Find("Icon")?.GetComponent<Image>();

        originalPosition = buttonRect.anchoredPosition;
        targetPosition = originalPosition;

        // Crear objetos de efecto
        CreateShineObjects();

        if (iconImage != null)
            iconImage.color = normalIconColor;
    }

    void Update()
    {
        buttonRect.anchoredPosition = Vector2.Lerp(
            buttonRect.anchoredPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPosition = originalPosition + new Vector2(moveDistance, 0);

        if (iconImage != null)
            iconImage.color = hoverIconColor;

        StartCoroutine(ShineTrailEffect());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPosition = originalPosition;

        if (iconImage != null)
            iconImage.color = normalIconColor;

        StopAllCoroutines();

        // Resetear efectos
        if (shineLineImage != null)
            shineLineImage.enabled = false;

        if (glowOverlayImage != null)
            glowOverlayImage.enabled = false;
    }

    void CreateShineObjects()
    {
        // 1. Línea de brillo móvil
        shineLineObject = new GameObject("ShineLine");
        shineLineObject.transform.SetParent(buttonImage.transform, false);

        shineLineImage = shineLineObject.AddComponent<Image>();
        shineLineImage.color = shineColor;
        shineLineImage.raycastTarget = false;

        RectTransform shineLineRect = shineLineObject.GetComponent<RectTransform>();
        shineLineRect.anchorMin = new Vector2(0, 0);
        shineLineRect.anchorMax = new Vector2(0, 1);
        shineLineRect.pivot = new Vector2(0.5f, 0.5f);
        shineLineRect.sizeDelta = new Vector2(shineLineWidth, 0);

        shineLineImage.enabled = false;

        // 2. Overlay de brillo completo (el que queda al final)
        glowOverlay = new GameObject("GlowOverlay");
        glowOverlay.transform.SetParent(buttonImage.transform, false);

        glowOverlayImage = glowOverlay.AddComponent<Image>();
        glowOverlayImage.color = finalGlowColor;
        glowOverlayImage.raycastTarget = false;

        RectTransform glowRect = glowOverlay.GetComponent<RectTransform>();
        glowRect.anchorMin = Vector2.zero;
        glowRect.anchorMax = Vector2.one;
        glowRect.offsetMin = Vector2.zero;
        glowRect.offsetMax = Vector2.zero;

        glowOverlayImage.enabled = false;
    }

    IEnumerator ShineTrailEffect()
    {
        float buttonWidth = buttonRect.rect.width;

        // --- FASE 1: Barrido con trail ---
        shineLineImage.enabled = true;
        glowOverlayImage.enabled = true;

        RectTransform shineLineRect = shineLineObject.GetComponent<RectTransform>();

        float startX = 0f;
        float endX = buttonWidth;
        float elapsed = 0f;

        while (elapsed < shineDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shineDuration;

            // Mover la línea de brillo
            float currentX = Mathf.Lerp(startX, endX, t);
            shineLineRect.anchoredPosition = new Vector2(currentX, 0);

            // CLAVE: El glow overlay va apareciendo gradualmente
            // simulando que el brillo "pinta" el botón
            Color glowColor = finalGlowColor;
            glowColor.a = finalGlowColor.a * t; // Fade in progresivo
            glowOverlayImage.color = glowColor;

            yield return null;
        }

        // --- FASE 2: Línea desaparece, queda el glow completo ---
        shineLineImage.enabled = false;

        // El botón queda brillando
        glowOverlayImage.color = finalGlowColor;

        // Esperar un momento
        yield return new WaitForSeconds(0.1f);

        // --- FASE 3: Fade out del glow ---
        elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;

            Color glowColor = finalGlowColor;
            glowColor.a = finalGlowColor.a * (1f - t); // Fade out
            glowOverlayImage.color = glowColor;

            yield return null;
        }

        // Desactivar
        glowOverlayImage.enabled = false;
    }
}