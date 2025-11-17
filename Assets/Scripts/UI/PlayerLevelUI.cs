using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Level Display")]
    public TextMeshProUGUI txtLevel;

    [Header("Health Bar")]
    public Slider healthBar;
    public TextMeshProUGUI txtHealthIndicator;

    [Header("XP Bar")]
    public Slider xpBar;
    public TextMeshProUGUI txtXPIndicator;

    [Header("Stats - Ahora muestra NIVELES")]
    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtMaxHealth;
    public TextMeshProUGUI txtSkillPoints;

    [Header("Buttons")]
    public Button btnAddStrength;
    public Button btnAddMaxHealth;

    [Header("Button Shake Components")]
    public ButtonShake shakeStrength;
    public ButtonShake shakeMaxHealth;

    [Header("References")]
    public PlayerLevelSystem playerLevel;
    public PlayerHealthManager playerHealthManager;

    [Header("Stat Configuration")]
    public int healthIncreasePerPoint = 5;
    public int baseHealth = 100;
    public int baseVitLevel = 1;

    public bool menuAbierto = false;

    void Start()
    {
        if (panel == null)
        {
            Debug.LogError("PlayerLevelUI: Panel no está asignado!");
            return;
        }

        panel.SetActive(false);

        if (btnAddStrength != null)
            btnAddStrength.onClick.AddListener(AddStrength);
        if (btnAddMaxHealth != null)
            btnAddMaxHealth.onClick.AddListener(AddMaxHealth);

        if (shakeStrength == null && btnAddStrength != null)
            shakeStrength = btnAddStrength.GetComponent<ButtonShake>();
        if (shakeMaxHealth == null && btnAddMaxHealth != null)
            shakeMaxHealth = btnAddMaxHealth.GetComponent<ButtonShake>();

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuAbierto) CerrarMenu();
            else AbrirMenu();
        }
    }

    void AbrirMenu()
    {
        if (panel == null)
        {
            Debug.LogError("No se puede abrir menu: Panel es NULL!");
            return;
        }

        panel.SetActive(true);
        UpdateUI();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        menuAbierto = true;

        if (AudioManager.instance != null)
            AudioManager.instance.SonidoAbrirMenu();
    }

    void CerrarMenu()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menuAbierto = false;

        if (AudioManager.instance != null)
            AudioManager.instance.SonidoCerrarMenu();
    }

    void AddStrength()
    {
        if (playerLevel != null)
        {
            if (playerLevel.skillPoints > 0)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoBotonClick();

                playerLevel.UpgradeStrength();
                UpdateUI();
            }
            else
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoNoPuntos();

                if (shakeStrength != null)
                    shakeStrength.Shake();
            }
        }
    }

    void AddMaxHealth()
    {
        if (playerLevel != null && playerHealthManager != null)
        {
            if (playerLevel.skillPoints > 0)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoBotonClick();

                int currentHealthBeforeUpgrade = playerHealthManager.CurrentHealth;
                playerLevel.UpgradeMaxHealth();
                playerHealthManager.MaxHealth += healthIncreasePerPoint;
                playerHealthManager.CurrentHealth = currentHealthBeforeUpgrade + healthIncreasePerPoint;

                if (playerHealthManager.CurrentHealth > playerHealthManager.MaxHealth)
                    playerHealthManager.CurrentHealth = playerHealthManager.MaxHealth;

                UpdateUI();
            }
            else
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoNoPuntos();

                if (shakeMaxHealth != null)
                    shakeMaxHealth.Shake();
            }
        }
    }

    void UpdateUI()
    {
        if (playerLevel == null)
        {
            Debug.LogError("PlayerLevelSystem no está asignado!");
            return;
        }

        if (playerHealthManager == null)
        {
            Debug.LogError("PlayerHealthManager no está asignado!");
            return;
        }

        if (txtLevel != null)
            txtLevel.text = playerLevel.currentLevel.ToString();

        if (healthBar != null)
        {
            healthBar.maxValue = playerHealthManager.MaxHealth;
            healthBar.value = playerHealthManager.CurrentHealth;
        }

        if (txtHealthIndicator != null)
            txtHealthIndicator.text = $"{playerHealthManager.CurrentHealth} / {playerHealthManager.MaxHealth}";

        if (xpBar != null)
        {
            xpBar.maxValue = playerLevel.xpToNextLevel;
            xpBar.value = playerLevel.currentXP;
        }

        if (txtXPIndicator != null)
            txtXPIndicator.text = $"{playerLevel.currentXP} / {playerLevel.xpToNextLevel}";

        // MODIFICADO: Solo el número de fuerza
        if (txtStrength != null)
            txtStrength.text = playerLevel.strength.ToString();

        // MODIFICADO: Solo el número de vitalidad
        if (txtMaxHealth != null)
        {
            int vitPointsInvested = (playerHealthManager.MaxHealth - baseHealth) / healthIncreasePerPoint;
            int vitLevel = baseVitLevel + vitPointsInvested;
            txtMaxHealth.text = vitLevel.ToString();
        }

        if (txtSkillPoints != null)
            txtSkillPoints.text = "Puntos: " + playerLevel.skillPoints;
    }
}