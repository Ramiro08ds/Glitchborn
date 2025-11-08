using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float speed = 15f; // velocidad del proyectil
    private Vector3 moveDirection;

    // Inicializar la dirección desde otro script (AttackRanged)
    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
        // Ajuste automático para cualquier orientación del prefab
        // Calcula la rotación que apunta forward del proyectil (Z+) hacia la dirección del disparo
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, moveDirection);

        // NUEVO: Reproducir sonido al disparar
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoEnemyShoot(transform.position);
    }

    private void Update()
    {
        if (moveDirection != Vector3.zero)
        {
            // Mover el proyectil
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // NUEVO: Sonido de impacto al golpear al jugador
            if (AudioManager.instance != null)
                AudioManager.instance.SonidoProyectilImpacto(transform.position);

            PlayerHealthManager.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy"))
        {
            // NUEVO: Sonido de impacto al golpear una pared u obstáculo
            if (AudioManager.instance != null)
                AudioManager.instance.SonidoProyectilImpacto(transform.position);

            Destroy(gameObject);
        }
    }
}