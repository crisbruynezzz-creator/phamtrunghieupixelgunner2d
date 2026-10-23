using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PixelGunner.UI
{
    /// <summary>
    /// Quản lý toàn bộ giao diện người dùng (HUD Canvas, Game Over, Victory, Thanh máu, Điểm số).
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("--- THANH MÁU & CHỈ SỐ PLAYER ---")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("--- CÁC BẢNG ĐIỀU KHIỂN (PANELS) ---")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject pausePanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        /// <summary>
        /// Cập nhật hiển thị thanh máu Player.
        /// </summary>
        public void UpdatePlayerHealth(int currentHealth, int maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{currentHealth} / {maxHealth}";
            }
        }

        /// <summary>
        /// Cập nhật hiển thị số tiền vàng nhặt được.
        /// </summary>
        public void UpdateCoinCount(int coins)
        {
            if (coinText != null)
            {
                coinText.text = $"Coins: {coins}";
            }
        }

        /// <summary>
        /// Cập nhật hiển thị điểm số (Score).
        /// </summary>
        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        /// <summary>
        /// Hiển thị bảng Game Over khi người chơi hết máu.
        /// </summary>
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hiển thị bảng Chiến thắng khi hoàn thành màn chơi.
        /// </summary>
        public void ShowVictory()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Bật/Tắt bảng Pause Game.
        /// </summary>
        public void TogglePauseMenu(bool isPaused)
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(isPaused);
            }
        }
    }
}
