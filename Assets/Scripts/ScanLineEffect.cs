using UnityEngine;

/// <summary>
/// ESTE SCRIPT MUEVE LA LÍNEA DE ESCANEO DE ARRIBA HACIA ABAJO
/// Adjuntar al objeto "ScanLine"
/// </summary>
public class ScanLineEffect : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Velocidad (más alto = más rápido)")]
    public float speed = 200f;

    [Header("Límites de pantalla")]
    [Tooltip("Posición superior (donde empieza) - justo en el borde superior visible")]
    public float topPosition = 0f;

    [Tooltip("Posición inferior (donde desaparece) - justo en el borde inferior")]
    public float bottomPosition = -1080f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Empezar arriba de la pantalla (borde superior visible)
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = topPosition;
        rectTransform.anchoredPosition = pos;
    }

    void Update()
    {
        // Obtener posición actual
        Vector2 pos = rectTransform.anchoredPosition;

        // Mover hacia abajo
        pos.y -= speed * Time.deltaTime;

        // Cuando llega abajo, volver arriba (loop)
        if (pos.y < bottomPosition)
        {
            pos.y = topPosition; // Volver arriba
        }

        // Aplicar movimiento
        rectTransform.anchoredPosition = pos;
    }
}