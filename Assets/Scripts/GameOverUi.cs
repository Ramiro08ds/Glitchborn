using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverUi : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup canvasGroup;  // Para efecto de fade in
    public float fadeSpeed = 1f;

    [Header("=== ESTADÍSTICAS ===")]
    [Tooltip("Texto que muestra el nivel alcanzado")]
    public TextMeshProUGUI txtNivelAlcanzado;

    [Tooltip("Texto que muestra enemigos derrotados")]
    public TextMeshProUGUI txtEnemigosEliminados;

    [Tooltip("Texto opcional para tiempo jugado")]
    public TextMeshProUGUI txtTiempoJugado;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reproducir música de Game Over
        if (AudioManager.instance != null)
            AudioManager.instance.ReproducirMusicaGameOver();

        // NUEVO: Mostrar estadísticas
        MostrarEstadisticas();

        // Fade in effect
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }

        Debug.Log("GameOver UI iniciado, cursor liberado");
    }

    /// <summary>
    /// Muestra las estadísticas de la partida
    /// </summary>
    void MostrarEstadisticas()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager no encontrado, no se pueden mostrar estadísticas");
            return;
        }

        // Nivel alcanzado
        if (txtNivelAlcanzado != null)
        {
            txtNivelAlcanzado.text = "Nivel alcanzado: " + GameManager.instance.nivelAlcanzado;
        }

        // Enemigos eliminados
        if (txtEnemigosEliminados != null)
        {
            txtEnemigosEliminados.text = "Enemigos derrotados: " + GameManager.instance.enemigosEliminados;
        }

        // Tiempo jugado (opcional)
        if (txtTiempoJugado != null)
        {
            int minutos = Mathf.FloorToInt(GameManager.instance.tiempoJugado / 60f);
            int segundos = Mathf.FloorToInt(GameManager.instance.tiempoJugado % 60f);
            txtTiempoJugado.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);
        }

        Debug.Log($"Estadísticas mostradas - Nivel: {GameManager.instance.nivelAlcanzado}, Enemigos: {GameManager.instance.enemigosEliminados}");
    }

    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public void Retry()
    {
        // Sonido de click
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoBotonClick();

        // Vuelve a cargar la escena del juego
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitToMenu()
    {
        // Sonido de click
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoBotonClick();

        // Vuelve al menú principal
        SceneManager.LoadScene("MainMenu");
    }
}