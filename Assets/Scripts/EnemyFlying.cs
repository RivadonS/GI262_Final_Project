using UnityEngine;

public class FlyingEnemy : Enemy
{
    protected override void CheckObstacles()
    {
        if (wallDetect == null) return;

        Vector2 forwardDir = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetect.position, forwardDir, rayDistance, wallLayer);

        if (wallInfo.collider != null)
        {
            SwitchTarget();
        }
    }
}