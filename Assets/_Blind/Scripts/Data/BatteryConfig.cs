using UnityEngine;

namespace Blind
{
    [CreateAssetMenu(fileName = "BatteryConfig", menuName = "Blind/Battery Config")]
    public class BatteryConfig : ScriptableObject
    {
        [Tooltip("Seconds until battery fully drains from 100%")]
        public float fullDrainDuration = 60f;

        [Tooltip("How much battery a pickup restores (0-1, e.g. 0.3 = 30%)")]
        public float pickupRestoreAmount = 0.3f;

        [Tooltip("Seconds the grace period lasts before game over")]
        public float gracePeriodDuration = 15f;

        [Tooltip("How fast the flashlight flickers during grace period")]
        public float flickerSpeed = 8f;

        [Tooltip("Radius within which the exit light becomes visible (in metres)")]
        public float exitRevealRadius = 6f;
    }
}
