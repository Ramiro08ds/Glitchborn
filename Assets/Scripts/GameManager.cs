using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("=== ESTADÍSTICAS DE LA PARTIDA ===")]
    public int enemigosEliminados = 0;
    public int nivelAlcanzado = 1;

    // Stats adicionales opcionales
    public float tiempoJugado = 0f;
    public int dañoTotalRecibido = 0;
    public int dañoTotalCausado = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // --- Al iniciar el juego, ir al MainMenu ---
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        // Trackear tiempo jugado (solo si no estamos en menú o game over)
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu" && currentScene != "GameOver")
        {
            tiempoJugado += Time.deltaTime;
        }
    }

    /// <summary>
    /// Llamar cuando el jugador mata un enemigo
    /// </summary>
    public void RegistrarEnemigoEliminado()
    {
        enemigosEliminados++;
        Debug.Log($"Enemigos eliminados: {enemigosEliminados}");
    }

    /// <summary>
    /// Actualizar el nivel actual del jugador
    /// </summary>
    public void ActualizarNivel(int nivel)
    {
        if (nivel > nivelAlcanzado)
        {
            nivelAlcanzado = nivel;
            Debug.Log($"Nivel alcanzado: {nivelAlcanzado}");
        }
    }

    /// <summary>
    /// Resetear estadísticas al empezar nueva partida
    /// </summary>
    public void ResetearEstadisticas()
    {
        enemigosEliminados = 0;
        nivelAlcanzado = 1;
        tiempoJugado = 0f;
        dañoTotalRecibido = 0;
        dañoTotalCausado = 0;
        Debug.Log("Estadísticas reseteadas");
    }

    public void LoadGame()
    {
        ResetearEstadisticas(); // Resetear al empezar nueva partida
        SceneManager.LoadScene("Tutorial");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    public void PlayerDied()
    {
        Debug.Log($"Has Muerto - Nivel: {nivelAlcanzado}, Enemigos: {enemigosEliminados}");
        SceneManager.LoadScene("GameOver");
    }
}