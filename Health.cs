using System;
using System.Collections;
using UnityEngine;
using PixelGunner.Player;
using PixelGunner.Managers;
using PixelGunner.UI;

namespace PixelGunner.Core
{
    /// <summary>
    /// Hệ thống quản lý điểm sinh lực (HP), nhận sát thương, hồi máu và xử lý khi chết.
    /// Dùng chung cho cả Player và Quái vật.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Header("--- ĐIỂM SINH LỰC ---")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        [Header("--- THỜI GIAN BẤT TỬ SAU KHI DÍNH ĐÒN ---")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;
        [SerializeField] private bool isInvulnerable = false;

        [Header("--- HIỆU ỨNG KHI BỊ SÁT THƯƠNG & CHẾT ---")]
        [SerializeField] private GameObject deathEffect;
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public event Action<int, int> OnHealthChanged; // (current, max)
        public event Action OnDeath;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0;

        private AudioSource audioSource;

        private void Awake()
        {
            currentHealth = maxHealth;
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Gây sát thương lên đối tượng.
        /// </summary>
        public void TakeDamage(int damage, Vector2 damageSourceDirection = default)
        {
            if (IsDead || isInvulnerable) return;

            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Âm thanh bị thương
            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }

            // Xử lý giật lùi nếu là Player
            if (CompareTag("Player"))
            {
                PlayerController player = GetComponent<PlayerController>();
                if (player != null && damageSourceDirection != default)
                {
                    player.ApplyKnockback(damageSourceDirection * 5f);
                }

                // Cập nhật thanh máu trên UIManager
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
                }

                StartCoroutine(InvulnerabilityRoutine());
            }

            // Chớp đỏ cảnh báo
            StartCoroutine(FlashDamageRoutine());

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Hồi máu cho đối tượng (nhặt túi cứu thương).
        /// </summary>
        public void Heal(int healAmount)
        {
            if (IsDead) return;

            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (CompareTag("Player") && UIManager.Instance != null)
            {
                UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
            }
        }

        private void Die()
        {
            OnDeath?.Invoke();

            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            if (CompareTag("Player"))
            {
                // Thông báo Game Over cho GameManager
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
                }
                gameObject.SetActive(false);
            }
            else
            {
                // Quái vật chết: Cộng điểm và hủy đối tượng
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(50);
                }
                Destroy(gameObject);
            }
        }

        private IEnumerator InvulnerabilityRoutine()
        {
            isInvulnerable = true;
            yield return new WaitForSeconds(invulnerabilityDuration);
            isInvulnerable = false;
        }

        private IEnumerator FlashDamageRoutine()
        {
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.12f);
                spriteRenderer.color = originalColor;
            }
        }
    }
}
