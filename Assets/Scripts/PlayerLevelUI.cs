using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelUI : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtXP;
    public Image xpBar; // 👈 ahora es Image

    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtMaxHealth;
    public TextMeshProUGUI txtSkillPoints;

    public Button btnAddStrength;
    public Button btnAddMaxHealth;

    public PlayerLevelSystem playerLevel;
    public PlayerHealthManager playerHealthManager;

    public int healthIncreaseAmount = 10;

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
            panel.SetActive(!panel.activeSelf);
            UpdateUI();
        }
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
