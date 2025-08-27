using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Tutorial"); // <-- corregido
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    public void PlayerDied()
    {
        Debug.Log("Has Muerto");
        SceneManager.LoadScene("GameOver");
    }
}
