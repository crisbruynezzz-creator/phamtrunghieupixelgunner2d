using UnityEngine;
using UnityEngine.SceneManagement;
using PixelGunner.UI;

namespace PixelGunner.Managers
{
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    /// <summary>
    /// Quản lý trạng thái vòng đời game, điểm số, nạp màn chơi (Scene Management) và Singleton.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("--- THỐNG KÊ MÀN CHƠI ---")]
        [SerializeField] private int currentScore = 0;
        [SerializeField] private int currentCoins = 0;

        private GameState currentState = GameState.Playing;
        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Time.timeScale = 1f; // Khôi phục thời gian khi nạp lại scene
        }

        private void Start()
        {
            UpdateUI();
        }

        private void Update()
        {
            // Nhấn phím Escape để tạm dừng game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void AddScore(int points)
        {
            currentScore += points;
            UpdateUI();
        }

        public void AddCoin(int amount)
        {
            currentCoins += amount;
            AddScore(amount * 10); // Mỗi đồng xu cộng thêm điểm
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScore(currentScore);
                UIManager.Instance.UpdateCoinCount(currentCoins);
            }
        }

        public void GameOver()
        {
            if (currentState == GameState.GameOver) return;
            currentState = GameState.GameOver;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
        }

        public void Victory()
        {
            if (currentState == GameState.Victory) return;
            currentState = GameState.Victory;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowVictory();
            }
        }

        public void TogglePause()
        {
            if (currentState == GameState.GameOver || currentState == GameState.Victory) return;

            if (currentState == GameState.Playing)
            {
                currentState = GameState.Paused;
                Time.timeScale = 0f;
                if (UIManager.Instance != null) UIManager.Instance.TogglePauseMenu(true);
            }
            else if (currentState == GameState.Paused)
            {
                currentState = GameState.Playing;
                Time.timeScale = 1f;
                if (UIManager.Instance != null) UIManager.Instance.TogglePauseMenu(false);
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                // Nếu là màn cuối thì tải lại menu hoặc màn 0
                SceneManager.LoadScene(0);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
