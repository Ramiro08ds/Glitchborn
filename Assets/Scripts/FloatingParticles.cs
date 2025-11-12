using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Sistema de partículas flotantes estilo ceniza/polvo
/// Igual al efecto del HTML - suben lentamente con deriva horizontal
/// </summary>
public class FloatingParticles : MonoBehaviour
{
    [Header("Sprites de Partículas")]
    [Tooltip("Puedes asignar varios sprites para variedad")]
    public Sprite[] particleSprites;
    
    [Header("Spawn Settings")]
    [Tooltip("Cantidad máxima de partículas")]
    public int maxParticles = 50;
    
    [Tooltip("Partículas que aparecen por segundo")]
    public float spawnRate = 5f;
    
    [Header("Movement")]
    [Tooltip("Velocidad vertical base (píxeles/segundo)")]
    public float baseSpeed = 30f;
    
    [Tooltip("Variación de velocidad (±)")]
    public float speedVariation = 10f;
    
    [Tooltip("Deriva horizontal máxima")]
    public float horizontalDrift = 50f;
    
    [Header("Appearance")]
    [Tooltip("Tamaño mínimo de partículas")]
    public float minSize = 2f;
    
    [Tooltip("Tamaño máximo de partículas")]
    public float maxSize = 6f;
    
    [Header("Colors")]
    [Tooltip("Colores posibles para las partículas")]
    public Color[] particleColors = new Color[]
    {
        new Color(0.18f, 0.8f, 0.44f, 0.6f),  // Verde
        new Color(0.2f, 0.6f, 0.86f, 0.6f),   // Azul
        new Color(0.9f, 0.3f, 0.24f, 0.6f)    // Rojo
    };
    
    [Header("Lifetime")]
    [Tooltip("Tiempo de vida mínimo (segundos)")]
    public float minLifetime = 5f;
    
    [Tooltip("Tiempo de vida máximo (segundos)")]
    public float maxLifetime = 10f;
    
    private RectTransform canvasRect;
    private List<Particle> particles = new List<Particle>();
    private float spawnTimer = 0f;
    
    class Particle
    {
        public GameObject gameObject;
        public RectTransform rectTransform;
        public Image image;
        public Vector2 velocity;
        public float lifetime;
        public float maxLifetime;
        public Color baseColor;
    }
    
    void Start()
    {
        canvasRect = GetComponent<RectTransform>();
        
        if (particleSprites == null || particleSprites.Length == 0)
        {
            Debug.LogWarning("⚠️ No hay sprites asignados. Asigna particle_soft.png u otros en el Inspector.");
        }
        
        if (particleColors == null || particleColors.Length == 0)
        {
            particleColors = new Color[] { Color.white };
        }
    }
    
    void Update()
    {
        // Spawn nuevas partículas
        spawnTimer += Time.deltaTime;
        float spawnInterval = 1f / spawnRate;
        
        if (spawnTimer >= spawnInterval && particles.Count < maxParticles)
        {
            SpawnParticle();
            spawnTimer = 0f;
        }
        
        // Actualizar partículas existentes
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            UpdateParticle(particles[i], Time.deltaTime);
            
            // Eliminar si salió de pantalla o expiró
            if (IsOutOfBounds(particles[i]) || particles[i].lifetime <= 0)
            {
                Destroy(particles[i].gameObject);
                particles.RemoveAt(i);
            }
        }
    }
    
    void SpawnParticle()
    {
        if (particleSprites == null || particleSprites.Length == 0) return;
        
        // Crear GameObject
        GameObject particleGO = new GameObject("FloatingParticle");
        particleGO.transform.SetParent(transform, false);
        
        // Agregar Image
        Image img = particleGO.AddComponent<Image>();
        img.sprite = particleSprites[Random.Range(0, particleSprites.Length)];
        img.raycastTarget = false;
        
        // Color aleatorio
        Color color = particleColors[Random.Range(0, particleColors.Length)];
        img.color = color;
        
        // Configurar RectTransform
        RectTransform rect = particleGO.GetComponent<RectTransform>();
        
        // Tamaño aleatorio
        float size = Random.Range(minSize, maxSize);
        rect.sizeDelta = new Vector2(size, size);
        
        // Posición inicial: abajo de pantalla, X aleatorio
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        
        float startX = Random.Range(-canvasWidth / 2, canvasWidth / 2);
        float startY = -canvasHeight / 2 - 50;
        rect.anchoredPosition = new Vector2(startX, startY);
        
        // Velocidad aleatoria
        float vertSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);
        float horzSpeed = Random.Range(-horizontalDrift, horizontalDrift);
        
        // Crear datos de partícula
        Particle particle = new Particle
        {
            gameObject = particleGO,
            rectTransform = rect,
            image = img,
            velocity = new Vector2(horzSpeed, vertSpeed),
            maxLifetime = Random.Range(minLifetime, maxLifetime),
            lifetime = Random.Range(minLifetime, maxLifetime),
            baseColor = color
        };
        
        particles.Add(particle);
    }
    
    void UpdateParticle(Particle particle, float deltaTime)
    {
        // Mover partícula
        Vector2 pos = particle.rectTransform.anchoredPosition;
        pos += particle.velocity * deltaTime;
        particle.rectTransform.anchoredPosition = pos;
        
        // Actualizar lifetime
        particle.lifetime -= deltaTime;
        
        // Fade in/out
        float lifePercent = particle.lifetime / particle.maxLifetime;
        float alpha;
        
        if (lifePercent > 0.9f) // Fade in (primeros 10%)
        {
            alpha = (1f - lifePercent) * 10f;
        }
        else if (lifePercent < 0.1f) // Fade out (últimos 10%)
        {
            alpha = lifePercent * 10f;
        }
        else
        {
            alpha = 1f;
        }
        
        Color color = particle.baseColor;
        color.a = particle.baseColor.a * alpha;
        particle.image.color = color;
        
        // Rotación leve
        float rotSpeed = 10f;
        particle.rectTransform.Rotate(0, 0, rotSpeed * deltaTime);
    }
    
    bool IsOutOfBounds(Particle particle)
    {
        Vector2 pos = particle.rectTransform.anchoredPosition;
        float canvasHeight = canvasRect.rect.height;
        
        // Si salió por arriba
        return pos.y > canvasHeight / 2 + 100;
    }
}
