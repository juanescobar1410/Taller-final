using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementDebug : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    [Header("Física")]
    public float gravity = -9.81f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;          // asigna un empty en los pies
    public float groundRadius = 0.16f;
    public LayerMask groundMask;           // marca la capa del suelo en el inspector

    [Header("Debug")]
    public bool logToConsole = true;       // activa mensajes en consola
    public bool disableAnimator = false;   // si true, desactiva el Animator para probar
    public bool drawGizmos = true;

    private CharacterController controller;
    private Animator animator;

    // input system
    private Vector2 moveInput;
    private bool jumpPressed;

    private Vector3 velocity;
    private bool isGroundedByController;
    private bool isGroundedBySphere;

    // animator hashes (si los usas)
    private static readonly int VelX = Animator.StringToHash("velX");
    private static readonly int VelY = Animator.StringToHash("velY");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int LandingHash = Animator.StringToHash("Landing");

    private float velXCur, velYCur;
    [SerializeField] private float animDamp = 0.08f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (groundCheck == null)
        {
            // crear groundCheck por defecto justo encima de los pies
            GameObject go = new GameObject("GroundCheck");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0f, -controller.height * 0.5f + 0.1f, 0f);
            groundCheck = go.transform;
        }

        if (animator == null)
        {
            if (logToConsole) Debug.LogWarning("[DEBUG] No se encontró Animator en el objeto.");
            disableAnimator = true;
        }

        if (disableAnimator && animator != null)
            animator.enabled = false;
    }

    // Input System callbacks
    public void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) jumpPressed = true;
    }

    private void Update()
    {
        // 1) Detectar suelo (dos métodos)
        isGroundedByController = controller.isGrounded;
        isGroundedBySphere = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        // logs de diagnóstico (una vez por frame)
        if (logToConsole)
        {
            Debug.Log($"[DEBUG] groundCheckPos={groundCheck.position:F3} | controller.isGrounded={isGroundedByController} | CheckSphere={isGroundedBySphere} | velY={velocity.y:F3}");
        }

        // 2) Si alguna detecta suelo lo consideramos grounded
        bool isGrounded = isGroundedByController || isGroundedBySphere;

        // 3) Mantener velocidad vertical estable cuando grounded
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // fuerza pequeña hacia abajo para "pegar" al suelo
            // activar/desactivar bools del animator si está activo
            if (!disableAnimator)
            {
                animator.SetBool(JumpHash, false);
                animator.SetBool(LandingHash, true);
            }
        }

        // 4) Movimiento horizontal (igual que antes)
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveWorld = transform.right * input.x + transform.forward * input.z;

        // Rotación suave si hay input
        Vector3 lookDir = new Vector3(moveWorld.x, 0f, moveWorld.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        controller.Move(moveWorld * moveSpeed * Time.deltaTime);

        // 5) Salto (si estamos grounded por cualquiera de los métodos)
        if (isGrounded && jumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpPressed = false;
            if (!disableAnimator)
            {
                animator.SetBool(JumpHash, true);
                animator.SetBool(LandingHash, false);
            }
            if (logToConsole) Debug.Log("[DEBUG] Saltando: velocity.y set");
        }

        // 6) Gravedad y movimiento vertical
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 7) Enviar parametros de movimiento al animator (si no está desactivado)
        if (!disableAnimator)
        {
            velXCur = Mathf.Lerp(velXCur, moveInput.x, animDamp);
            velYCur = Mathf.Lerp(velYCur, moveInput.y, animDamp);
            animator.SetFloat(VelX, velXCur);
            animator.SetFloat(VelY, velYCur);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);

        // dibujar "bottom" del CharacterController para referencia
        if (controller != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 bottomCenter = transform.position + controller.center - Vector3.up * (controller.height * 0.5f - controller.radius);
            Gizmos.DrawWireSphere(bottomCenter, controller.radius);
        }
    }
}