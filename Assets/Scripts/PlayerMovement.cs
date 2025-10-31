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

    [Header("Chequeo de suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool isAttacking = false;
    public float attackDuration = 0.8f;

    [Header("Referencias")]
    public PlayerLevelUI menu;

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
            return;
        }

        // --- Detección de suelo ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Sprint ---
        currentSpeed = (Input.GetKey(KeyCode.LeftShift) && isGrounded) ? sprintSpeed : walkSpeed;

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
            Invoke(nameof(StopJumping), 0.5f);
        }

        // --- Ataque ---
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            Invoke(nameof(ResetAttack), attackDuration);
        }

        // --- Gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void StopJumping() => animator.SetBool("IsJumping", false);
    void ResetAttack() => isAttacking = false;
}
