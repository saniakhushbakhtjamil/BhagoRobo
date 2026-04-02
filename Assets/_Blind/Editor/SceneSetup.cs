using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Blind
{
    // Run this once to build the entire game scene.
    // Menu: Blind > Setup Scene
    public static class SceneSetup
    {
        [MenuItem("Blind/Setup Scene")]
        static void Run()
        {
            BatteryConfig config = AssetDatabase.LoadAssetAtPath<BatteryConfig>(
                "Assets/_Blind/Data/BatteryConfig.asset");

            if (config == null)
                Debug.LogWarning("[SceneSetup] BatteryConfig asset not found. " +
                    "Create it via: right-click Assets/_Blind/Data > Create > Blind > Battery Config, " +
                    "then re-run Setup Scene. Continuing without it — assign it manually in the Inspector.");

            ClearScene();
            SetupLighting();
            var room       = CreateRoom();
            var player     = CreatePlayer(config);
            var cam        = CreateCamera(player.transform);
            CreateBatteryPickups(config, 5);
            CreateExit(config);
            CreateUI(player, config);

            // Save the scene so nothing is lost
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            Debug.Log("[SceneSetup] Scene built. Press Ctrl+S to save.");
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        static void ClearScene()
        {
            // Destroy root objects only — Unity auto-destroys children with the parent
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name == "Directional Light") continue;
                Object.DestroyImmediate(go);
            }
        }

        static void SetupLighting()
        {
            // Flat ambient — dark but not pitch black so walls are barely visible
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.06f, 0.06f, 0.08f);

            // Dim the default directional light (or create one if missing)
            var dirLight = GameObject.Find("Directional Light");
            if (dirLight == null)
            {
                dirLight = new GameObject("Directional Light");
                var l = dirLight.AddComponent<Light>();
                l.type = LightType.Directional;
            }
            dirLight.GetComponent<Light>().intensity = 0.05f;
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Ensure URP renders spot/point lights per-pixel via SerializedObject
            var urpAsset = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline
                as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
            if (urpAsset != null)
            {
                var so = new SerializedObject(urpAsset);
                var modeProp = so.FindProperty("m_AdditionalLightsRenderingMode");
                if (modeProp != null) modeProp.intValue = 1; // 1 = PerPixel
                var limitProp = so.FindProperty("m_AdditionalLightsPerObjectLimit");
                if (limitProp != null) limitProp.intValue = 8;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(urpAsset);
                Debug.Log("[SceneSetup] URP additional lights set to Per Pixel.");
            }
            else
            {
                Debug.LogWarning("[SceneSetup] Could not find URP asset — lights may not render.");
            }
        }

        static GameObject CreateRoom()
        {
            var room = new GameObject("Room");

            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.parent = room.transform;
            floor.transform.localScale = new Vector3(3f, 1f, 3f); // 30x30 metres
            SetColor(floor, new Color(0.15f, 0.15f, 0.18f));

            // Walls — North, South, East, West
            CreateWall(room.transform, "Wall_North", new Vector3(0f, 2f, 15f),  new Vector3(30f, 4f, 0.5f));
            CreateWall(room.transform, "Wall_South", new Vector3(0f, 2f, -15f), new Vector3(30f, 4f, 0.5f));
            CreateWall(room.transform, "Wall_East",  new Vector3(15f, 2f, 0f),  new Vector3(0.5f, 4f, 30f));
            CreateWall(room.transform, "Wall_West",  new Vector3(-15f, 2f, 0f), new Vector3(0.5f, 4f, 30f));

            return room;
        }

        static void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.parent = parent;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            SetColor(wall, new Color(0.12f, 0.12f, 0.15f));
        }

        static GameObject CreatePlayer(BatteryConfig config)
        {
            // Body
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            SetColor(player, new Color(0.8f, 0.85f, 1f)); // pale blue robot

            // Rigidbody — physics-driven movement, no tipping
            var rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ
                           | RigidbodyConstraints.FreezePositionY;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Scripts
            player.AddComponent<PlayerMovement>();
            var battery = player.AddComponent<BatterySystem>();

            // Flashlight — child of player, at head height, pointing forward
            var flashlightGO = new GameObject("Flashlight");
            flashlightGO.transform.parent = player.transform;
            flashlightGO.transform.localPosition = new Vector3(0f, 0.5f, 0.3f);
            flashlightGO.transform.localRotation = Quaternion.Euler(20f, 0f, 0f); // tilt down to hit floor

            var spotLight = flashlightGO.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.spotAngle = 60f;
            spotLight.range = 12f;
            spotLight.intensity = 3f;
            spotLight.color = new Color(1f, 0.97f, 0.9f);

            var flashCtrl = player.AddComponent<FlashlightController>();

            // Wire up serialized fields via SerializedObject
            var so = new SerializedObject(flashCtrl);
            so.FindProperty("spotLight").objectReferenceValue = spotLight;
            so.FindProperty("batterySystem").objectReferenceValue = battery;
            if (config != null) so.FindProperty("config").objectReferenceValue = config;
            so.ApplyModifiedProperties();

            var battSO = new SerializedObject(battery);
            if (config != null) battSO.FindProperty("config").objectReferenceValue = config;
            battSO.ApplyModifiedProperties();

            return player;
        }

        static GameObject CreateCamera(Transform playerTransform)
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;

            // Position: above and behind the player
            camGO.transform.position = new Vector3(0f, 8f, -9f);
            camGO.transform.rotation = Quaternion.Euler(42f, 0f, 0f); // ~42° pitch — see ahead of robot

            var follow = camGO.AddComponent<CameraFollow>();
            var so = new SerializedObject(follow);
            so.FindProperty("target").objectReferenceValue = playerTransform;
            so.ApplyModifiedProperties();

            // Audio listener moves with camera
            camGO.AddComponent<AudioListener>();

            return camGO;
        }

        static void CreateBatteryPickups(BatteryConfig config, int count)
        {
            var pickupsRoot = new GameObject("Pickups");

            // Random positions inside the room, avoiding the centre (player spawn)
            var rng = new System.Random(42); // fixed seed so layout is consistent each setup
            for (int i = 0; i < count; i++)
            {
                float x = (float)(rng.NextDouble() * 20 - 10);
                float z = (float)(rng.NextDouble() * 20 - 10);

                // Keep away from player spawn at (0,0,0)
                if (Mathf.Abs(x) < 3f && Mathf.Abs(z) < 3f) x += 5f;

                var pickup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pickup.name = $"BatteryPickup_{i + 1}";
                pickup.transform.parent = pickupsRoot.transform;
                pickup.transform.position = new Vector3(x, 0.5f, z);
                pickup.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                SetColor(pickup, new Color(1f, 0.85f, 0.1f)); // yellow

                // Trigger collider
                var col = pickup.GetComponent<Collider>();
                col.isTrigger = true;

                var bp = pickup.AddComponent<BatteryPickup>();
                if (config != null)
                {
                    var so = new SerializedObject(bp);
                    so.FindProperty("config").objectReferenceValue = config;
                    so.ApplyModifiedProperties();
                }

                // Small glow so the pickup is barely visible in the dark
                var glow = new GameObject("Glow");
                glow.transform.parent = pickup.transform;
                glow.transform.localPosition = Vector3.zero;
                var glowLight = glow.AddComponent<Light>();
                glowLight.type = LightType.Point;
                glowLight.range = 1.5f;
                glowLight.intensity = 0.4f;
                glowLight.color = new Color(1f, 0.9f, 0.2f);
            }
        }

        static void CreateExit(BatteryConfig config)
        {
            var exit = new GameObject("Exit");
            exit.transform.position = new Vector3(10f, 0.01f, 10f); // far corner

            // Visual marker — a flat glowing disc
            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "ExitMarker";
            disc.transform.parent = exit.transform;
            disc.transform.localPosition = Vector3.zero;
            disc.transform.localScale = new Vector3(1.5f, 0.05f, 1.5f);
            SetColor(disc, new Color(0.2f, 1f, 0.4f));
            Object.DestroyImmediate(disc.GetComponent<Collider>()); // no collision on visual

            // Trigger collider on the root
            var triggerCol = exit.AddComponent<CapsuleCollider>();
            triggerCol.isTrigger = true;
            triggerCol.radius = 1.2f;
            triggerCol.height = 2f;
            triggerCol.center = new Vector3(0f, 1f, 0f);

            // Exit reveal light — starts invisible, fades in when player is near
            var exitLight = new GameObject("ExitLight");
            exitLight.transform.parent = exit.transform;
            exitLight.transform.localPosition = new Vector3(0f, 1f, 0f);
            var light = exitLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 5f;
            light.intensity = 0f; // starts off
            light.color = new Color(0.3f, 1f, 0.5f);

            var trigger = exit.AddComponent<ExitTrigger>();
            var so = new SerializedObject(trigger);
            so.FindProperty("exitLight").objectReferenceValue = light;
            if (config != null) so.FindProperty("config").objectReferenceValue = config;
            so.ApplyModifiedProperties();
        }

        static void CreateUI(GameObject player, BatteryConfig config)
        {
            // Canvas
            var canvasGO = new GameObject("HUD Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Battery bar background
            var barBG = CreateUIPanel(canvasGO.transform, "BatteryBarBG",
                new Vector2(200f, 20f), new Vector2(-120f, -30f), new Color(0.1f, 0.1f, 0.1f, 0.8f),
                TextAnchor.UpperRight);

            // Battery bar fill — pivot at left edge so it shrinks right-to-left
            var barFill = CreateUIPanel(barBG.transform, "BatteryBarFill",
                new Vector2(200f, 20f), Vector2.zero, Color.green,
                TextAnchor.UpperLeft);
            var fillRect = barFill.GetComponent<RectTransform>();
            fillRect.pivot = new Vector2(0f, 0.5f);
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(0f, 1f);
            fillRect.anchoredPosition = Vector2.zero;

            // Game Over panel
            var gameOverPanel = CreateUIPanel(canvasGO.transform, "GameOverPanel",
                new Vector2(400f, 150f), Vector2.zero, new Color(0f, 0f, 0f, 0.85f),
                TextAnchor.MiddleCenter);
            AddLabel(gameOverPanel.transform, "The spaceship never reaches Jupiter.", 22);
            gameOverPanel.SetActive(false);

            // Win panel
            var winPanel = CreateUIPanel(canvasGO.transform, "WinPanel",
                new Vector2(400f, 150f), Vector2.zero, new Color(0f, 0f, 0f, 0.85f),
                TextAnchor.MiddleCenter);
            AddLabel(winPanel.transform, "You escaped!", 28);
            winPanel.SetActive(false);

            // GameManager
            var gmGO = new GameObject("GameManager");
            gmGO.AddComponent<GameManager>();

            // HUDController
            var hud = canvasGO.AddComponent<HUDController>();
            var battery = player.GetComponent<BatterySystem>();
            var hudSO = new SerializedObject(hud);
            hudSO.FindProperty("batterySystem").objectReferenceValue = battery;
            hudSO.FindProperty("batteryBarFill").objectReferenceValue = barFill.GetComponent<RectTransform>();
            hudSO.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
            hudSO.FindProperty("winPanel").objectReferenceValue = winPanel;
            hudSO.ApplyModifiedProperties();
        }

        // ─── UI helpers ──────────────────────────────────────────────────────────

        static GameObject CreateUIPanel(Transform parent, string name, Vector2 size,
            Vector2 anchoredPos, Color color, TextAnchor anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;

            // Anchor based on usage
            if (anchor == TextAnchor.UpperRight)
            {
                rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
            }
            else if (anchor == TextAnchor.MiddleCenter)
            {
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
            }

            return go;
        }

        static void AddLabel(Transform parent, string text, int fontSize)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        static void SetColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            renderer.sharedMaterial = mat;
        }
    }
}
