using UnityEngine;

namespace PixelGunner.Player
{
    public enum WeaponType
    {
        PistolSingle,
        ShotgunSpread,
        RapidMachineGun
    }

    /// <summary>
    /// Điều khiển việc xoay nòng súng theo con trỏ chuột và bắn đạn.
    /// </summary>
    public class GunController : MonoBehaviour
    {
        [Header("--- THIẾT LẬP VŨ KHÍ ---")]
        [SerializeField] private WeaponType currentWeapon = WeaponType.PistolSingle;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private float fireRate = 0.25f;

        [Header("--- HIỆU ỨNG ÂM THANH & HÌNH ẢNH ---")]
        [SerializeField] private GameObject muzzleFlashEffect;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioSource audioSource;

        private float nextFireTime = 0f;
        private Camera mainCamera;
        private SpriteRenderer gunSpriteRenderer;

        public WeaponType CurrentWeapon => currentWeapon;

        private void Awake()
        {
            mainCamera = Camera.main;
            gunSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            HandleAiming();
            HandleShooting();
        }

        /// <summary>
        /// Xoay nòng súng theo hướng chuột.
        /// </summary>
        private void HandleAiming()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            Vector2 aimDirection = (mouseWorldPos - transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Lật súng theo trục Y khi ngắm sang trái để không bị lộn ngược hình
            if (gunSpriteRenderer != null)
            {
                if (angle > 90f || angle < -90f)
                {
                    gunSpriteRenderer.flipY = true;
                }
                else
                {
                    gunSpriteRenderer.flipY = false;
                }
            }
        }

        /// <summary>
        /// Kiểm tra điều kiện và thực hiện bắn đạn.
        /// </summary>
        private void HandleShooting()
        {
            bool isPressingFire = (currentWeapon == WeaponType.RapidMachineGun) 
                ? Input.GetButton("Fire1") 
                : Input.GetButtonDown("Fire1");

            // Có thể nhấn phím J hoặc K để bắn tiện cho bàn phím
            if (Input.GetKeyDown(KeyCode.J)) isPressingFire = true;

            if (isPressingFire && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Shoot()
        {
            if (bulletPrefab == null || muzzlePoint == null) return;

            switch (currentWeapon)
            {
                case WeaponType.PistolSingle:
                    SpawnBullet(muzzlePoint.rotation);
                    break;

                case WeaponType.ShotgunSpread:
                    // Bắn 3 viên đạn tỏa quạt (-15 độ, 0 độ, +15 độ)
                    SpawnBullet(muzzlePoint.rotation * Quaternion.Euler(0, 0, -15f));
                    SpawnBullet(muzzlePoint.rotation);
                    SpawnBullet(muzzlePoint.rotation * Quaternion.Euler(0, 0, 15f));
                    break;

                case WeaponType.RapidMachineGun:
                    // Thêm chút độ lệch (spread) ngẫu nhiên
                    float randomOffset = Random.Range(-4f, 4f);
                    SpawnBullet(muzzlePoint.rotation * Quaternion.Euler(0, 0, randomOffset));
                    break;
            }

            // Kích hoạt hiệu ứng tóe lửa đầu nòng
            if (muzzleFlashEffect != null)
            {
                GameObject flash = Instantiate(muzzleFlashEffect, muzzlePoint.position, muzzlePoint.rotation);
                Destroy(flash, 0.08f);
            }

            // Âm thanh
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }

        private void SpawnBullet(Quaternion rotation)
        {
            Instantiate(bulletPrefab, muzzlePoint.position, rotation);
        }

        /// <summary>
        /// Nâng cấp hoặc chuyển đổi loại vũ khí khi nhặt vật phẩm.
        /// </summary>
        public void SetWeapon(WeaponType newWeapon, float customFireRate = 0.2f)
        {
            currentWeapon = newWeapon;
            fireRate = customFireRate;
        }
    }
}
