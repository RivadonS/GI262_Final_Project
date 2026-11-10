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

    [Header("Combat Settings")]
    public Transform attackPoint;      // จุดศูนย์กลางวงสวิงดาบ (สร้าง Empty Object วางไว้ด้านหน้า Player)
    public float attackRange = 0.8f;   // รัศมีการฟันดาบ
    public LayerMask enemyLayer;       // Layer ของศัตรู
    public float attackCooldown = 0.5f;
    private float attackTimer = 0f;

    [Header("Input Actions")]
    public InputAction moveAction, jumpAction, dashAction, attackAction; // เพิ่ม attackAction

    private Rigidbody2D rb;
    private bool canDoubleJump, isDashing;
    private PlayerStats playerStats;

    private bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    private Vector2 MoveInput => moveAction.ReadValue<Vector2>();

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
    }

    private void OnEnable() { moveAction.Enable(); jumpAction.Enable(); dashAction.Enable(); attackAction.Enable(); } // เพิ่ม Enable
    private void OnDisable() { moveAction.Disable(); jumpAction.Disable(); dashAction.Disable(); attackAction.Disable(); } // เพิ่ม Disable

    private void Update()
    {
        if (isDashing) return;
        attackTimer -= Time.deltaTime;

        if (IsGrounded && rb.linearVelocity.y <= 0.1f) canDoubleJump = true;

        if (jumpAction.WasPressedThisFrame() && (IsGrounded || canDoubleJump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = IsGrounded; 
        }

        if (dashAction.WasPressedThisFrame() && playerStats.TryUseStamina(dashStaminaCost))
        {
            StartCoroutine(DashRoutine());
        }

        // ระบบโจมตี
        if (attackAction.WasPressedThisFrame() && attackTimer <= 0f)
        {
            Attack();
        }

        if (MoveInput.x != 0) 
            transform.localScale = new Vector3(Mathf.Sign(MoveInput.x), 1f, 1f);
    }

    private void FixedUpdate()
    {
        if (!isDashing) rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Attack()
    {
        attackTimer = attackCooldown; // รีเซ็ตคูลดาวน์

        // สร้างวงกลมรัศมีฟันดาบเพื่อหาศัตรูทั้งหมดที่อยู่ในระยะ
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemyScript = enemyCollider.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // โจมตีด้วยดาเมจจาก Stat STR ของเรา
                enemyScript.TakeDamage(playerStats.GetAttackDamage());
            }
        }
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

    // วาดวงกลมสีแดงใน Scene View เพื่อให้ปรับระยะฟันดาบง่ายๆ
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}