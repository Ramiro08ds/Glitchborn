using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager instance;

    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI (HUD)")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("UI (Level Panel)")]
    public Slider healthBarLevelPanel;
    public TMP_Text healthTextLevelPanel;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Buscar automaticamente la barra del panel de leveleo si no esta asignada
        if (healthBarLevelPanel == null)
        {
            Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
            foreach (Slider slider in allSliders)
            {
                if (slider.gameObject.scene.isLoaded && slider.gameObject.name == "HealthBar_Leveleo")
                {
                    healthBarLevelPanel = slider;
                    break;
                }
            }
        }

        // Buscar automaticamente el texto del panel de leveleo si no esta asignado
        if (healthTextLevelPanel == null)
        {
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (TMP_Text text in allTexts)
            {
                if (text.gameObject.scene.isLoaded && text.gameObject.name == "HealthText_Leveleo")
                {
                    healthTextLevelPanel = text;
                    break;
                }
            }
        }

        if (healthBarLevelPanel != null)
        {
            healthBarLevelPanel.maxValue = maxHealth;
            healthBarLevelPanel.value = currentHealth;
        }

        UpdateHealthUI();
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthUI();
        }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            UpdateHealthUI();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        // NUEVO: Reproducir sonido de recibir daño
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoPlayerDamage();

        if (PlayerHitFeedback.instance != null)
            PlayerHitFeedback.instance.OnPlayerDamaged();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;

        if (healthBarLevelPanel != null)
            healthBarLevelPanel.value = currentHealth;

        if (healthTextLevelPanel != null)
            healthTextLevelPanel.text = currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoPlayerMuerte();

        Debug.Log("Jugador murio. Intentando llamar a GameManager...");
        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();
        else
            Debug.LogError("GameManager.instance es NULL");
    }
}