using UnityEngine;

/// <summary>
/// Audio Manager - Sistema centralizado para gestionar todos los sonidos del juego
/// Patrón Singleton para acceso global
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("=== MÚSICA ===")]
    [Tooltip("Música de fondo para el nivel (cueva)")]
    public AudioClip musicaCueva;

    [Tooltip("Música del menú principal")]
    public AudioClip musicaMenu;

    [Tooltip("Música de Game Over")]
    public AudioClip musicaGameOver;

    [Header("=== SONIDOS DEL JUGADOR ===")]
    [Tooltip("Sonido de pasos (se reproduce en loop mientras camina)")]
    public AudioClip sonidoPasos;

    [Tooltip("Sonido de pasos al correr (se reproduce en loop mientras corre)")]
    public AudioClip sonidoPasosCorrer;

    [Tooltip("Sonido del salto")]
    public AudioClip sonidoSalto;

    [Tooltip("Sonido al aterrizar después de un salto")]
    public AudioClip sonidoAterrizaje;

    [Tooltip("Sonido del ataque con espada (swoosh)")]
    public AudioClip sonidoAtaqueEspada;

    [Tooltip("Sonido cuando el jugador golpea a un enemigo (impacto)")]
    public AudioClip sonidoPlayerGolpeaEnemigo;

    [Tooltip("Sonido cuando el jugador recibe daño")]
    public AudioClip sonidoPlayerDamage;

    [Tooltip("Sonido de muerte del jugador")]
    public AudioClip sonidoPlayerMuerte;

    [Tooltip("Sonido al subir de nivel")]
    public AudioClip sonidoLevelUp;

    [Header("=== SONIDOS DE ENEMIGOS ===")]
    [Tooltip("Sonido cuando un enemigo recibe daño")]
    public AudioClip sonidoEnemyDamage;

    [Tooltip("Sonido de muerte de enemigo")]
    public AudioClip sonidoEnemyMuerte;

    [Tooltip("Sonido de ataque del enemigo cuerpo a cuerpo")]
    public AudioClip sonidoEnemyAttack;

    [Tooltip("Sonido de disparo del enemigo mago")]
    public AudioClip sonidoEnemyShoot;

    [Tooltip("Sonido del proyectil al impactar")]
    public AudioClip sonidoProyectilImpacto;

    [Header("=== SONIDOS DE UI ===")]
    [Tooltip("Click en botones del menú")]
    public AudioClip sonidoBotonClick;

    [Tooltip("Abrir menú de pausa")]
    public AudioClip sonidoAbrirMenu;

    [Tooltip("Cerrar menú de pausa")]
    public AudioClip sonidoCerrarMenu;

    [Header("=== SONIDOS AMBIENTALES ===")]
    [Tooltip("Sonido de antorchas (fuego)")]
    public AudioClip sonidoAntorcha;

    [Header("=== CONFIGURACIÓN ===")]
    [Range(0f, 1f)]
    public float volumenMusica = 0.5f;

    [Range(0f, 1f)]
    public float volumenSFX = 0.8f;

    [Header("=== AJUSTES DE VOLUMEN ESPECÍFICOS ===")]
    [Range(0f, 2f)]
    [Tooltip("Multiplicador de volumen para el swoosh de la espada")]
    public float volumenSwoosh = 0.5f;

    [Range(0f, 2f)]
    [Tooltip("Multiplicador de volumen para el impacto al golpear enemigo")]
    public float volumenHitEnemigo = 1.2f;

    [Header("=== AJUSTES DE PASOS ===")]
    [Range(0.3f, 2f)]
    [Tooltip("Velocidad de reproducción de los pasos (pitch). Menor = más lento")]
    public float velocidadPasos = 1.0f;  // SIN PITCH - timing natural del audio

    // AudioSources internos
    private AudioSource musicaSource;
    private AudioSource sfxSource;
    private AudioSource pasosSource; // Dedicado para pasos (loop)

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Crea los AudioSources necesarios
    /// </summary>
    void InicializarAudioSources()
    {
        // AudioSource para música de fondo
        musicaSource = gameObject.AddComponent<AudioSource>();
        musicaSource.loop = true;
        musicaSource.playOnAwake = false;
        musicaSource.volume = volumenMusica;

        // AudioSource para efectos de sonido (SFX)
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = volumenSFX;

        // AudioSource dedicado para pasos
        pasosSource = gameObject.AddComponent<AudioSource>();
        pasosSource.loop = true;
        pasosSource.playOnAwake = false;
        pasosSource.volume = volumenSFX * 0.6f; // Pasos más suaves
    }

    #region MÚSICA

    /// <summary>
    /// Reproduce música de fondo
    /// </summary>
    public void ReproducirMusica(AudioClip clip)
    {
        if (clip == null) return;

        if (musicaSource.clip == clip && musicaSource.isPlaying)
            return; // Ya se está reproduciendo

        musicaSource.clip = clip;
        musicaSource.Play();
    }

    /// <summary>
    /// Detiene la música
    /// </summary>
    public void DetenerMusica()
    {
        musicaSource.Stop();
    }

    /// <summary>
    /// Cambia el volumen de la música
    /// </summary>
    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        musicaSource.volume = volumenMusica;
    }

    #endregion

    #region EFECTOS DE SONIDO

    /// <summary>
    /// Reproduce un efecto de sonido (SFX) una vez
    /// </summary>
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumenSFX);
    }

    /// <summary>
    /// Reproduce un efecto de sonido en una posición específica en el mundo 3D
    /// </summary>
    public void ReproducirSFX3D(AudioClip clip, Vector3 posicion)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, posicion, volumenSFX);
    }

    /// <summary>
    /// Cambia el volumen de los efectos de sonido
    /// </summary>
    public void SetVolumenSFX(float volumen)
    {
        volumenSFX = Mathf.Clamp01(volumen);
        sfxSource.volume = volumenSFX;
        pasosSource.volume = volumenSFX * 0.6f;
    }

    #endregion

    #region SONIDOS DE PASOS

    /// <summary>
    /// Inicia el sonido de pasos al caminar (loop)
    /// </summary>
    public void IniciarPasos()
    {
        if (sonidoPasos == null) return;

        // Si ya está corriendo, detener primero
        if (pasosSource.isPlaying && pasosSource.clip == sonidoPasosCorrer)
        {
            pasosSource.Stop();
        }

        if (!pasosSource.isPlaying || pasosSource.clip != sonidoPasos)
        {
            pasosSource.clip = sonidoPasos;
            pasosSource.pitch = velocidadPasos;
            pasosSource.Play();
        }
    }

    /// <summary>
    /// Inicia el sonido de pasos al correr (loop)
    /// </summary>
    public void IniciarPasosCorrer()
    {
        // Si no hay audio de correr, usar el de caminar
        if (sonidoPasosCorrer == null)
        {
            IniciarPasos();
            return;
        }

        // Si ya está caminando, detener primero
        if (pasosSource.isPlaying && pasosSource.clip == sonidoPasos)
        {
            pasosSource.Stop();
        }

        if (!pasosSource.isPlaying || pasosSource.clip != sonidoPasosCorrer)
        {
            pasosSource.clip = sonidoPasosCorrer;
            pasosSource.pitch = velocidadPasos;
            pasosSource.Play();
        }
    }

    /// <summary>
    /// Detiene el sonido de pasos
    /// </summary>
    public void DetenerPasos()
    {
        if (pasosSource != null && pasosSource.isPlaying)
            pasosSource.Stop();
    }

    #endregion

    #region MÉTODOS ESPECÍFICOS DEL JUEGO

    // ========== JUGADOR ==========
    public void SonidoPlayerSalto() => ReproducirSFX(sonidoSalto);
    public void SonidoPlayerAterrizaje() => ReproducirSFX(sonidoAterrizaje);
    public void SonidoPlayerAtaque() => sfxSource.PlayOneShot(sonidoAtaqueEspada, volumenSFX * volumenSwoosh);
    public void SonidoPlayerGolpeaEnemigo() => sfxSource.PlayOneShot(sonidoPlayerGolpeaEnemigo, volumenSFX * volumenHitEnemigo);
    public void SonidoPlayerDamage() => ReproducirSFX(sonidoPlayerDamage);
    public void SonidoPlayerMuerte() => ReproducirSFX(sonidoPlayerMuerte);
    public void SonidoLevelUp() => ReproducirSFX(sonidoLevelUp);

    // ========== ENEMIGOS ==========
    public void SonidoEnemyDamage(Vector3 posicion) => ReproducirSFX3D(sonidoEnemyDamage, posicion);
    public void SonidoEnemyMuerte(Vector3 posicion) => ReproducirSFX3D(sonidoEnemyMuerte, posicion);
    public void SonidoEnemyAttack(Vector3 posicion) => ReproducirSFX3D(sonidoEnemyAttack, posicion);
    public void SonidoEnemyShoot(Vector3 posicion) => ReproducirSFX3D(sonidoEnemyShoot, posicion);
    public void SonidoProyectilImpacto(Vector3 posicion) => ReproducirSFX3D(sonidoProyectilImpacto, posicion);

    // ========== UI ==========
    public void SonidoBotonClick() => ReproducirSFX(sonidoBotonClick);
    public void SonidoAbrirMenu() => ReproducirSFX(sonidoAbrirMenu);
    public void SonidoCerrarMenu() => ReproducirSFX(sonidoCerrarMenu);

    // ========== MÚSICA POR ESCENA ==========
    public void ReproducirMusicaCueva() => ReproducirMusica(musicaCueva);
    public void ReproducirMusicaMenu() => ReproducirMusica(musicaMenu);
    public void ReproducirMusicaGameOver() => ReproducirMusica(musicaGameOver);

    #endregion
}