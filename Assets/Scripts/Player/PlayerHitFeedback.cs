using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHitFeedback : MonoBehaviour
{
    public static PlayerHitFeedback instance;

    [Header("Pantalla")]
    public Image damageOverlay;
    public Color damageColor = new Color(1, 0, 0, 0.25f);
    public float overlayFadeSpeed = 3.5f;

    void Awake()
    {
        instance = this;
        if (damageOverlay != null)
            damageOverlay.color = new Color(0, 0, 0, 0);
    }

    public void OnPlayerDamaged()
    {
        // Solo el borde rojo
        if (damageOverlay != null)
            StartCoroutine(ShowDamageOverlay());
    }

    IEnumerator ShowDamageOverlay()
    {
        if (damageOverlay == null) yield break;
        damageOverlay.color = damageColor;

        while (damageOverlay.color.a > 0)
        {
            damageOverlay.color = Color.Lerp(damageOverlay.color, new Color(0, 0, 0, 0), Time.unscaledDeltaTime * overlayFadeSpeed);
            yield return null;
        }
    }
}

