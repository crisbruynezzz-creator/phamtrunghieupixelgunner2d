using UnityEngine;
using PixelGunner.Core;

namespace PixelGunner.Enemies
{
    /// <summary>
    /// Quái vật đi tuần tra trên mặt đất, phát hiện và truy đuổi người chơi khi vào tầm ngắm.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Health))]
    public class EnemyPatrol : MonoBehaviour
    {
        [Header("--- DI CHUYỂN TUẦN TRA ---")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float patrolSpeed = 2.5f;
        [SerializeField] private float chaseSpeed = 4.5f;

        [Header("--- TẦM PHÁT HIỆN & TẤN CÔNG ---")]
        [SerializeField] private float detectionRange = 6f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private int contactDamage = 15;
        [SerializeField] private float attackCooldown = 1.0f;

        [Header("--- RƠI VẬT PHẨM KHI CHẾT ---")]
        [SerializeField] private GameObject[] dropItems;
        [SerializeField] [Range(0f, 1f)] private float dropRate = 0.6f;

        private Rigidbody2D rb;
        private Transform currentTargetPoint;
        private Transform playerTransform;
        private Health health;
        private float lastAttackTime = 0f;
        private bool isFacingRight = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
        }

        private void Start()
        {
            if (pointA != null)
            {
                currentTargetPoint = pointA;
            }

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }

            health.OnDeath += HandleDeathDrop;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.OnDeath -= HandleDeathDrop;
            }
        }

        private void Update()
        {
            if (health.IsDead) return;

            // Kiểm tra khoảng cách tới Player
            float distanceToPlayer = float.MaxValue;
            if (playerTransform != null)
            {
                distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            }

            if (distanceToPlayer <= detectionRange)
            {
                // Trạng thái: Truy đuổi Player
                ChasePlayer(distanceToPlayer);
            }
            else
            {
                // Trạng thái: Đi tuần tra giữa A và B
                Patrol();
            }
        }

        private void Patrol()
        {
            if (pointA == null || pointB == null) return;

            Vector2 targetPos = new Vector2(currentTargetPoint.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, patrolSpeed * Time.deltaTime);

            if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(currentTargetPoint.position.x, 0)) < 0.2f)
            {
                currentTargetPoint = (currentTargetPoint == pointA) ? pointB : pointA;
            }

            UpdateFacing(currentTargetPoint.position.x - transform.position.x);
        }

        private void ChasePlayer(float distanceToPlayer)
        {
            if (playerTransform == null) return;

            Vector2 targetPos = new Vector2(playerTransform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

            UpdateFacing(playerTransform.position.x - transform.position.x);

            // Tấn công cận chiến khi đủ gần
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
        }

        private void AttackPlayer()
        {
            if (playerTransform == null) return;

            Health playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                Vector2 hitDirection = (playerTransform.position - transform.position).normalized;
                playerHealth.TakeDamage(contactDamage, hitDirection);
                lastAttackTime = Time.time;
            }
        }

        private void UpdateFacing(float directionX)
        {
            if (directionX > 0.05f && !isFacingRight)
            {
                Flip();
            }
            else if (directionX < -0.05f && isFacingRight)
            {
                Flip();
            }
        }

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
                {
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                    playerHealth.TakeDamage(contactDamage, hitDirection);
                    lastAttackTime = Time.time;
                }
            }
        }

        private void HandleDeathDrop()
        {
            if (dropItems == null || dropItems.Length == 0) return;

            if (Random.value <= dropRate)
            {
                int randomIndex = Random.Range(0, dropItems.Length);
                if (dropItems[randomIndex] != null)
                {
                    Instantiate(dropItems[randomIndex], transform.position + Vector3.up * 0.5f, Quaternion.identity);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
