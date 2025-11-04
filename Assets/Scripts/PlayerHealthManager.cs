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

    // 🩸 NUEVO: referencias para la UI del panel de leveleo
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

        // 🩸 NUEVO: también configuramos la barra del panel de leveleo
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
        // 🔹 Actualiza el HUD normal
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;

        // 🔹 NUEVO: actualiza también la UI del panel de leveleo
        if (healthBarLevelPanel != null)
            healthBarLevelPanel.value = currentHealth;

        if (healthTextLevelPanel != null)
            healthTextLevelPanel.text = currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        Debug.Log("Jugador murió. Intentando llamar a GameManager...");
        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();
        else
            Debug.LogError("GameManager.instance es NULL");
    }
}
