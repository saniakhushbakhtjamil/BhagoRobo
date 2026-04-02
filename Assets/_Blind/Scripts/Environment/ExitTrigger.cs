using UnityEngine;

namespace Blind
{
    // The exit is invisible until the player is within reveal radius.
    // A dim point light fades in to guide the player once they're close enough.
    public class ExitTrigger : MonoBehaviour
    {
        [SerializeField] BatteryConfig config;

        [Tooltip("The dim light that reveals the exit when player is nearby")]
        [SerializeField] Light exitLight;

        [Tooltip("Max intensity of the exit light when player is right on top of it")]
        [SerializeField] float maxExitLightIntensity = 1.5f;

        Transform player;

        void Start()
        {
            if (exitLight != null)
                exitLight.intensity = 0f;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        void Update()
        {
            if (player == null || exitLight == null) return;

            float distance = Vector3.Distance(transform.position, player.position);
            float revealRadius = config.exitRevealRadius;

            // Fade the exit light in as the player approaches
            float t = 1f - Mathf.Clamp01(distance / revealRadius);
            exitLight.intensity = Mathf.Lerp(0f, maxExitLightIntensity, t);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            GameManager.Instance.TriggerWin();
        }
    }
}
