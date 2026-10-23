using UnityEngine;

namespace PixelGunner.CameraSystem
{
    /// <summary>
    /// Camera bám theo người chơi mượt mà trong không gian 2D và giới hạn theo biên bản đồ.
    /// </summary>
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("--- MỤC TIÊU BÁM THEO ---")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -10f);
        [SerializeField] private float smoothTime = 0.2f;

        [Header("--- GIỚI HẠN BIÊN BẢN ĐỒ (BOUNDS) ---")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 100f;
        [SerializeField] private float minY = -2f;
        [SerializeField] private float maxY = 20f;

        private Vector3 currentVelocity = Vector3.zero;

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 targetPosition = target.position + offset;

            if (useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            }

            // Đảm bảo trục Z của Camera luôn giữ nguyên
            targetPosition.z = offset.z;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
    }
}
