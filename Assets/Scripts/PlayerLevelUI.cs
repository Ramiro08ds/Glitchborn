using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelUI : MonoBehaviour
{
    [Header("References")]
    public PlayerLevelSystem player;

    [Header("Texts")]
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtSkillPoints;
    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtMaxHealth;
    public TextMeshProUGUI txtXPNumbers; // Para mostrar "XP: 20/40" encima de la barra

    [Header("XP Bar")]
    public Image xpFill; // La imagen de relleno (XPFill)

    [Header("Buttons")]
    public Button btnAddStrength;
    public Button btnAddMaxHealth;

    void Start()
    {
        // Conectar botones
        btnAddStrength.onClick.AddListener(() => player.UpgradeStrength());
        btnAddMaxHealth.onClick.AddListener(() => player.UpgradeMaxHealth(10)); // +10 vida por punto
    }

    void Update()
    {
        if (player == null) return;

        // Textos básicos
        txtLevel.text = "Nivel: " + player.currentLevel;
        txtSkillPoints.text = "Skill Points: " + player.skillPoints;
        txtStrength.text = "Fuerza: " + player.strength;
        txtMaxHealth.text = "Vida Máx: " + player.maxHealth;

        // Barra de XP
        float xpPercent = (float)player.currentXP / player.xpToNextLevel;
        xpFill.fillAmount = xpPercent; // llena la barra

        // Números encima de la barra
        txtXPNumbers.text = $"XP: {player.currentXP}/{player.xpToNextLevel}";
    }
}
