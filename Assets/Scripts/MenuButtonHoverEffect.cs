using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Efecto de botón del menú - VERSIÓN SIMPLE Y FUNCIONAL
/// - Shine se mueve y entra/sale gradualmente (clipeado)
/// - Fondo cambia de negro a transparente
/// - Borde se ilumina
/// </summary>
public class MenuButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("=== REFERENCIAS ===")]
    [Tooltip("La imagen del fondo del botón")]
    public Image buttonBackground;

    [Tooltip("La imagen del borde del botón")]
    public Image buttonBorder;

    [Tooltip("Sprite del shine")]
    public Sprite shineSprite;

    [Header("=== MOVIMIENTO ===")]
    public float moveDistance = 10f;
    public float moveSpeed = 10f;

    [Header("=== COLORES DEL FONDO ===")]
    public Color backgroundNormal = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Color backgroundHover = new Color(0.1f, 0.1f, 0.1f, 0f);

    [Header("=== COLORES DEL BORDE ===")]
    public Color borderNormal = new Color(0.18f, 0.8f, 0.44f, 0.3f);
    public Color borderGlow = new Color(0.18f, 0.8f, 0.44f, 1f);

    [Header("=== CONFIGURACIÓN DEL SHINE ===")]
    public float shineDuration = 0.5f;
    public float shineWidth = 80f;
    public Color shineColor = new Color(0.18f, 0.8f, 0.44f, 1f);

    [Range(0f, 1f)]
    public float shineOpacity = 0.8f;

    private RectTransform buttonRect;
    private Vector2 originalPosition;
    private Vector2 targetPosition;

    private GameObject shineContainer;
    private GameObject shineObject;
    private Image shineImage;
    private RectTransform shineRect;

    private Coroutine currentAnimation;

    void Start()
    {
        buttonRect = GetComponent<RectTransform>();

        if (buttonBackground == null)
            buttonBackground = transform.Find("Background")?.GetComponent<Image>();

        if (buttonBackground == null)
            buttonBackground = GetComponent<Image>();

        if (buttonBorder == null)
            buttonBorder = transform.Find("Border")?.GetComponent<Image>();

        originalPosition = buttonRect.anchoredPosition;
        targetPosition = originalPosition;

        // Estado inicial
        if (buttonBackground != null)
            buttonBackground.color = backgroundNormal;

        if (buttonBorder != null)
            buttonBorder.color = borderNormal;

        CreateShine();
    }

    void Update()
    {
        buttonRect.anchoredPosition = Vector2.Lerp(
            buttonRect.anchoredPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    void CreateShine()
    {
        if (buttonBackground == null) return;

        // Contenedor con máscara
        shineContainer = new GameObject("ShineContainer");
        shineContainer.transform.SetParent(buttonBackground.transform, false);

        RectMask2D rectMask = shineContainer.AddComponent<RectMask2D>();

        RectTransform containerRect = shineContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Shine dentro del contenedor
        shineObject = new GameObject("Shine");
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
        targetPosition = originalPosition + new Vector2(moveDistance, 0);

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(HoverIn());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPosition = originalPosition;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(HoverOut());
    }

    IEnumerator HoverIn()
    {
        if (shineImage == null || buttonBackground == null) yield break;

        float buttonWidth = buttonRect.rect.width;

        shineImage.enabled = true;

        float elapsed = 0f;

        while (elapsed < shineDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shineDuration;

            // Shine se mueve de izquierda a derecha (entra y sale)
            float shineX = Mathf.Lerp(-shineWidth / 2, buttonWidth + shineWidth / 2, t);
            shineRect.anchoredPosition = new Vector2(shineX, 0);

            // Fondo se transparenta
            buttonBackground.color = Color.Lerp(backgroundNormal, backgroundHover, t);

            // Borde se ilumina
            if (buttonBorder != null)
                buttonBorder.color = Color.Lerp(borderNormal, borderGlow, t);

            yield return null;
        }

        buttonBackground.color = backgroundHover;
        if (buttonBorder != null)
            buttonBorder.color = borderGlow;

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

            // Shine se mueve de derecha a izquierda (inverso)
            float shineX = Mathf.Lerp(buttonWidth + shineWidth / 2, -shineWidth / 2, t);
            shineRect.anchoredPosition = new Vector2(shineX, 0);

            // Fondo vuelve a negro
            buttonBackground.color = Color.Lerp(backgroundHover, backgroundNormal, t);

            // Borde se apaga
            if (buttonBorder != null)
                buttonBorder.color = Color.Lerp(borderGlow, borderNormal, t);

            yield return null;
        }

        buttonBackground.color = backgroundNormal;
        if (buttonBorder != null)
            buttonBorder.color = borderNormal;

        shineImage.enabled = false;
        currentAnimation = null;
    }
}