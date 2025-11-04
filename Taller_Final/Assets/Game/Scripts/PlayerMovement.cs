using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f; // grados/seg

    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f; // altura del salto

    [Header("Optional")]
    [Tooltip("Si se asigna, el movimiento será relativo a esta cámara (ej: cámara orbital).")]
    public Camera mouseOrbitCamera;

    private CharacterController controller;
    private Animator anim;

    // Input System
    private Vector2 moveInput;
    private bool jumpPressed; // se activa al presionar el botón de salto

    private Vector3 velocity;

    // Hash del Animator
    private static readonly int VelX = Animator.StringToHash("velX");
    private static readonly int VelY = Animator.StringToHash("velY");

    [SerializeField] private float animDamp = 0.05f;
    private float velXCur, velYCur;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // === NUEVO INPUT SYSTEM ===
    // Movimiento
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    // Salto
    public void OnJump(InputAction.CallbackContext ctx)
    {
        // Solo marcamos el salto cuando se PRESIONA (no cuando se suelta)
        if (ctx.performed)
        {
            jumpPressed = true;
        }
    }

    private void Update()
    {
        // 1) Dirección de movimiento
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveWorld;

        if (mouseOrbitCamera != null && mouseOrbitCamera.gameObject.activeInHierarchy)
        {
            Vector3 camFwd = mouseOrbitCamera.transform.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = mouseOrbitCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveWorld = camRight * input.x + camFwd * input.z;
        }
        else
        {
            moveWorld = transform.right * input.x + transform.forward * input.z;
        }

        // 2) Rotación hacia dirección de movimiento
        Vector3 lookDir = new Vector3(moveWorld.x, 0f, moveWorld.z);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 3) Movimiento horizontal
        Vector3 horizontal = moveWorld * moveSpeed;
        controller.Move(horizontal * Time.deltaTime);

        // 4) Gravedad y salto
        if (controller.isGrounded)
        {
            if (velocity.y < 0f)
                velocity.y = -2f; // mantener grounded

            // Salto (Input System)
            if (jumpPressed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpPressed = false; // reseteamos
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5) Blend Tree (Animator)
        velXCur = Mathf.SmoothDamp(velXCur, moveInput.x, ref velXCur, animDamp);
        velYCur = Mathf.SmoothDamp(velYCur, moveInput.y, ref velYCur, animDamp);
        anim.SetFloat(VelX, velXCur);
        anim.SetFloat(VelY, velYCur);
    }
}