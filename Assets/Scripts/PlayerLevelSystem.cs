using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Player Stats")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int skillPoints = 0;
    public int strength = 1;
    public int maxHealth = 100;

    [Header("UI (HUD)")]
    public Slider xpBarHUD;
    public TMP_Text xpTextHUD;

    public static PlayerLevelSystem Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("=== PlayerLevelSystem Start ===");

        // Buscar automaticamente la barra de XP del HUD si no esta asignada
        if (xpBarHUD == null)
        {
            Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
            foreach (Slider slider in allSliders)
            {
                if (slider.gameObject.scene.isLoaded && slider.gameObject.name == "XpBar_HUD")
                {
                    xpBarHUD = slider;
                    Debug.Log("XpBar_HUD encontrada automaticamente!");
                    break;
                }
            }

            if (xpBarHUD == null)
            {
                Debug.LogError("NO se encontro XpBar_HUD!");
            }
        }

        // Buscar automaticamente el texto de XP del HUD si no esta asignado
        if (xpTextHUD == null)
        {
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (TMP_Text text in allTexts)
            {
                if (text.gameObject.scene.isLoaded && text.gameObject.name == "XpText_HUD")
                {
                    xpTextHUD = text;
                    Debug.Log("XpText_HUD encontrado automaticamente!");
                    break;
                }
            }
        }

        UpdateXPUI();
    }

    public void GainXP(int amount)
    {
        Debug.Log("=== GainXP llamado! ===");
        Debug.Log("Amount recibido: " + amount);
        Debug.Log("XP antes: " + currentXP);

        currentXP += amount;

        Debug.Log("XP despues: " + currentXP);
        Debug.Log("XP necesario para level up: " + xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    void LevelUp()
    {
        currentLevel++;
        skillPoints += 2;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
        Debug.Log("¡Subiste de nivel! Nivel: " + currentLevel + ", SkillPoints: " + skillPoints);
        UpdateXPUI();
    }

    public void UpgradeStrength()
    {
        if (skillPoints > 0)
        {
            strength++;
            skillPoints--;
        }
    }

    public void UpgradeMaxHealth()
    {
        if (skillPoints > 0)
        {
            maxHealth++;
            skillPoints--;
        }
    }

    public void EnemyKilled(int xpReward, int healAmount)
    {
        Debug.Log("=== EnemyKilled llamado! ===");
        Debug.Log("xpReward: " + xpReward);
        Debug.Log("healAmount: " + healAmount);

        GainXP(xpReward);

        if (PlayerHealthManager.instance != null)
        {
            PlayerHealthManager.instance.CurrentHealth += healAmount;
            Debug.Log("Jugador curado en " + healAmount + " de vida al matar enemigo.");
        }
    }

    void UpdateXPUI()
    {
        Debug.Log("=== UpdateXPUI llamado ===");
        Debug.Log("xpBarHUD es null? " + (xpBarHUD == null));
        Debug.Log("currentXP: " + currentXP + ", xpToNextLevel: " + xpToNextLevel);

        // Actualizar barra de XP del HUD
        if (xpBarHUD != null)
        {
            xpBarHUD.maxValue = xpToNextLevel;
            xpBarHUD.value = currentXP;
            Debug.Log("Barra actualizada: value=" + xpBarHUD.value + ", max=" + xpBarHUD.maxValue);
        }
        else
        {
            Debug.LogError("xpBarHUD es NULL! No se puede actualizar la barra.");
        }

        // Actualizar texto de XP del HUD
        if (xpTextHUD != null)
        {
            xpTextHUD.text = currentXP + " / " + xpToNextLevel;
            Debug.Log("Texto XP actualizado: " + xpTextHUD.text);
        }
    }
}