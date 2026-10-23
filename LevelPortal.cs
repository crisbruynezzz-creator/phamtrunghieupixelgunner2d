using UnityEngine;
using PixelGunner.Managers;

namespace PixelGunner.Core
{
    /// <summary>
    /// Điểm đích / Cánh cổng dịch chuyển hoàn thành màn chơi khi người chơi chạm tới.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LevelPortal : MonoBehaviour
    {
        [Header("--- HIỆU ỨNG CHIẾN THẮNG ---")]
        [SerializeField] private GameObject victoryEffect;
        [SerializeField] private AudioClip victorySound;

        private bool isTriggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isTriggered) return;

            if (collision.CompareTag("Player"))
            {
                isTriggered = true;

                if (victoryEffect != null)
                {
                    Instantiate(victoryEffect, transform.position, Quaternion.identity);
                }

                if (victorySound != null)
                {
                    AudioSource.PlayClipAtPoint(victorySound, transform.position);
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Victory();
                }
            }
        }
    }
}
