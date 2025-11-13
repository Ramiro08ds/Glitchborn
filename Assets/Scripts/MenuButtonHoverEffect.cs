using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Efecto completo del botón:
/// - Shine se mueve con entrada/salida gradual
/// - Fondo se revela progresivamente (izq→der / der→izq)
/// - Borde se ilumina
/// - Glow aparece alrededor del botón
/// </summary>
public class MenuButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("=== REFERENCIAS ===")]
    [Tooltip("Imagen del fondo del botón")]
    public Image buttonBackground;

    [Tooltip("Imagen del borde del botón")]
    public Image buttonBorder;

    [Tooltip("Imagen del glow (detrás del botón)")]
    public Image buttonGlow;

    [Tooltip("Sprite del shine")]
    public Sprite shineSprite;

    [Header("=== MOVIMIENTO ===")]
    [Tooltip("Distancia de desplazamiento")]
    public float moveDistance = 15f;

    [Tooltip("Duración del movimiento (debe ser igual a shineDuration)")]
    public float moveDuration = 0.5f;

    [Tooltip("Tipo de easing para el movimiento")]
    public AnimationCurve moveEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("=== COLORES DEL FONDO ===")]
    public Color backgroundNormal = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Color backgroundHover = new Color(0.1f, 0.1f, 0.1f, 0f);

    [Header("=== COLORES DEL BORDE ===")]
    public Color borderNormal = new Color(0.18f, 0.8f, 0.44f, 0.3f);
    public Color borderGlow = new Color(0.18f, 0.8f, 0.44f, 1f);

    [Header("=== GLOW ALREDEDOR ===")]
    [Tooltip("Color del glow (mismo que borde brillante pero con alpha)")]
    public Color glowColorNormal = new Color(0.18f, 0.8f, 0.44f, 0f); // Invisible

    [Tooltip("Color del glow brillante")]
    public Color glowColorBright = new Color(0.18f, 0.8f, 0.44f, 0.6f); // Verde brillante

    [Header("=== CONFIGURACIÓN DEL SHINE ===")]
    public float shineDuration = 0.5f;
    public float shineWidth = 80f;
    public Color shineColor = new Color(0.18f, 0.8f, 0.44f, 1f);

    [Range(0f, 1f)]
    public float shineOpacity = 0.8f;

    private RectTransform buttonRect;
    private Vector2 originalPosition;

    private GameObject shineContainer;
    private GameObject shineObject;
    private Image shineImage;
    private RectTransform shineRect;

    private Coroutine currentAnimation;
    private Coroutine moveCoroutine;

    void Start()
    {
        buttonRect = GetComponent<RectTransform>();

        // Auto-referencias
        if (buttonBackground == null)
        {
            buttonBackground = transform.Find("Background")?.GetComponent<Image>();
            if (buttonBackground == null)
                buttonBackground = GetComponent<Image>();
        }

        if (buttonBorder == null)
            buttonBorder = transform.Find("Border")?.GetComponent<Image>();

        if (buttonGlow == null)
            buttonGlow = transform.Find("Glow")?.GetComponent<Image>();

        originalPosition = buttonRect.anchoredPosition;

        // Estado inicial
        if (buttonBackground != null)
            buttonBackground.color = backgroundNormal;

        if (buttonBorder != null)
            buttonBorder.color = borderNormal;

        if (buttonGlow != null)
        {
            buttonGlow.color = glowColorNormal; // Empieza invisible
        }

        CreateShine();
    }

    void OnValidate()
    {
        // Actualizar color del shine cuando se cambia en el Inspector
        if (Application.isPlaying && shineImage != null)
        {
            Color color = shineColor;
            color.a = shineOpacity;
            shineImage.color = color;
        }

        // Actualizar border color si está en estado normal
        if (Application.isPlaying && buttonBorder != null && !IsHovered())
        {
            buttonBorder.color = borderNormal;
        }
    }

    bool IsHovered()
    {
        return currentAnimation != null;
    }

    void CreateShine()
    {
        if (buttonBackground == null) return;

        // Crear nombre único para este botón
        string uniqueName = "ShineContainer_" + gameObject.GetInstanceID();

        // Verificar que no exista ya
        Transform existingContainer = buttonBackground.transform.Find(uniqueName);
        if (existingContainer != null)
        {
            DestroyImmediate(existingContainer.gameObject);
        }

        // Contenedor con máscara
        shineContainer = new GameObject(uniqueName);
        shineContainer.transform.SetParent(buttonBackground.transform, false);

        RectMask2D rectMask = shineContainer.AddComponent<RectMask2D>();

        RectTransform containerRect = shineContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Shine dentro del contenedor con nombre único
        shineObject = new GameObject("Shine_" + gameObject.GetInstanceID());
        shineObject.transform.SetParent(shineContainer.transform, false);

        shineImage = shineObject.AddComponent<Image>();
        shineImage.sprite = shineSprite;

        Color color = shineColor;
        color.a = shineOpacity;
        shineImage.color = color;
        shineImage.raycastTarget = false;

        shineRect = shineObject.GetComponent<RectTransform>();
        shineRect.anchorMin = new Vector2(0, 0);
        shineRect.anchorMax = new Vector2(0, 1);
        shineRect.pivot = new Vector2(0.5f, 0.5f);
        shineRect.sizeDelta = new Vector2(shineWidth, 0);
        shineRect.anchoredPosition = Vector2.zero;

        shineImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Detener animaciones anteriores
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        // Iniciar animación de hover y movimiento simultáneo
        currentAnimation = StartCoroutine(HoverIn());
        moveCoroutine = StartCoroutine(SmoothMove(originalPosition + new Vector2(moveDistance, 0), moveDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Detener animaciones anteriores
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        // Iniciar animación de hover out y movimiento simultáneo
        currentAnimation = StartCoroutine(HoverOut());
        moveCoroutine = StartCoroutine(SmoothMove(originalPosition, moveDuration));
    }

    /// <summary>
    /// Mueve el botón suavemente con easing
    /// </summary>
    IEnumerator SmoothMove(Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = buttonRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Aplicar easing curve para movimiento suave
            float easedT = moveEasing.Evaluate(t);

            buttonRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedT);

            yield return null;
        }

        // Asegurar posición final
        buttonRect.anchoredPosition = targetPosition;
        moveCoroutine = null;
    }

    IEnumerator HoverIn()
    {
        if (shineImage == null || buttonBackground == null) yield break;

        float buttonWidth = buttonRect.rect.width;
        shineImage.enabled = true;

        float elapsed = 0f;

        // Posiciones del reveal progresivo
        float revealStartX = 0f;

        while (elapsed < shineDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shineDuration;

            // 1. SHINE se mueve (entra y sale)
            float shineX = Mathf.Lerp(-shineWidth / 2, buttonWidth + shineWidth / 2, t);
            shineRect.anchoredPosition = new Vector2(shineX, 0);

            // 2. FONDO se transparenta PROGRESIVAMENTE de izquierda a derecha
            // Calculamos qué parte del fondo ya "pasó" el shine
            float revealProgress = Mathf.Clamp01((shineX - revealStartX) / buttonWidth);

            // Interpolamos solo la parte revelada
            Color currentBgColor = buttonBackground.color;
            currentBgColor = Color.Lerp(backgroundNormal, backgroundHover, revealProgress);
            buttonBackground.color = currentBgColor;

            // 3. BORDE se ilumina
            if (buttonBorder != null)
                buttonBorder.color = Color.Lerp(borderNormal, borderGlow, t);

            // 4. GLOW aparece GRADUALMENTE junto con el borde
            if (buttonGlow != null)
            {
                buttonGlow.color = Color.Lerp(glowColorNormal, glowColorBright, t);
            }

            yield return null;
        }

        // Estado final
        buttonBackground.color = backgroundHover;
        if (buttonBorder != null)
            buttonBorder.color = borderGlow;
        if (buttonGlow != null)
            buttonGlow.color = glowColorBright;

        shineImage.enabled = false;
        currentAnimation = null;
    }

    IEnumerator HoverOut()
    {
        if (shineImage == null || buttonBackground == null) yield break;

        float buttonWidth = buttonRect.rect.width;
        shineImage.enabled = true;

        float elapsed = 0f;

        while (elapsed < shineDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shineDuration;

            // 1. SHINE se mueve de derecha a izquierda (inverso)
            float shineX = Mathf.Lerp(buttonWidth + shineWidth / 2, -shineWidth / 2, t);
            shineRect.anchoredPosition = new Vector2(shineX, 0);

            // 2. FONDO vuelve a negro PROGRESIVAMENTE de derecha a izquierda
            float revealProgress = Mathf.Clamp01((buttonWidth - shineX) / buttonWidth);

            Color currentBgColor = buttonBackground.color;
            currentBgColor = Color.Lerp(backgroundHover, backgroundNormal, revealProgress);
            buttonBackground.color = currentBgColor;

            // 3. BORDE se apaga
            if (buttonBorder != null)
                buttonBorder.color = Color.Lerp(borderGlow, borderNormal, t);

            // 4. GLOW desaparece GRADUALMENTE junto con el borde
            if (buttonGlow != null)
            {
                buttonGlow.color = Color.Lerp(glowColorBright, glowColorNormal, t);
            }

            yield return null;
        }

        // Estado final
        buttonBackground.color = backgroundNormal;
        if (buttonBorder != null)
            buttonBorder.color = borderNormal;
        if (buttonGlow != null)
            buttonGlow.color = glowColorNormal;

        shineImage.enabled = false;
        currentAnimation = null;
    }
}