using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Anima el brillo del título cuando pasa la línea de escaneo
/// Funciona con una imagen overlay separada
/// </summary>
public class TitleGlowScanEffect : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("La línea de escaneo")]
    public RectTransform scanLine;
    
    [Tooltip("La imagen del brillo (overlay)")]
    public Image glowImage;
    
    [Header("Posición del Título")]
    [Tooltip("Posición Y donde está el título en la pantalla")]
    public float titlePositionY = 250f;
    
    [Header("Brillo Settings")]
    [Tooltip("Alpha normal del brillo (0-255)")]
    public float normalAlpha = 80f;
    
    [Tooltip("Alpha cuando pasa el escáner (0-255)")]
    public float scanAlpha = 255f;
    
    [Tooltip("Distancia de detección (píxeles)")]
    public float detectionRange = 200f;
    
    [Tooltip("Velocidad de transición")]
    public float transitionSpeed = 5f;
    
    private float currentAlpha;
    private float targetAlpha;
    
    void Start()
    {
        // Auto-buscar referencias si no están asignadas
        if (scanLine == null)
        {
            GameObject scanObj = GameObject.Find("ScanLine");
            if (scanObj != null)
                scanLine = scanObj.GetComponent<RectTransform>();
        }
        
        if (glowImage == null)
        {
            glowImage = GetComponent<Image>();
        }
        
        currentAlpha = normalAlpha;
        targetAlpha = normalAlpha;
        
        // Setear alpha inicial
        if (glowImage != null)
        {
            Color color = glowImage.color;
            color.a = normalAlpha / 255f;
            glowImage.color = color;
        }
    }
    
    void Update()
    {
        if (scanLine == null || glowImage == null) return;
        
        // Obtener posición Y de la línea de escaneo
        float scanY = scanLine.anchoredPosition.y;
        
        // Calcular distancia entre la línea y el título
        float distance = Mathf.Abs(scanY - titlePositionY);
        
        // Si está cerca, aumentar brillo
        if (distance < detectionRange)
        {
            // Interpolación: más cerca = más brillo
            float proximity = 1f - (distance / detectionRange);
            targetAlpha = Mathf.Lerp(normalAlpha, scanAlpha, proximity);
        }
        else
        {
            // Volver al brillo normal
            targetAlpha = normalAlpha;
        }
        
        // Transición suave
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * transitionSpeed);
        
        // Aplicar al Image
        Color newColor = glowImage.color;
        newColor.a = currentAlpha / 255f;
        glowImage.color = newColor;
    }
}
