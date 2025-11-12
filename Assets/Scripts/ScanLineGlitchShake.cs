using UnityEngine;
using System.Collections;

/// <summary>
/// Efecto de shake/glitch cuando pasa la línea de escaneo
/// </summary>
public class ScanLineGlitchShake : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("La línea de escaneo")]
    public RectTransform scanLine;
    
    [Tooltip("La imagen a sacudir (tu background con título)")]
    public RectTransform targetImage;
    
    [Header("Posición del Título")]
    [Tooltip("Posición Y donde está el título")]
    public float titlePositionY = 250f;
    
    [Header("Glitch Settings")]
    [Tooltip("Distancia para activar el glitch")]
    public float triggerDistance = 50f;
    
    [Tooltip("Intensidad del shake")]
    public float shakeIntensity = 5f;
    
    [Tooltip("Duración del shake (segundos)")]
    public float shakeDuration = 0.1f;
    
    private Vector2 originalPosition;
    private bool isShaking = false;
    private bool hasTriggered = false;
    private float lastScanY = 0f;
    
    void Start()
    {
        // Auto-buscar referencias
        if (scanLine == null)
        {
            GameObject scanObj = GameObject.Find("ScanLine");
            if (scanObj != null)
                scanLine = scanObj.GetComponent<RectTransform>();
        }
        
        if (targetImage == null)
        {
            targetImage = GetComponent<RectTransform>();
        }
        
        if (targetImage != null)
        {
            originalPosition = targetImage.anchoredPosition;
        }
    }
    
    void Update()
    {
        if (scanLine == null || targetImage == null) return;
        
        // Obtener posición Y de la línea
        Vector3[] corners = new Vector3[4];
        scanLine.GetWorldCorners(corners);
        
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, 
            RectTransformUtility.WorldToScreenPoint(null, corners[0]), 
            null, 
            out localPoint
        );
        
        float scanY = localPoint.y;
        
        // Calcular distancia
        float distance = Mathf.Abs(scanY - titlePositionY);
        
        // Trigger glitch cuando pasa JUSTO por el título
        if (distance < triggerDistance && !hasTriggered && !isShaking)
        {
            // Solo trigger si la línea está bajando (pasando por el título)
            if (scanY < lastScanY)
            {
                StartCoroutine(DoGlitchShake());
                hasTriggered = true;
            }
        }
        
        // Reset trigger cuando se aleja
        if (distance > triggerDistance * 2)
        {
            hasTriggered = false;
        }
        
        lastScanY = scanY;
    }
    
    IEnumerator DoGlitchShake()
    {
        isShaking = true;
        float elapsed = 0f;
        
        // Shake rápido durante la duración especificada
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            targetImage.anchoredPosition = originalPosition + new Vector2(x, y);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Restaurar posición original
        targetImage.anchoredPosition = originalPosition;
        isShaking = false;
    }
}
