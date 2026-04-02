using UnityEngine;

namespace Blind
{
    // Recharges the player's battery on contact, then disappears.
    public class BatteryPickup : MonoBehaviour
    {
        [SerializeField] BatteryConfig config;

        void OnTriggerEnter(Collider other)
        {
            BatterySystem battery = other.GetComponent<BatterySystem>();
            if (battery == null) return;

            battery.Recharge(config.pickupRestoreAmount);
            Destroy(gameObject);
        }
    }
}
