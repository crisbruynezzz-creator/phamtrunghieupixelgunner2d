using UnityEngine;
using PixelGunner.Core;

namespace PixelGunner.Player
{
    /// <summary>
    /// Xử lý chuyển động của viên đạn và va chạm gây sát thương lên quái vật.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bullet : MonoBehaviour
    {
        [Header("--- THÔNG SỐ ĐẠN ---")]
        [SerializeField] private float speed = 18f;
        [SerializeField] private int damage = 25;
        [SerializeField] private float lifeTime = 3f;

        [Header("--- HIỆU ỨNG VA CHẠM ---")]
        [SerializeField] private GameObject hitImpactEffect;
        [SerializeField] private LayerMask hitLayers;

        private void Start()
        {
            // Tự hủy sau thời gian nhất định nếu không va chạm để tránh rò rỉ bộ nhớ
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            // Đạn bay theo hướng vector X của chính nó
            transform.Translate(Vector2.right * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Không va chạm với người bắn (Player)
            if (collision.CompareTag("Player")) return;

            // Kiểm tra đối tượng bị bắn có thanh máu (Health) hay không
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth == null)
            {
                targetHealth = collision.GetComponentInParent<Health>();
            }

            if (targetHealth != null && collision.CompareTag("Enemy"))
            {
                // Gây sát thương lên quái vật
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                targetHealth.TakeDamage(damage, hitDirection);
                SpawnImpactEffect();
                Destroy(gameObject);
                return;
            }

            // Nếu bắn trúng chướng ngại vật / mặt đất
            if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle"))
            {
                SpawnImpactEffect();
                Destroy(gameObject);
            }
        }

        private void SpawnImpactEffect()
        {
            if (hitImpactEffect != null)
            {
                GameObject effect = Instantiate(hitImpactEffect, transform.position, Quaternion.identity);
                Destroy(effect, 0.4f);
            }
        }
    }
}
