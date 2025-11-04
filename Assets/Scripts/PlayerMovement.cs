using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    private float currentSpeed;

    [Header("Cámara y Control")]
    public float mouseSensitivity = 0.5f;
    public float jumpForce = 5f;
    public float gravity = -5f;

    private CharacterController controller;
    private Transform cam;
    private Animator animator;

    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded; // 🔊 NUEVO: para detectar aterrizaje

    [Header("Chequeo de suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool isAttacking = false;
    public float attackDuration = 0.8f;

    [Header("Referencias")]
    public PlayerLevelUI menu;

    // 🔊 NUEVO: Control de pasos
    private bool estaCaminando = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // --- Si el menú está abierto ---
        if (menu != null && menu.menuAbierto)
        {
            animator.SetFloat("Speed", 0);
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            
            // 🔊 NUEVO: Detener pasos si el menú está abierto
            if (AudioManager.instance != null)
                AudioManager.instance.DetenerPasos();
            
            return;
        }

        // --- Detección de suelo ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 🔊 NUEVO: Detectar aterrizaje
        if (isGrounded && !wasGrounded && velocity.y < 0)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.SonidoPlayerAterrizaje();
        }
        wasGrounded = isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Sprint ---
        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift) && isGrounded;
        currentSpeed = estaCorriendo ? sprintSpeed : walkSpeed;

        // --- Movimiento ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Animación de movimiento escalada por velocidad ---
        float moveAmount = new Vector3(x, 0, z).magnitude;

        // Escalamos según velocidad actual (para que al pasar cierto valor, corra)
        float normalizedSpeed = moveAmount * (currentSpeed / sprintSpeed);
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

        animator.SetFloat("Speed", normalizedSpeed);

        // 🔊 NUEVO: Control de sonidos de pasos
        bool deberiaReproducirPasos = moveAmount > 0.1f;
        Debug.Log($"isGrounded: {isGrounded}, moveAmount: {moveAmount}, deberiaReproducir: {deberiaReproducirPasos}");

        if (deberiaReproducirPasos && !estaCaminando)
        {
            Debug.Log("🔊 INICIANDO PASOS!");

            // Comenzar a reproducir pasos
            if (AudioManager.instance != null)
            {
                Debug.Log("✅ IniciarPasos() llamado");
                AudioManager.instance.IniciarPasos();

            }
            else
            {
                Debug.LogError("❌ AudioManager.instance es NULL");
            }
            estaCaminando = true;
        }
        else if (!deberiaReproducirPasos && estaCaminando)
        {
            Debug.Log("🔇 DETENIENDO PASOS");

            // Detener pasos
            if (AudioManager.instance != null)
                AudioManager.instance.DetenerPasos();
            estaCaminando = false;
        }

        // Ajustar velocidad de pasos según si está corriendo o caminando
        if (estaCaminando && AudioManager.instance != null)
        {
            AudioManager.instance.AjustarVelocidadPasos(estaCorriendo);
        }

        // --- Rotación de cámara ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- Salto ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetBool("IsJumping", true);
            
            // 🔊 NUEVO: Reproducir sonido de salto
            if (AudioManager.instance != null)
                AudioManager.instance.SonidoPlayerSalto();
            
            Invoke(nameof(StopJumping), 0.5f);
        }

        // --- Ataque ---
        // NOTA: Este código de ataque debería estar en PlayerAttack.cs
        // Mantenido aquí por compatibilidad con tu proyecto actual
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            
            // 🔊 NUEVO: Reproducir sonido de ataque
            if (AudioManager.instance != null)
                AudioManager.instance.SonidoPlayerAtaque();
            
            Invoke(nameof(ResetAttack), attackDuration);
        }

        // --- Gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void StopJumping() => animator.SetBool("IsJumping", false);
    void ResetAttack() => isAttacking = false;

    // 🔊 NUEVO: Asegurar que los pasos se detengan cuando se desactiva el script
    void OnDisable()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.DetenerPasos();
    }
}
