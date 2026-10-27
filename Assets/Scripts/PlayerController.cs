using System.Collections; // อย่าลืมตัวนี้สำหรับใช้ Coroutine
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Double Jump Settings")]
    private bool canDoubleJump; // ตัวเช็คว่ากระโดดขั้น 2 ได้ไหม

    [Header("Dash Settings")]
    public float dashSpeed = 20f; // ความเร็วตอนพุ่ง
    public float dashDuration = 0.2f; // ระยะเวลาที่พุ่ง
    public float dashCooldown = 1f; // ระยะเวลาคูลดาวน์ก่อนพุ่งได้อีกครั้ง
    private bool isDashing; // สถานะว่ากำลังพุ่งอยู่ไหม
    private bool canDash = true; // สถานะว่าพร้อมพุ่งไหม

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction dashAction; // เพิ่มปุ่ม Dash

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isFacingRight = true;
    private float originalGravity; // เก็บค่าแรงโน้มถ่วงเดิมไว้

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale; // จำค่าแรงโน้มถ่วงตั้งต้นไว้
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable(); // เปิดใช้งานปุ่ม Dash
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }

    private void Update()
    {
        // ถ้าระหว่างพุ่งตัวอยู่ จะไม่รับคำสั่งอย่างอื่นเลย (ห้ามเดิน ห้ามกระโดด ห้ามหันหน้า)
        if (isDashing) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        moveInput = moveAction.ReadValue<Vector2>();

        // รีเซ็ต Double Jump เมื่อเท้าแตะพื้น 
        // (เช็ค linearVelocity.y <= 0.1f ด้วย เพื่อกันการรีเซ็ตผิดจังหวะตอนกำลังเด้งขึ้น)
        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            canDoubleJump = true;
        }

        // ระบบกระโดด & Double Jump
        if (jumpAction.WasPressedThisFrame())
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // กระโดดขั้น 2 ไปแล้ว หมดสิทธิ์
            }
        }

        // ระบบ Dash
        if (dashAction.WasPressedThisFrame() && canDash)
        {
            StartCoroutine(DashRoutine());
        }

        // ระบบหันหน้า
        if (moveInput.x > 0f && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0f && isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // ถ้าพุ่งตัวอยู่ ให้ข้ามการอัปเดตการเดินปกติไปเลย
        if (isDashing) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        // ปรับแกน Y เป็น jumpForce ทันที (รีเซ็ตความเร็วตกก่อนกระโดด เพื่อให้ Double Jump พุ่งขึ้นสุดเสมอ)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // กระบวนการ Dash ทำงานเป็น Sequence ตามเวลา
    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        
        rb.gravityScale = 0f; // ปิดแรงโน้มถ่วง จะได้พุ่งตรงๆ ไม่ตกลงพื้น

        // หาทิศทางจากสเกลของตัวละคร (ถ้าหันขวา x จะเป็น 1 ถ้าหันซ้ายจะเป็น -1)
        float dashDirection = Mathf.Sign(transform.localScale.x);
        
        // อัดความเร็วพุ่งตัว
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        // รอจนกว่าจะหมดเวลา Dash
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity; // คืนค่าแรงโน้มถ่วง
        isDashing = false;

        // รอเวลาคูลดาวน์ก่อนจะเปิดให้ Dash ได้อีกครั้ง
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}