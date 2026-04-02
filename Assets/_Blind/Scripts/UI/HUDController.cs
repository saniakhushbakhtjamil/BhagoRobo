using UnityEngine;
using UnityEngine.UI;

namespace Blind
{
    // Updates the battery bar fill and color based on battery level.
    public class HUDController : MonoBehaviour
    {
        [SerializeField] BatterySystem batterySystem;

        [Tooltip("The RectTransform of the battery bar fill panel")]
        [SerializeField] RectTransform batteryBarFill;

        [Tooltip("Color at 100% battery")]
        [SerializeField] Color fullColor = Color.green;

        [Tooltip("Color at 0% battery")]
        [SerializeField] Color emptyColor = Color.red;

        [Tooltip("Game Over panel to show when the robot shuts down")]
        [SerializeField] GameObject gameOverPanel;

        [Tooltip("Win panel to show when the player reaches the exit")]
        [SerializeField] GameObject winPanel;

        float maxBarWidth;

        void OnEnable()
        {
            batterySystem.OnBatteryChanged += UpdateBar;
        }

        void OnDisable()
        {
            batterySystem.OnBatteryChanged -= UpdateBar;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver -= ShowGameOver;
                GameManager.Instance.OnWin -= ShowWin;
            }
        }

        void Start()
        {
            // GameManager.Instance is guaranteed to exist by Start (after all Awake calls)
            GameManager.Instance.OnGameOver += ShowGameOver;
            GameManager.Instance.OnWin += ShowWin;

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);

            // Store the full width so we can scale it down as battery drains
            if (batteryBarFill != null)
                maxBarWidth = batteryBarFill.sizeDelta.x;
        }

        void UpdateBar(float percent)
        {
            if (batteryBarFill == null) return;
            var size = batteryBarFill.sizeDelta;
            size.x = maxBarWidth * percent;
            batteryBarFill.sizeDelta = size;
            batteryBarFill.GetComponent<Image>().color = Color.Lerp(emptyColor, fullColor, percent);
        }

        void ShowGameOver()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }

        void ShowWin()
        {
            if (winPanel != null) winPanel.SetActive(true);
        }
    }
}
