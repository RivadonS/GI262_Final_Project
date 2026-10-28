using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 8f, jumpForce = 12f, dashSpeed = 20f, dashDuration = 0.2f, dashCooldown = 1f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Input Actions")]
    public InputAction moveAction, jumpAction, dashAction;

    private Rigidbody2D rb;
    private bool canDoubleJump, isDashing;
    private float dashTimer;

    private bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    private Vector2 MoveInput => moveAction.ReadValue<Vector2>();

    private void Awake() => rb = GetComponent<Rigidbody2D>();
    private void OnEnable() { moveAction.Enable(); jumpAction.Enable(); dashAction.Enable(); }
    private void OnDisable() { moveAction.Disable(); jumpAction.Disable(); dashAction.Disable(); }

    private void Update()
    {
        dashTimer -= Time.deltaTime;
        if (isDashing) return;

        if (IsGrounded && rb.linearVelocity.y <= 0.1f) canDoubleJump = true;

        if (jumpAction.WasPressedThisFrame() && (IsGrounded || canDoubleJump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            // ทริค Boolean: ถ้าโดดจากพื้น (IsGrounded=true) ให้ canDoubleJump เป็น true เพื่อให้โดดต่อได้ 
            // แต่ถ้าโดดกลางอากาศ (IsGrounded=false) canDoubleJump จะกลายเป็น false ทันที (หมดสิทธิ์)
            canDoubleJump = IsGrounded; 
        }

        if (dashAction.WasPressedThisFrame() && dashTimer <= 0f)
            StartCoroutine(DashRoutine());

        if (MoveInput.x != 0) 
            transform.localScale = new Vector3(Mathf.Sign(MoveInput.x), 1f, 1f);
    }

    private void FixedUpdate()
    {
        if (!isDashing) rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        dashTimer = dashCooldown; // รีเซ็ตเวลาคูลดาวน์ทันที
        
        float ogGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = ogGravity;
        isDashing = false;
    }
}