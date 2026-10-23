using UnityEngine;
using PixelGunner.Core;
using PixelGunner.Player;
using PixelGunner.Managers;

namespace PixelGunner.Items
{
    public enum ItemType
    {
        Coin,
        HealthPack,
        ShotgunUpgrade,
        RapidGunUpgrade
    }

    /// <summary>
    /// Xử lý vật phẩm nhặt được trên đường đi hoặc rơi ra từ quái vật.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        [Header("--- LOẠI VẬT PHẨM ---")]
        [SerializeField] private ItemType itemType = ItemType.Coin;
        [SerializeField] private int value = 10;

        [Header("--- HIỆU ỨNG NHẬT ĐƯỢC ---")]
        [SerializeField] private GameObject pickupEffect;
        [SerializeField] private AudioClip pickupSound;

        [Header("--- HIỆU ỨNG BỒNG BỀNH (FLOATING) ---")]
        [SerializeField] private float bobSpeed = 3f;
        [SerializeField] private float bobHeight = 0.15f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            // Hiệu ứng nhấp nhô nhẹ tạo điểm nhấn thị giác
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                ApplyItemEffect(collision.gameObject);

                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }

        private void ApplyItemEffect(GameObject player)
        {
            switch (itemType)
            {
                case ItemType.Coin:
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddCoin(value);
                    }
                    break;

                case ItemType.HealthPack:
                    Health health = player.GetComponent<Health>();
                    if (health != null)
                    {
                        health.Heal(value);
                    }
                    break;

                case ItemType.ShotgunUpgrade:
                    GunController shotgun = player.GetComponentInChildren<GunController>();
                    if (shotgun != null)
                    {
                        shotgun.SetWeapon(WeaponType.ShotgunSpread, 0.45f);
                    }
                    break;

                case ItemType.RapidGunUpgrade:
                    GunController rapid = player.GetComponentInChildren<GunController>();
                    if (rapid != null)
                    {
                        rapid.SetWeapon(WeaponType.RapidMachineGun, 0.12f);
                    }
                    break;
            }
        }
    }
}
