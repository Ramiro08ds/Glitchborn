using UnityEngine;
using System.Collections;

public class ButtonShake : MonoBehaviour
{
    [Header("Configuración del Shake")]
    [Tooltip("Duración total del shake en segundos")]
    public float duracion = 0.5f;

    [Tooltip("Intensidad del movimiento (distancia en píxeles)")]
    public float intensidad = 10f;

    [Tooltip("Velocidad del shake (cuántas veces vibra)")]
    public float velocidad = 30f;

    private RectTransform rectTransform;
    private Vector3 posicionOriginal;
    private bool estaShakeando = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
            posicionOriginal = rectTransform.localPosition;
    }

    /// <summary>
    /// Inicia el efecto de shake
    /// </summary>
    public void Shake()
    {
        if (!estaShakeando)
            StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        estaShakeando = true;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            // Calcular offset aleatorio que disminuye con el tiempo
            float progreso = 1f - (tiempoTranscurrido / duracion);
            float offsetX = Mathf.Sin(tiempoTranscurrido * velocidad) * intensidad * progreso;
            float offsetY = Mathf.Cos(tiempoTranscurrido * velocidad * 1.5f) * intensidad * 0.5f * progreso;

            rectTransform.localPosition = posicionOriginal + new Vector3(offsetX, offsetY, 0f);

            tiempoTranscurrido += Time.unscaledDeltaTime; // unscaledDeltaTime para que funcione con Time.timeScale = 0
            yield return null;
        }

        // Volver a la posición original
        rectTransform.localPosition = posicionOriginal;
        estaShakeando = false;
    }
}