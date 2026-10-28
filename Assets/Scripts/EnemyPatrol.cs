using UnityEngine;

public class PatrolEnemy : Enemy
{
    [Header("Patrol Settings")]
    public Transform groundDetect;
    public Transform wallDetect;
    public float rayDistance = 0.5f;
    public LayerMask groundLayer;

    private void Update()
    {
        // 1. ยิง Raycast ลงข้างล่าง (เช็คเหว)
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetect.position, Vector2.down, rayDistance, groundLayer);
        
        // 2. ยิง Raycast ไปข้างหน้า (เช็คกำแพง)
        Vector2 forwardDir = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetect.position, forwardDir, rayDistance, groundLayer);

        // ถ้า เลเซอร์ข้างล่างไม่โดนพื้น (สุดขอบ) หรือ เลเซอร์ข้างหน้าโดนกำแพง (ชนกำแพง)
        if (groundInfo.collider == null || wallInfo.collider != null)
        {
            Flip(); // เรียกใช้ฟังก์ชัน Flip() จากคลาสแม่ Enemy ได้เลย!
        }
    }

    // เอาไว้ให้ Unity วาดเส้นเลเซอร์สีแดงตอน Debug จะได้กะระยะถูก
    private void OnDrawGizmosSelected()
    {
        if (groundDetect != null && wallDetect != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(groundDetect.position, Vector2.down * rayDistance);
            
            Vector2 forwardDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(wallDetect.position, forwardDir * rayDistance);
        }
    }
}