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

    [Header("Chequeo de suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Ataque")]
    public float attackDuration = 0.8f;

    [Header("Referencias")]
    public PlayerLevelUI menu;

    // --- Privadas ---
    private CharacterController controller;
    private Transform cam;
    private Animator animator;

    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isAttacking = false;
    private bool estaCaminando = false;

    // --------------------------------------------------------
    // 🔹 INICIALIZACIÓN
    // --------------------------------------------------------
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed;
    }

    // --------------------------------------------------------
    // 🔹 LOOP PRINCIPAL
    // --------------------------------------------------------
    void Update()
    {
        if (MenuAbierto()) return;

        CheckGround();
        HandleSprint();
        HandleMovement();
        HandleCamera();
        HandleJump();
        HandleAttack();
        ApplyGravity();
    }

    // --------------------------------------------------------
    // 🔹 MENÚ
    // --------------------------------------------------------
    private bool MenuAbierto()
    {
        if (menu != null && menu.menuAbierto)
        {
            animator.SetFloat("Speed", 0);
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Detener pasos si el menú está abierto
            AudioManager.instance?.DetenerPasos();
            return true;
        }
        return false;
    }

    // --------------------------------------------------------
    // 🔹 SUELO Y GRAVEDAD
    // --------------------------------------------------------
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Sonido de aterrizaje
        if (isGrounded && !wasGrounded && velocity.y < 0)
            AudioManager.instance?.SonidoPlayerAterrizaje();

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        wasGrounded = isGrounded;
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // --------------------------------------------------------
    // 🔹 MOVIMIENTO Y CORRER
    // --------------------------------------------------------
    private void HandleSprint()
    {
        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift) && isGrounded;
        currentSpeed = estaCorriendo ? sprintSpeed : walkSpeed;

        if (estaCaminando && AudioManager.instance != null)
            AudioManager.instance.AjustarVelocidadPasos(estaCorriendo);
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        float moveAmount = new Vector3(x, 0, z).magnitude;
        float normalizedSpeed = Mathf.Clamp01(moveAmount * (currentSpeed / sprintSpeed));

        animator.SetFloat("Speed", normalizedSpeed);
        HandleFootsteps(moveAmount);
    }

    // --------------------------------------------------------
    // 🔹 SONIDOS DE PASOS
    // --------------------------------------------------------
    private void HandleFootsteps(float moveAmount)
    {
        bool deberiaReproducirPasos = moveAmount > 0.1f;

        if (deberiaReproducirPasos && !estaCaminando)
        {
            AudioManager.instance?.IniciarPasos();
            estaCaminando = true;
        }
        else if (!deberiaReproducirPasos && estaCaminando)
        {
            AudioManager.instance?.DetenerPasos();
            estaCaminando = false;
        }
    }

    // --------------------------------------------------------
    // 🔹 CÁMARA Y ROTACIÓN
    // --------------------------------------------------------
    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // --------------------------------------------------------
    // 🔹 SALTO
    // --------------------------------------------------------
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetBool("IsJumping", true);

            AudioManager.instance?.SonidoPlayerSalto();
            Invoke(nameof(StopJumping), 0.5f);
        }
    }

    private void StopJumping() => animator.SetBool("IsJumping", false);

    // --------------------------------------------------------
    // 🔹 ATAQUE
    // --------------------------------------------------------
    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            AudioManager.instance?.SonidoPlayerAtaque();
            Invoke(nameof(ResetAttack), attackDuration);
        }
    }

    private void ResetAttack() => isAttacking = false;

    // --------------------------------------------------------
    // 🔹 EVENTOS UNITY
    // --------------------------------------------------------
    void OnDisable() => AudioManager.instance?.DetenerPasos();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
