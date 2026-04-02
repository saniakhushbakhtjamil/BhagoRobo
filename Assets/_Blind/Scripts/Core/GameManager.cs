using UnityEngine;

namespace Blind
{
    public enum GameState { Playing, GracePeriod, GameOver, Win }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.Playing;

        // Other systems subscribe to these
        public event System.Action OnGameOver;
        public event System.Action OnWin;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void TriggerGracePeriod()
        {
            if (State != GameState.Playing) return;
            State = GameState.GracePeriod;
        }

        public void TriggerPlaying()
        {
            if (State != GameState.GracePeriod) return;
            State = GameState.Playing;
        }

        public void TriggerGameOver()
        {
            if (State == GameState.GameOver || State == GameState.Win) return;
            State = GameState.GameOver;
            OnGameOver?.Invoke();
            Debug.Log("Game Over — the spaceship never reaches Jupiter.");
        }

        public void TriggerWin()
        {
            if (State == GameState.GameOver || State == GameState.Win) return;
            State = GameState.Win;
            OnWin?.Invoke();
            Debug.Log("You escaped!");
        }

        public bool IsPlaying() => State == GameState.Playing || State == GameState.GracePeriod;
    }
}
