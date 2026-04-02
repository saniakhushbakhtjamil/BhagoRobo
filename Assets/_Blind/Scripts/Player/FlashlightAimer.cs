using UnityEngine;
using UnityEngine.InputSystem;

namespace Blind
{
    // Rotates the flashlight toward the mouse cursor, clamped to ±90° in front of the robot.
    // This means the flashlight can sweep the full front hemisphere but never point behind.
    public class FlashlightAimer : MonoBehaviour
    {
        [Tooltip("The turret transform (small cube on top) that spins with the mouse")]
        [SerializeField] Transform turret;

        Camera mainCamera;

        void Awake()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (!GameManager.Instance.IsPlaying()) return;
            AimTurret();
        }

        void AimTurret()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane floor = new Plane(Vector3.up, Vector3.zero);

            if (!floor.Raycast(ray, out float distance)) return;

            Vector3 mouseWorld = ray.GetPoint(distance);
            Vector3 toMouse = (mouseWorld - transform.position);
            toMouse.y = 0f;

            if (toMouse.sqrMagnitude < 0.01f) return;

            toMouse.Normalize();

            // Clamp to ±90° of robot body forward — turret can't face behind
            float angle = Vector3.SignedAngle(transform.forward, toMouse, Vector3.up);
            angle = Mathf.Clamp(angle, -90f, 90f);

            // Rotate turret on Y axis only in local space
            turret.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}
