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

    [Header("UI (Level Panel)")]
    public Slider xpBarLevelPanel;
    public TMP_Text xpTextLevelPanel;

    [Header("Animation Settings")]
    public float xpFillSpeed = 1f;
    public float popupDuration = 0.4f;
    public float popupScale = 1.3f;
    public float floatingSpeed = 2f;
    public float floatingAmount = 10f;

    public static PlayerLevelSystem Instance;

    private float targetXP = 0f;
    private float displayedXP = 0f;
    private bool isAnimating = false;
    private Vector3 basePosition;
    private Coroutine popupCoroutine;

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

        // NUEVO: Buscar barra de XP del panel de leveleo
        if (xpBarLevelPanel == null)
        {
            Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();
            foreach (Slider slider in allSliders)
            {
                if (slider.gameObject.scene.isLoaded && slider.gameObject.name == "XpBar_Leveleo")
                {
                    xpBarLevelPanel = slider;
                    break;
                }
            }
        }

        // NUEVO: Buscar texto de XP del panel de leveleo
        if (xpTextLevelPanel == null)
        {
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (TMP_Text text in allTexts)
            {
                if (text.gameObject.scene.isLoaded && text.gameObject.name == "XpText_Leveleo")
                {
                    xpTextLevelPanel = text;
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
                    {
                        levelUpText.gameObject.SetActive(false);
                        basePosition = levelUpText.GetComponent<RectTransform>().anchoredPosition3D;
                    }
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

            UpdateXPVisuals();

            if (Mathf.Approximately(displayedXP, targetXP))
            {
                isAnimating = false;
            }
        }

        // Efecto de flotación (si el texto está visible)
        if (levelUpText != null && levelUpText.gameObject.activeSelf)
        {
            RectTransform rect = levelUpText.GetComponent<RectTransform>();
            rect.anchoredPosition3D = basePosition + new Vector3(0f, Mathf.Sin(Time.time * floatingSpeed) * floatingAmount, 0f);
        }
    }

    void UpdateXPVisuals()
    {
        // Actualizar barra HUD
        if (xpBarHUD != null)
            xpBarHUD.value = displayedXP;

        if (xpTextHUD != null)
            xpTextHUD.text = Mathf.FloorToInt(displayedXP) + " / " + xpToNextLevel;

        // NUEVO: Actualizar barra del panel de leveleo
        if (xpBarLevelPanel != null)
            xpBarLevelPanel.value = displayedXP;

        if (xpTextLevelPanel != null)
            xpTextLevelPanel.text = Mathf.FloorToInt(displayedXP) + " / " + xpToNextLevel;
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

        // Resetear ambas barras a 0
        if (xpBarHUD != null)
            xpBarHUD.value = 0;
        if (xpTextHUD != null)
            xpTextHUD.text = "0 / " + xpToNextLevel;

        if (xpBarLevelPanel != null)
            xpBarLevelPanel.value = 0;
        if (xpTextLevelPanel != null)
            xpTextLevelPanel.text = "0 / " + xpToNextLevel;

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

            if (popupCoroutine != null)
                StopCoroutine(popupCoroutine);
            popupCoroutine = StartCoroutine(AnimateLevelUpPopup());
        }
    }

    System.Collections.IEnumerator AnimateLevelUpPopup()
    {
        RectTransform rect = levelUpText.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;

        // Escala hacia adelante (aparece)
        float timer = 0f;
        while (timer < popupDuration)
        {
            float t = timer / popupDuration;
            float scale = Mathf.Lerp(0f, popupScale, Mathf.SmoothStep(0, 1, t));
            rect.localScale = Vector3.one * scale;
            timer += Time.deltaTime;
            yield return null;
        }

        rect.localScale = Vector3.one * popupScale;

        // Mantiene visible 2.5 segundos
        yield return new WaitForSeconds(2.5f);

        // Escala hacia atrás (desaparece)
        timer = 0f;
        while (timer < popupDuration)
        {
            float t = timer / popupDuration;
            float scale = Mathf.Lerp(popupScale, 0f, Mathf.SmoothStep(0, 1, t));
            rect.localScale = Vector3.one * scale;
            timer += Time.deltaTime;
            yield return null;
        }

        rect.localScale = Vector3.zero;
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

        // NUEVO: También actualizar el max del panel
        if (xpBarLevelPanel != null)
            xpBarLevelPanel.maxValue = xpToNextLevel;
    }
}