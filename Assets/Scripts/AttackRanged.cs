using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AttackRanged : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    [Header("Stats")]
    public float attackRange = 10f;
    public float attackCooldown = 1.5f;
    public float projectileSpeed = 15f;
    private float lastAttackTime;

    [Header("Telegraph Settings")]
    public Color telegraphColor = Color.red;
    public float telegraphWidth = 0.05f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Crear y configurar el LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = telegraphWidth;
        lineRenderer.endWidth = telegraphWidth;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = telegraphColor;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Si está lejos, lo persigue
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            lineRenderer.enabled = false;
        }
        else
        {
            // Si está en rango, se detiene y mira al jugador
            agent.isStopped = true;
            transform.LookAt(player.position);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(TelegraphAndShoot());
                lastAttackTime = Time.time;
            }
        }
    }

    private IEnumerator TelegraphAndShoot()
    {
        // Verificar si hay línea de visión (no dispara si hay obstáculos)
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, directionToPlayer, out hit, attackRange))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                yield break; // no ve al jugador → no dispara
            }
        }

        // Mostrar línea roja de advertencia
        lineRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < attackCooldown)
        {
            elapsed += Time.deltaTime;

            // Actualiza las posiciones de la línea cada frame
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, player.position);

            yield return null;
        }

        lineRenderer.enabled = false;
        Shoot();
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 direction = (player.position - firePoint.position).normalized;
        rb.velocity = direction * projectileSpeed;

        Destroy(projectile, 5f);
    }

}
