using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public int maxHealth = 100;

    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private Vector2 currentTargetPos;
    
    private float flipTimer; // ⭐ เพิ่มตัวแปรกันบั๊กหันรัวๆ

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
        
        Vector2 startPos = transform.position;
        leftBoundary = new Vector2(startPos.x - patrolDistance, startPos.y);
        rightBoundary = new Vector2(startPos.x + patrolDistance, startPos.y);
        
        currentTargetPos = rightBoundary; 
    }

    protected virtual void Update()
    {
        flipTimer -= Time.deltaTime; // ⭐ นับเวลาถอยหลัง

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
        // ⭐ เปลี่ยนมาเช็คระยะแค่แกน X (Mathf.Abs) ทำให้ไม่ว่าจะร่วงมาจากฟ้าหรือลอยนิดๆ ก็เดินถึงเป้าหมายได้
        if (Mathf.Abs(transform.position.x - currentTargetPos.x) < 0.2f)
        {
            SwitchTarget();
        }
    }

    protected virtual void CheckObstacles() { }

    protected void SwitchTarget()
    {
        if (flipTimer > 0f) return; // ⭐ ถ้าเพิ่งหันมา ห้ามหันซ้ำทันทีเด็ดขาด (แก้บั๊กเต้นเบรกแดนซ์)

        currentTargetPos = (currentTargetPos == leftBoundary) ? rightBoundary : leftBoundary;
        flipTimer = 0.5f; // ⭐ เซ็ตคูลดาวน์ให้รอ 0.5 วินาทีถึงจะหันใหม่ได้
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
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