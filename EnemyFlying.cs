using UnityEngine;
using PixelGunner.Core;

namespace PixelGunner.Enemies
{
    /// <summary>
    /// Quái vật bay (dơi / sinh vật đột biến) bay lượn trên không, lượn sóng và bắn đạn hoặc lao xuống người chơi.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Health))]
    public class EnemyFlying : MonoBehaviour
    {
        [Header("--- THIẾT LẬP BAY ---")]
        [SerializeField] private float flySpeed = 3f;
        [SerializeField] private float waveFrequency = 2.5f;
        [SerializeField] private float waveMagnitude = 0.5f;

        [Header("--- TẤN CÔNG BẮN ĐẠN ---")]
        [SerializeField] private GameObject enemyBulletPrefab;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private float attackRange = 8f;

        [Header("--- RƠI VẬT PHẨM ---")]
        [SerializeField] private GameObject[] dropItems;

        private Transform playerTransform;
        private Health health;
        private float nextFireTime;
        private Vector3 startPos;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Start()
        {
            startPos = transform.position;
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }

            health.OnDeath += HandleDeathDrop;
            nextFireTime = Time.time + Random.Range(0.5f, 1.5f);
        }

        private void OnDestroy()
        {
            if (health != null) health.OnDeath -= HandleDeathDrop;
        }

        private void Update()
        {
            if (health.IsDead) return;

            // Chuyển động lượn sóng ngang
            float newY = startPos.y + Mathf.Sin(Time.time * waveFrequency) * waveMagnitude;
            transform.position = new Vector3(transform.position.x - flySpeed * Time.deltaTime, newY, transform.position.z);

            // Tự động quay lại nếu bay ra xa khỏi camera
            if (playerTransform != null)
            {
                // Hướng mặt về phía Player
                if (playerTransform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                // Bắn cầu lửa / đạn quái nếu người chơi trong tầm bắn
                float distance = Vector2.Distance(transform.position, playerTransform.position);
                if (distance <= attackRange && Time.time >= nextFireTime)
                {
                    ShootAtPlayer();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }

        private void ShootAtPlayer()
        {
            if (enemyBulletPrefab == null || playerTransform == null) return;

            Transform spawn = shootPoint != null ? shootPoint : transform;
            Vector2 direction = (playerTransform.position - spawn.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Instantiate(enemyBulletPrefab, spawn.position, Quaternion.Euler(0, 0, angle));
        }

        private void HandleDeathDrop()
        {
            if (dropItems != null && dropItems.Length > 0)
            {
                int idx = Random.Range(0, dropItems.Length);
                if (dropItems[idx] != null)
                {
                    Instantiate(dropItems[idx], transform.position, Quaternion.identity);
                }
            }
        }
    }
}
