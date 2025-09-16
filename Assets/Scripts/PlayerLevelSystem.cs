using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Player Stats")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 1;

    public int skillPoints = 0;
    public int strength = 1;
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("XP Settings")]
    public int baseXPPerKill = 1; // XP base por enemigo
    public float xpScaling = 1.5f; // Multiplicador de XP a partir de nivel 11

    void Start()
    {
        CalculateXPToNextLevel();
    }

    public void GainXP(int amount)
    {
        currentXP += amount;

        // Chequear niveles múltiples en un solo golpe
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        CalculateXPToNextLevel();

        // Puntos de habilidad por nivel
        skillPoints += (currentLevel <= 10) ? 3 : 5;

        Debug.Log($"¡Subiste al nivel {currentLevel}! Puntos disponibles: {skillPoints}");
    }

    void CalculateXPToNextLevel()
    {
        if (currentLevel <= 5)
        {
            xpToNextLevel = baseXPPerKill; // 1 enemigo
        }
        else if (currentLevel <= 10)
        {
            xpToNextLevel = baseXPPerKill * 2; // 2 enemigos
        }
        else
        {
            // Escalado progresivo desde nivel 11
            int previousXP = xpToNextLevel > 0 ? xpToNextLevel : baseXPPerKill * 2;
            xpToNextLevel = Mathf.CeilToInt(previousXP * xpScaling);
        }
    }

    public void EnemyKilled(int xpReward, int healAmount)
    {
        GainXP(xpReward);

        // Recuperar vida al matar
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        Debug.Log($"Vida actual: {currentHealth}/{maxHealth} | XP: {currentXP}/{xpToNextLevel}");
    }

    public void UpgradeStrength()
    {
        if (skillPoints > 0)
        {
            strength++;
            skillPoints--;
            Debug.Log($"Fuerza aumentada a: {strength}");
        }
        else
        {
            Debug.Log("No hay puntos de habilidad disponibles.");
        }
    }

    public void UpgradeMaxHealth(int amount)
    {
        if (skillPoints > 0)
        {
            maxHealth += amount;
            skillPoints--;
            Debug.Log($"Vida máxima aumentada a: {maxHealth}");
        }
        else
        {
            Debug.Log("No hay puntos de habilidad disponibles.");
        }
    }
}
