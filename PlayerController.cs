using UnityEngine;

namespace PixelGunner.Player
{
    /// <summary>
    /// Điều khiển di chuyển, nhảy và tương tác vật lý của nhân vật chính (Player).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("--- THÔNG SỐ DI CHUYỂN ---")]
        [Tooltip("Tốc độ chạy ngang")]
        [SerializeField] private float moveSpeed = 8f;

        [Tooltip("Lực nhảy")]
        [SerializeField] private float jumpForce = 14f;

        [Header("--- KIỂM TRA MẶT ĐẤT (GROUND CHECK) ---")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckRadius = 0.25f;
        [SerializeField] private LayerMask groundLayer;

        [Header("--- THÀNH PHẦN KHÁC ---")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Trạng thái nội bộ
        private Rigidbody2D rb;
        private float horizontalInput;
        private bool isGrounded;
        private bool isFacingRight = true;
        private bool canMove = true;

        // Hỗ trợ cảm giác nhảy mượt mà (Coyote time & Jump buffer)
        private float coyoteTime = 0.15f;
        private float coyoteTimeCounter;
        private float jumpBufferTime = 0.15f;
        private float jumpBufferCounter;

        public bool IsFacingRight => isFacingRight;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (!canMove) return;

            // 1. Nhận input di chuyển ngang
            horizontalInput = Input.GetAxisRaw("Horizontal");

            // 2. Kiểm tra mặt đất
            if (groundCheckPoint != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
            }

            // 3. Xử lý Coyote Time (cho phép nhảy ngay sau khi vừa rời mép đất)
            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            // 4. Xử lý Jump Buffer (lưu lệnh nhảy nếu nhấn sớm trước khi chạm đất)
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            // 5. Kích hoạt nhảy khi thỏa mãn cả 2 điều kiện
            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            {
                PerformJump();
                jumpBufferCounter = 0f;
            }

            // Nhả phím nhảy sớm sẽ giảm độ cao nhảy (nhảy cao/thấp tùy lực bấm)
            if ((Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                coyoteTimeCounter = 0f;
            }

            // 6. Xoay hướng mặt nhân vật (Flip Sprite)
            HandleFlip();

            // 7. Cập nhật trạng thái Animator
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            if (!canMove) return;

            // Di chuyển ngang dựa trên Physics2D
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        private void PerformJump()
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        private void HandleFlip()
        {
            if (horizontalInput > 0.01f && !isFacingRight)
            {
                Flip();
            }
            else if (horizontalInput < -0.01f && isFacingRight)
            {
                Flip();
            }
        }

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
        }

        /// <summary>
        /// Kích hoạt trạng thái bị giật lùi (Knockback) khi trúng đòn từ quái.
        /// </summary>
        public void ApplyKnockback(Vector2 forceDirection, float forceDuration = 0.2f)
        {
            canMove = false;
            rb.velocity = Vector2.zero;
            rb.AddForce(forceDirection, ForceMode2D.Impulse);
            Invoke(nameof(ResetMovement), forceDuration);
        }

        private void ResetMovement()
        {
            canMove = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheckPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
            }
        }
    }
}
