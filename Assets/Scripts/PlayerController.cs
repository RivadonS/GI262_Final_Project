using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 8f, jumpForce = 12f, dashSpeed = 20f, dashDuration = 0.2f;
    public float dashStaminaCost = 15f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Input Actions")]
    public InputAction moveAction, jumpAction, dashAction;

    private Rigidbody2D rb;
    private bool canDoubleJump, isDashing;
    private PlayerStats playerStats;

    private bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    private Vector2 MoveInput => moveAction.ReadValue<Vector2>();

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>(); // ดึง Component PlayerStats มาเตรียมใช้งาน
    }
    private void OnEnable() { moveAction.Enable(); jumpAction.Enable(); dashAction.Enable(); }
    private void OnDisable() { moveAction.Disable(); jumpAction.Disable(); dashAction.Disable(); }

    private void Update()
    {
        if (isDashing) return;

        if (IsGrounded && rb.linearVelocity.y <= 0.1f) canDoubleJump = true;

        if (jumpAction.WasPressedThisFrame() && (IsGrounded || canDoubleJump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = IsGrounded; 
        }

        if (dashAction.WasPressedThisFrame())
        {
            if (playerStats.TryUseStamina(dashStaminaCost))
            {
                StartCoroutine(DashRoutine());
            }
        }

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
        
        float ogGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = ogGravity;
        isDashing = false;
    }
}