using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handler para los botones del menú principal
/// Conecta los botones con sus funciones
/// </summary>
public class MainMenuButtons : MonoBehaviour
{
    [Header("=== REFERENCIAS ===")]
    [Tooltip("Referencia al menú de opciones")]
    public OptionsMenu optionsMenu;
    
    [Header("=== BOTONES ===")]
    public Button btnJugar;
    public Button btnOpciones;
    public Button btnCreditos;
    public Button btnSalir;
    
    void Start()
    {
        // Configurar listeners
        if (btnJugar != null)
            btnJugar.onClick.AddListener(OnClickJugar);
        
        if (btnOpciones != null)
            btnOpciones.onClick.AddListener(OnClickOpciones);
        
        if (btnCreditos != null)
            btnCreditos.onClick.AddListener(OnClickCreditos);
        
        if (btnSalir != null)
            btnSalir.onClick.AddListener(OnClickSalir);
    }
    
    void OnClickJugar()
    {
        Debug.Log("Jugar clickeado");
        // Cargar escena del juego
        // SceneManager.LoadScene("GameScene");
    }
    
    void OnClickOpciones()
    {
        Debug.Log("Opciones clickeado");
        if (optionsMenu != null)
        {
            optionsMenu.OpenOptions();
        }
    }
    
    void OnClickCreditos()
    {
        Debug.Log("Créditos clickeado");
        // Abrir menú de créditos
    }
    
    void OnClickSalir()
    {
        Debug.Log("Salir clickeado");
        // Salir del juego
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
