using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    public float moveSpeed = 3f;
    public int maxHealth = 100;

    protected Rigidbody2D rb;
    protected bool isFacingRight = true; 

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        // ถ้าหันขวาให้ทิศทาง = 1, ถ้าซ้าย = -1
        float direction = isFacingRight ? 1f : -1f;
        
        // อัปเดต linearVelocity ให้เดินไปตามทิศทาง
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    // ฟังก์ชันกลับหลังหัน (ศัตรูส่วนใหญ่ใช้เหมือนกัน เลยเขียนไว้ในคลาสแม่เลย)
    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }
}