using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelUI : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI txtLevel;
    // txtXP eliminado - ya no se usa
    public Slider xpBar;  // Cambiado de Image a Slider

    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtMaxHealth;
    public TextMeshProUGUI txtSkillPoints;

    public Button btnAddStrength;
    public Button btnAddMaxHealth;

    public PlayerLevelSystem playerLevel;
    public PlayerHealthManager playerHealthManager;

    public int healthIncreasePerPoint = 3;

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
    }

    void CerrarMenu()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menuAbierto = false;
    }

    void AddStrength()
    {
        if (playerLevel != null)
        {
            playerLevel.UpgradeStrength();
            UpdateUI();
        }
    }

    void AddMaxHealth()
    {
        if (playerLevel != null && playerLevel.skillPoints > 0 && playerHealthManager != null)
        {
            playerLevel.UpgradeMaxHealth();

            // aumenta maxHealth y currentHealth proporcionalmente
            playerHealthManager.MaxHealth += healthIncreasePerPoint;
            playerHealthManager.CurrentHealth += healthIncreasePerPoint;

            UpdateUI();
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
            txtLevel.text = "Nivel: " + playerLevel.currentLevel;

        // txtXP eliminado - ya no se usa

        // Cambiado de fillAmount a value para Slider
        if (xpBar != null)
        {
            xpBar.maxValue = playerLevel.xpToNextLevel;
            xpBar.value = playerLevel.currentXP;
        }

        if (txtStrength != null)
            txtStrength.text = "Fuerza: " + playerLevel.strength;
        if (txtMaxHealth != null)
            txtMaxHealth.text = "Vida Máx: " + playerHealthManager.MaxHealth;
        if (txtSkillPoints != null)
            txtSkillPoints.text = "Puntos: " + playerLevel.skillPoints;
    }
}