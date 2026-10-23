using UnityEngine;

namespace PixelGunner.Core
{
    /// <summary>
    /// Gắn vào chông gai, bẫy hoặc đòn tấn công để gây sát thương liên tục hoặc tức thì khi va chạm.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        [Header("--- THIẾT LẬP SÁT THƯƠNG ---")]
        [SerializeField] private int damageAmount = 20;
        [SerializeField] private bool destroyOnHit = false;
        [SerializeField] private string targetTag = "Player";

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(targetTag))
            {
                Health targetHealth = collision.GetComponent<Health>();
                if (targetHealth == null)
                {
                    targetHealth = collision.GetComponentInParent<Health>();
                }

                if (targetHealth != null)
                {
                    Vector2 forceDir = (collision.transform.position - transform.position).normalized;
                    targetHealth.TakeDamage(damageAmount, forceDir);

                    if (destroyOnHit)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
