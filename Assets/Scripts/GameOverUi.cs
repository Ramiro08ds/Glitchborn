using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUi : MonoBehaviour
{
 
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("GameOver UI iniciado, cursor liberado");
    }

public void Retry()
    {
        // vuelve a cargar la escena del juego
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitToMenu()
    {
        // vuelve al menú principal
        SceneManager.LoadScene("MainMenu");
    }
}
