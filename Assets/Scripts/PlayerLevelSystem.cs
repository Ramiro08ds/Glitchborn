using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Player Stats")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public int skillPoints = 0;
    public int strength = 1;
    public int maxHealth = 100;

    public static PlayerLevelSystem Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        skillPoints += 2;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
        Debug.Log("¡Subiste de nivel! Nivel: " + currentLevel + ", SkillPoints: " + skillPoints);
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
        
        GainXP(xpReward);

        
        if (PlayerHealthManager.instance != null)
        {
            PlayerHealthManager.instance.CurrentHealth += healAmount;
            Debug.Log("Jugador curado en " + healAmount + " de vida al matar enemigo.");
        }
    }
}
