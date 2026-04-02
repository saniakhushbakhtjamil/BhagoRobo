using UnityEngine;

namespace Blind
{
    // Controls the spotlight that acts as the robot's flashlight.
    // Radius and intensity shrink as battery drops. Flickers during grace period.
    public class FlashlightController : MonoBehaviour
    {
        [SerializeField] Light spotLight;
        [SerializeField] BatteryConfig config;
        [SerializeField] BatterySystem batterySystem;

        [Tooltip("Max spotlight range at 100% battery")]
        [SerializeField] float maxRange = 12f;

        [Tooltip("Min spotlight range at 0% battery")]
        [SerializeField] float minRange = 2f;

        [Tooltip("Max spotlight intensity at 100% battery")]
        [SerializeField] float maxIntensity = 3f;

        [Tooltip("Min spotlight intensity at 0% battery")]
        [SerializeField] float minIntensity = 0.3f;

        bool isFlickering;

        void OnEnable()
        {
            batterySystem.OnBatteryChanged += HandleBatteryChanged;
            batterySystem.OnGracePeriodStarted += HandleGracePeriod;
        }

        void OnDisable()
        {
            batterySystem.OnBatteryChanged -= HandleBatteryChanged;
            batterySystem.OnGracePeriodStarted -= HandleGracePeriod;
        }

        void Update()
        {
            if (isFlickering)
                Flicker();
        }

        void HandleBatteryChanged(float percent)
        {
            spotLight.range = Mathf.Lerp(minRange, maxRange, percent);
            spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, percent);
        }

        void HandleGracePeriod()
        {
            isFlickering = true;
        }

        // Perlin noise gives organic, non-repeating flicker
        void Flicker()
        {
            float noise = Mathf.PerlinNoise(Time.time * config.flickerSpeed, 0f);
            spotLight.intensity = Mathf.Lerp(0f, minIntensity, noise);
        }
    }
}
