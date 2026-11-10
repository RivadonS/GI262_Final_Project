using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float maxHealth = 100f;
    public float attackDamage = 5f;
    protected float currentHealth;

    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private Vector2 currentTargetPos;
    
    private float flipTimer;

    [Header("Detection Settings")]
    public Transform groundDetect;
    public Transform wallDetect;
    public float rayDistance = 0.5f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    protected Rigidbody2D rb;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // เซ็ตเลือดเต็มตอนเกิด
        
        Vector2 startPos = transform.position;
        leftBoundary = new Vector2(startPos.x - patrolDistance, startPos.y);
        rightBoundary = new Vector2(startPos.x + patrolDistance, startPos.y);
        
        currentTargetPos = rightBoundary; 
    }

    protected virtual void Update()
    {
        flipTimer -= Time.deltaTime;
        CheckWaypoints();
        CheckObstacles();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        float direction = (currentTargetPos.x > transform.position.x) ? 1f : -1f;

        if (direction > 0 && !isFacingRight) Flip();
        else if (direction < 0 && isFacingRight) Flip();

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    protected virtual void CheckWaypoints()
    {
        if (Mathf.Abs(transform.position.x - currentTargetPos.x) < 0.2f)
        {
            SwitchTarget();
        }
    }

    protected virtual void CheckObstacles() { }

    protected void SwitchTarget()
    {
        if (flipTimer > 0f) return;

        currentTargetPos = (currentTargetPos == leftBoundary) ? rightBoundary : leftBoundary;
        flipTimer = 0.5f;
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    // ฟังก์ชันรับดาเมจจาก Player
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " โดนตี! เลือดเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // ทำลายตัวเองทิ้ง (SpawnNode จะตรวจจับได้ทันทีว่าตัวนี้ตายแล้วและเริ่มนับเวลาเกิดใหม่)
        Destroy(gameObject); 
    }

    // เมื่อเดินชนผู้เล่น (ใช้ Physics 2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Vector2 startPos = Application.isPlaying ? (leftBoundary + rightBoundary) / 2f : (Vector2)transform.position;
        Vector2 left = new Vector2(startPos.x - patrolDistance, startPos.y);
        Vector2 right = new Vector2(startPos.x + patrolDistance, startPos.y);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(left, 0.3f);
        Gizmos.DrawWireSphere(right, 0.3f);
        Gizmos.DrawLine(left, right);

        Gizmos.color = Color.red;
        if (groundDetect != null) Gizmos.DrawRay(groundDetect.position, Vector2.down * rayDistance);
        if (wallDetect != null)
        {
            Vector2 forwardDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(wallDetect.position, forwardDir * rayDistance);
        }
    }
}