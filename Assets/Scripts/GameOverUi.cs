using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUi : MonoBehaviour
{
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
