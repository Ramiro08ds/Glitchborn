using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menú de opciones con sliders de volumen
/// Se abre/cierra con animación suave
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    [Header("=== REFERENCIAS ===")]
    [Tooltip("Panel del menú de opciones")]
    public GameObject optionsPanel;
    
    [Tooltip("Overlay oscuro de fondo")]
    public Image backgroundOverlay;
    
    [Header("=== SLIDERS ===")]
    [Tooltip("Slider de volumen general")]
    public Slider masterVolumeSlider;
    
    [Tooltip("Slider de música")]
    public Slider musicVolumeSlider;
    
    [Tooltip("Slider de efectos de sonido")]
    public Slider sfxVolumeSlider;
    
    [Header("=== TEXTOS DE VALORES ===")]
    [Tooltip("Texto que muestra el valor del volumen general")]
    public TextMeshProUGUI masterValueText;
    
    [Tooltip("Texto que muestra el valor de la música")]
    public TextMeshProUGUI musicValueText;
    
    [Tooltip("Texto que muestra el valor de los SFX")]
    public TextMeshProUGUI sfxValueText;
    
    [Header("=== ANIMACIÓN ===")]
    [Tooltip("Duración de la animación de apertura/cierre")]
    public float animationDuration = 0.3f;
    
    [Tooltip("Escala inicial del panel (pequeño)")]
    public float startScale = 0.8f;
    
    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private bool isOpen = false;
    
    void Start()
    {
        // Obtener componentes
        if (optionsPanel != null)
        {
            canvasGroup = optionsPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = optionsPanel.AddComponent<CanvasGroup>();
            
            panelRect = optionsPanel.GetComponent<RectTransform>();
        }
        
        // Cargar valores guardados
        LoadVolumes();
        
        // Configurar listeners de sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        // Cerrar al inicio
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
    
    void LoadVolumes()
    {
        // Cargar valores guardados o usar defaults
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        // Aplicar a sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
        
        // Actualizar textos
        UpdateVolumeTexts();
    }
    
    void UpdateVolumeTexts()
    {
        if (masterValueText != null && masterVolumeSlider != null)
            masterValueText.text = Mathf.RoundToInt(masterVolumeSlider.value * 100) + "%";
        
        if (musicValueText != null && musicVolumeSlider != null)
            musicValueText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100) + "%";
        
        if (sfxValueText != null && sfxVolumeSlider != null)
            sfxValueText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100) + "%";
    }
    
    void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        UpdateVolumeTexts();
        
        // Aplicar volumen al AudioManager si existe
        if (AudioManager.instance != null)
        {
            // AudioManager.instance.SetMasterVolume(value);
        }
    }
    
    void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        UpdateVolumeTexts();
        
        // Aplicar volumen al AudioManager si existe
        if (AudioManager.instance != null)
        {
            // AudioManager.instance.SetMusicVolume(value);
        }
    }
    
    void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        UpdateVolumeTexts();
        
        // Aplicar volumen al AudioManager si existe
        if (AudioManager.instance != null)
        {
            // AudioManager.instance.SetSFXVolume(value);
        }
    }
    
    public void OpenOptions()
    {
        if (isOpen) return;
        
        isOpen = true;
        
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            StartCoroutine(AnimateOpen());
        }
    }
    
    public void CloseOptions()
    {
        if (!isOpen) return;
        
        isOpen = false;
        StartCoroutine(AnimateClose());
    }
    
    System.Collections.IEnumerator AnimateOpen()
    {
        float elapsed = 0f;
        
        // Estado inicial
        canvasGroup.alpha = 0f;
        panelRect.localScale = Vector3.one * startScale;
        
        if (backgroundOverlay != null)
        {
            Color overlayColor = backgroundOverlay.color;
            overlayColor.a = 0f;
            backgroundOverlay.color = overlayColor;
        }
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Ease out
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            
            // Fade in
            canvasGroup.alpha = eased;
            
            // Scale up
            float scale = Mathf.Lerp(startScale, 1f, eased);
            panelRect.localScale = Vector3.one * scale;
            
            // Fade in overlay
            if (backgroundOverlay != null)
            {
                Color overlayColor = backgroundOverlay.color;
                overlayColor.a = 0.8f * eased;
                backgroundOverlay.color = overlayColor;
            }
            
            yield return null;
        }
        
        // Estado final
        canvasGroup.alpha = 1f;
        panelRect.localScale = Vector3.one;
        
        if (backgroundOverlay != null)
        {
            Color overlayColor = backgroundOverlay.color;
            overlayColor.a = 0.8f;
            backgroundOverlay.color = overlayColor;
        }
    }
    
    System.Collections.IEnumerator AnimateClose()
    {
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Ease in
            float eased = Mathf.Pow(t, 3f);
            
            // Fade out
            canvasGroup.alpha = 1f - eased;
            
            // Scale down
            float scale = Mathf.Lerp(1f, startScale, eased);
            panelRect.localScale = Vector3.one * scale;
            
            // Fade out overlay
            if (backgroundOverlay != null)
            {
                Color overlayColor = backgroundOverlay.color;
                overlayColor.a = 0.8f * (1f - eased);
                backgroundOverlay.color = overlayColor;
            }
            
            yield return null;
        }
        
        // Desactivar
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
    
    void Update()
    {
        // Cerrar con ESC
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
        }
    }
}
