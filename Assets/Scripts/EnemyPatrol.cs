using UnityEngine;

public class EnemyPatrol : Enemy
{
    protected override void CheckObstacles()
    {
        if (groundDetect == null || wallDetect == null) return;

        // เช็คเหวด้วย groundLayer
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetect.position, Vector2.down, rayDistance, groundLayer);
        
        Vector2 forwardDir = isFacingRight ? Vector2.right : Vector2.left;
        // เช็คกำแพงเปลี่ยนไปใช้ wallLayer
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetect.position, forwardDir, rayDistance, wallLayer);

        if (groundInfo.collider == null || wallInfo.collider != null)
        {
            SwitchTarget();
        }
    }
}