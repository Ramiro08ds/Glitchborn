using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Player Stats")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 1;
    public int skillPoints = 0;
    public int strength = 1;
    public int maxHealth = 100;

    [Header("UI (HUD)")]
    public Slider xpBarHUD;
    public TMP_Text xpTextHUD;
    public TMP_Text levelUpText;

    [Header("Animation Settings")]
    public float xpFillSpeed = 1f; // Cambiado de 2 a 1 (mitad de velocidad)

    public static PlayerLevelSystem Instance;

    private float targetXP = 0f;
    private float displayedXP = 0f;
    private bool isAnimating = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (xpBarHUD == null)
        {
            Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
            foreach (Slider slider in allSliders)
            {
                if (slider.gameObject.scene.isLoaded && slider.gameObject.name == "XpBar_HUD")
                {
                    xpBarHUD = slider;
                    break;
                }
            }
        }

        if (xpTextHUD == null)
        {
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (TMP_Text text in allTexts)
            {
                if (text.gameObject.scene.isLoaded && text.gameObject.name == "XpText_HUD")
                {
                    xpTextHUD = text;
                    break;
                }
            }
        }

        if (levelUpText == null)
        {
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (TMP_Text text in allTexts)
            {
                if (text.gameObject.scene.isLoaded && text.gameObject.name == "LevelUpText")
                {
                    levelUpText = text;
                    if (levelUpText != null)
                        levelUpText.gameObject.SetActive(false);
                    break;
                }
            }
        }

        targetXP = currentXP;
        displayedXP = currentXP;
        UpdateXPUI();
    }

    void Update()
    {
        if (isAnimating)
        {
            displayedXP = Mathf.MoveTowards(displayedXP, targetXP, xpFillSpeed * Time.deltaTime * xpToNextLevel);

            if (xpBarHUD != null)
                xpBarHUD.value = displayedXP;

            if (xpTextHUD != null)
                xpTextHUD.text = Mathf.FloorToInt(displayedXP) + " / " + xpToNextLevel;

            if (Mathf.Approximately(displayedXP, targetXP))
            {
                isAnimating = false;
            }
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        targetXP = currentXP;

        if (currentXP >= xpToNextLevel)
        {
            StartCoroutine(AnimateLevelUp());
        }
        else
        {
            isAnimating = true;
        }
    }

    System.Collections.IEnumerator AnimateLevelUp()
    {
        targetXP = xpToNextLevel;
        isAnimating = true;

        yield return new WaitWhile(() => isAnimating);

        int xpOverflow = currentXP - xpToNextLevel;
        currentXP = 0;
        displayedXP = 0;
        targetXP = 0;

        LevelUp();

        if (xpBarHUD != null)
            xpBarHUD.value = 0;
        if (xpTextHUD != null)
            xpTextHUD.text = "0 / " + xpToNextLevel;

        ShowLevelUpText();

        if (xpOverflow > 0)
        {
            yield return new WaitForSeconds(0.5f);
            currentXP = xpOverflow;
            targetXP = xpOverflow;
            displayedXP = 0;
            isAnimating = true;
        }
    }

    void LevelUp()
    {
        currentLevel++;
        skillPoints += 2;
        xpToNextLevel = currentLevel;

        Debug.Log("¡Subiste de nivel! Nivel: " + currentLevel);
        UpdateXPUI();
    }

    void ShowLevelUpText()
    {
        if (levelUpText != null)
        {
            levelUpText.text = "¡Subiste de nivel!\nNivel " + currentLevel;
            levelUpText.gameObject.SetActive(true);
            StartCoroutine(HideLevelUpText(2f));
        }
    }

    System.Collections.IEnumerator HideLevelUpText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (levelUpText != null)
            levelUpText.gameObject.SetActive(false);
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
        }
    }

    void UpdateXPUI()
    {
        if (xpBarHUD != null)
            xpBarHUD.maxValue = xpToNextLevel;
    }
}