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
    public TextMeshProUGUI txtHealthIndicator;  // "100 / 100"

    [Header("XP Bar")]
    public Slider xpBar;
    public TextMeshProUGUI txtXPIndicator;  // "0 / 1"

    [Header("Stats - Ahora muestra NIVELES")]
    public TextMeshProUGUI txtStrength;     // Muestra "STR: 1"
    public TextMeshProUGUI txtMaxHealth;    // Muestra "VIT: 1" (nivel, no valor)
    public TextMeshProUGUI txtSkillPoints;

    [Header("Buttons")]
    public Button btnAddStrength;
    public Button btnAddMaxHealth;

    [Header("References")]
    public PlayerLevelSystem playerLevel;
    public PlayerHealthManager playerHealthManager;

    [Header("Stat Configuration")]
    public int healthIncreasePerPoint = 5;
    public int baseHealth = 100;  // Vida inicial
    public int baseVitLevel = 1;  // VIT empieza en nivel 1

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

        // Sonido de abrir menú
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

        // Sonido de cerrar menú
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoCerrarMenu();
    }

    void AddStrength()
    {
        if (playerLevel != null)
        {
            // Verificar si tiene puntos disponibles
            if (playerLevel.skillPoints > 0)
            {
                // Sonido de éxito
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoBotonClick();

                playerLevel.UpgradeStrength();
                UpdateUI();
            }
            else
            {
                // Sonido de error/no disponible
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoNoPuntos();
            }
        }
    }

    void AddMaxHealth()
    {
        if (playerLevel != null && playerHealthManager != null)
        {
            // Verificar si tiene puntos disponibles
            if (playerLevel.skillPoints > 0)
            {
                // Sonido de éxito
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoBotonClick();

                // Primero guardamos la vida actual ANTES de modificar
                int currentHealthBeforeUpgrade = playerHealthManager.CurrentHealth;

                // Subir el stat en el sistema de leveleo
                playerLevel.UpgradeMaxHealth();

                // Aumentar MaxHealth y CurrentHealth correctamente
                playerHealthManager.MaxHealth += healthIncreasePerPoint;
                playerHealthManager.CurrentHealth = currentHealthBeforeUpgrade + healthIncreasePerPoint;

                // Asegurarse de que no exceda el máximo
                if (playerHealthManager.CurrentHealth > playerHealthManager.MaxHealth)
                    playerHealthManager.CurrentHealth = playerHealthManager.MaxHealth;

                UpdateUI();
            }
            else
            {
                // Sonido de error/no disponible
                if (AudioManager.instance != null)
                    AudioManager.instance.SonidoNoPuntos();
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

        // Nivel - solo el número
        if (txtLevel != null)
            txtLevel.text = playerLevel.currentLevel.ToString();

        // BARRA DE VIDA - usa los valores REALES del PlayerHealthManager
        if (healthBar != null)
        {
            healthBar.maxValue = playerHealthManager.MaxHealth;
            healthBar.value = playerHealthManager.CurrentHealth;
        }

        // Indicador numérico de vida - usa los valores REALES
        if (txtHealthIndicator != null)
            txtHealthIndicator.text = $"{playerHealthManager.CurrentHealth} / {playerHealthManager.MaxHealth}";

        // BARRA DE XP
        if (xpBar != null)
        {
            xpBar.maxValue = playerLevel.xpToNextLevel;
            xpBar.value = playerLevel.currentXP;
        }

        // Indicador numérico de XP
        if (txtXPIndicator != null)
            txtXPIndicator.text = $"{playerLevel.currentXP} / {playerLevel.xpToNextLevel}";

        // STR
        if (txtStrength != null)
            txtStrength.text = "STR: " + playerLevel.strength;

        // VIT - Empieza en nivel 1 en lugar de 0
        if (txtMaxHealth != null)
        {
            // Calcula cuántos puntos de VIT se invirtieron y suma el nivel base
            int vitPointsInvested = (playerHealthManager.MaxHealth - baseHealth) / healthIncreasePerPoint;
            int vitLevel = baseVitLevel + vitPointsInvested;
            txtMaxHealth.text = "VIT: " + vitLevel;
        }

        if (txtSkillPoints != null)
            txtSkillPoints.text = "Puntos: " + playerLevel.skillPoints;
    }
}