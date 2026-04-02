using UnityEngine;

namespace Blind
{
    // Owns the battery percentage and drives the grace period countdown.
    // Fires events so other scripts (flashlight, HUD, GameManager) can react.
    public class BatterySystem : MonoBehaviour
    {
        [SerializeField] BatteryConfig config;

        public float BatteryPercent { get; private set; } = 1f; // 1 = 100%

        // Fired every frame with current battery 0-1
        public event System.Action<float> OnBatteryChanged;
        // Fired once when battery hits 0
        public event System.Action OnGracePeriodStarted;

        float graceTimer;
        bool inGracePeriod;

        void Start()
        {
            if (config == null)
                Debug.LogError("[BatterySystem] BatteryConfig not assigned! " +
                    "Select the Player in the Hierarchy and drag Assets/_Blind/Data/BatteryConfig into the Config field.");
        }

        void Update()
        {
            if (config == null) return;
            if (!GameManager.Instance.IsPlaying()) return;

            if (!inGracePeriod)
            {
                BatteryPercent -= Time.deltaTime / config.fullDrainDuration;
                BatteryPercent = Mathf.Clamp01(BatteryPercent);
                OnBatteryChanged?.Invoke(BatteryPercent);

                if (BatteryPercent <= 0f)
                {
                    inGracePeriod = true;
                    graceTimer = config.gracePeriodDuration;
                    GameManager.Instance.TriggerGracePeriod();
                    OnGracePeriodStarted?.Invoke();
                }
            }
            else
            {
                graceTimer -= Time.deltaTime;
                if (graceTimer <= 0f)
                    GameManager.Instance.TriggerGameOver();
            }
        }

        // Called by BatteryPickup
        public void Recharge(float amount)
        {
            if (inGracePeriod) return; // pickup can't save you in grace period
            BatteryPercent = Mathf.Clamp01(BatteryPercent + amount);
            OnBatteryChanged?.Invoke(BatteryPercent);
        }
    }
}
