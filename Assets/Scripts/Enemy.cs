using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public int maxHealth = 100;

    [Header("Patrol Waypoints")]
    public Transform pointA;
    public Transform pointB;
    protected Transform currentTarget; // เก็บเป้าหมายปัจจุบัน

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
        if (pointA != null) pointA.SetParent(null);
        if (pointB != null) pointB.SetParent(null);
        currentTarget = pointB; // ให้เริ่มเดินไปที่จุด B ก่อน
    }

    protected virtual void Update()
    {
        CheckWaypoints();
        CheckObstacles();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (currentTarget == null) return;

        // 1. หาว่าเป้าหมายอยู่ซ้ายหรือขวาเทียบกับตำแหน่งปัจจุบัน
        float direction = (currentTarget.position.x > transform.position.x) ? 1f : -1f;

        // 2. หันหน้าอัตโนมัติตามเป้าหมาย (ไม่ต้องไปผูกกับเลเซอร์แล้ว)
        if (direction > 0 && !isFacingRight) Flip();
        else if (direction < 0 && isFacingRight) Flip();

        // 3. เดินไปข้างหน้าตามทิศทาง (แกน Y ปล่อยไว้ตามฟิสิกส์)
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    protected virtual void CheckWaypoints()
    {
        if (pointA == null || pointB == null) return;

        // ถ้าเดินมาใกล้เป้าหมายปัจจุบันมากพอ (ระยะห่างน้อยกว่า 0.2f) ให้สลับเป้าหมาย
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.2f)
        {
            SwitchTarget();
        }
    }

    // ฟังก์ชันเปล่า ให้คลาสลูกเอาไป override (เพราะลูกแต่ละตัวเช็คอุปสรรคไม่เหมือนกัน)
    protected virtual void CheckObstacles() { }

    protected void SwitchTarget()
    {
        // สลับหมุด A กับ B
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // วาดหมุดและเส้นเชื่อมสีเขียว
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // วาดเลเซอร์สีแดง (ใส่ if ดัก null ไว้ เพื่อให้รองรับศัตรูบางตัวที่ไม่มีเลเซอร์)
        Gizmos.color = Color.red;
        if (groundDetect != null)
            Gizmos.DrawRay(groundDetect.position, Vector2.down * rayDistance);
            
        if (wallDetect != null)
        {
            Vector2 forwardDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(wallDetect.position, forwardDir * rayDistance);
        }
    }
}