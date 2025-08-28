using UnityEngine;

public class VoidDeath : MonoBehaviour
{
    [Header("Altura mínima antes de morir")]
    public float deathY = -10f;   // cuando el player cae por debajo de este valor

    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.y < deathY)
        {
            // Llamamos al GameManager
            GameManager.instance.PlayerDied();
        }
    }
}
