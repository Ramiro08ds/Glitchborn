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

        if (agent == null)
        {
            Debug.LogError("AttackRanged: Este enemigo necesita un NavMeshAgent!", this);
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Crear y configurar el LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = telegraphWidth;
        lineRenderer.endWidth = telegraphWidth;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = telegraphColor;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            lineRenderer.enabled = false;
        }
        else
        {
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
        if (firePoint == null || player == null)
            yield break;

        // Línea de visión
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        if (Physics.Raycast(firePoint.position, directionToPlayer, out hit, attackRange))
        {
            if (!hit.collider.CompareTag("Player"))
                yield break;
        }

        // Mostrar línea roja
        lineRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < attackCooldown)
        {
            elapsed += Time.deltaTime;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, player.position);
            yield return null;
        }

        lineRenderer.enabled = false;
        Shoot();
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        // NUEVO: Reproducir sonido de disparo AQUÍ
        if (AudioManager.instance != null)
            AudioManager.instance.SonidoEnemyShoot(firePoint.position);

        Vector3 direction = (player.position - firePoint.position).normalized;
        Vector3 spawnPos = firePoint.position + direction * 0.35f;
        Quaternion rot = Quaternion.LookRotation(direction);

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, rot);
        if (projectile == null) return;

        int projLayer = LayerMask.NameToLayer("EnemyProjectile");
        if (projLayer != -1)
            projectile.layer = projLayer;

        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider[] enemyColliders = GetComponentsInChildren<Collider>();
        if (projectileCollider != null)
        {
            foreach (var col in enemyColliders)
                Physics.IgnoreCollision(projectileCollider, col, true);
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = direction * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("[AttackRanged] Projectile no tiene Rigidbody!", this);
        }

        Destroy(projectile, 5f);
    }
}