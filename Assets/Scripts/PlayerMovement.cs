using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    private float currentSpeed;

    [Header("Cámara y Control")]
    public float mouseSensitivity = 0.5f;
    public float jumpForce = 5f;
    public float gravity = -5f;

    private CharacterController controller;
    private Transform cam;
    private Animator animator; // 👈 Referencia al Animator

    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Chequeo de suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        animator = GetComponent<Animator>(); // 👈 Busca el Animator en el mismo objeto
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed; // Arranca caminando
    }

    void Update()
    {
        // --- Detección de suelo ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Mantener pegado al suelo
        }

        // --- Sprint ---
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // --- Movimiento horizontal ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Animaciones de movimiento ---
        float speedPercent = new Vector2(x, z).magnitude * (currentSpeed == sprintSpeed ? 2f : 1f);
        animator.SetFloat("Speed", speedPercent);
        // 👆 "Speed" controla Idle/Walk/Run en el Animator

        // --- Rotación con el mouse ---
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
            animator.SetBool("IsJumping", true); // 👈 Activa animación de salto
        }

        // --- Si está en el suelo, dejar de saltar ---
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        // --- Ataque (clic izquierdo) ---
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack"); // 👈 Dispara animación de ataque
        }

        // --- Aplicar gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
