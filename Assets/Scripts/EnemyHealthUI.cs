using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider healthSlider;   // referencia al Slider de UI
    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = enemyHealth.maxHealth;
            healthSlider.value = enemyHealth.maxHealth;
        }
    }

    void Update()
    {
        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.value = enemyHealth.CurrentHealth; // 👈 vida actual
        }
    }
}
