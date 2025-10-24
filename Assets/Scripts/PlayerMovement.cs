using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 8f;
    public float sprintSpeed = 18f;
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
        // --- ⚡ Si el menú está abierto, no procesar inputs ---
        if (menu != null && menu.menuAbierto)
        {
            // Detener movimiento, animaciones de velocidad y rotación
            animator.SetFloat("Speed", 0);
            return;
        }

        // --- Detección de suelo ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- Sprint ---
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = walkSpeed;

        // --- Movimiento horizontal ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Animaciones de movimiento ---
        float moveAmount = new Vector3(x, 0, z).magnitude;
        float speedValue = moveAmount * currentSpeed;
        animator.SetFloat("Speed", speedValue);

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
            animator.SetBool("IsJumping", true);
            Invoke("StopJumping", 0.5f);
        }

        // --- Ataque ---
        if (Input.GetMouseButtonDown(0) && !isAttacking) 
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            Invoke("ResetAttack", attackDuration);
        }

        // --- Aplicar gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void StopJumping()
    {
        animator.SetBool("IsJumping", false);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }
}
