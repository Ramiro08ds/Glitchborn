using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelUI : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtXP;
    public Image xpBar;

    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtMaxHealth;
    public TextMeshProUGUI txtSkillPoints;

    public Button btnAddStrength;
    public Button btnAddMaxHealth;

    public PlayerLevelSystem playerLevel;
    public PlayerHealthManager playerHealthManager;

    public int healthIncreaseAmount = 10;

    // ⚡ Para bloquear acciones del jugador:
    public bool menuAbierto = false; 

    void Start()
    {
        panel.SetActive(false);
        btnAddStrength.onClick.AddListener(AddStrength);
        btnAddMaxHealth.onClick.AddListener(AddMaxHealth);
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Alterna menú
            if (menuAbierto)
                CerrarMenu();
            else
                AbrirMenu();
        }
    }

    void AbrirMenu()
    {
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
        playerLevel.UpgradeStrength();
        UpdateUI();
    }

    void AddMaxHealth()
    {
        if (playerLevel.skillPoints > 0)
        {
            playerLevel.UpgradeMaxHealth(healthIncreaseAmount);
            if (playerHealthManager != null)
            {
                playerHealthManager.maxHealth = playerLevel.maxHealth;
                playerHealthManager.Heal(playerHealthManager.maxHealth);
            }
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        txtLevel.text = "Nivel: " + playerLevel.currentLevel;
        txtXP.text = $"XP: {playerLevel.currentXP} / {playerLevel.xpToNextLevel}";

        if (xpBar != null)
            xpBar.fillAmount = (float)playerLevel.currentXP / playerLevel.xpToNextLevel;

        txtStrength.text = "Fuerza: " + playerLevel.strength;
        txtMaxHealth.text = "Vida Máx: " + playerLevel.maxHealth;
        txtSkillPoints.text = "Puntos: " + playerLevel.skillPoints;
    }
}
