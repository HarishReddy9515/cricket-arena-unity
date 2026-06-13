#if UNITY_EDITOR
using CricketArena.Core;
using CricketArena.Gameplay;
using CricketArena.Presentation;
using CricketArena.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CricketArena.EditorTools
{
    public static class ArenaSceneBuilder
    {
        [MenuItem("Cricket Arena/Build Playable Prototype Scene")]
        public static void BuildScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "ArenaPrototype";

            Material grass = Material("Arena Grass", new Color(0.08f, 0.42f, 0.19f));
            Material pitch = Material("Pitch Clay", new Color(0.63f, 0.42f, 0.24f));
            Material blue = Material("Harish XI Blue", new Color(0.03f, 0.32f, 0.75f));
            Material red = Material("Opponent Red", new Color(0.74f, 0.16f, 0.15f));
            Material white = Material("Kit White", new Color(0.92f, 0.93f, 0.94f));
            Material ballMat = Material("Cricket Ball Red", new Color(0.78f, 0.05f, 0.05f));

            CreateLighting();
            CreateGround(grass, pitch);
            CreateStadium();

            GameObject game = new GameObject("Game");
            var match = game.AddComponent<MatchManager>();
            var haptics = game.AddComponent<MobileHaptics>();
            var replayRecorder = game.AddComponent<ReplayRecorder>();
            var impactVfx = game.AddComponent<ImpactVfxController>();
            var batting = game.AddComponent<BattingController>();
            var bowling = game.AddComponent<BowlingController>();
            var cameraDirector = game.AddComponent<CameraDirector>();

            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Ball";
            ball.transform.position = new Vector3(0, 0.45f, -18f);
            ball.transform.localScale = Vector3.one * 0.42f;
            ball.GetComponent<Renderer>().sharedMaterial = ballMat;
            Rigidbody body = ball.AddComponent<Rigidbody>();
            body.mass = 0.16f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            var ballPhysics = ball.AddComponent<BallPhysicsController>();

            GameObject batter = CreatePlayer("Batter", blue, white, new Vector3(0, 0, 21.5f));
            GameObject bowler = CreatePlayer("Bowler", red, white, new Vector3(0, 0, -22f));
            bowler.transform.rotation = Quaternion.Euler(0, 180, 0);

            GameObject contact = new GameObject("ContactPoint");
            contact.transform.position = new Vector3(0, 0.85f, 20.7f);

            GameObject release = new GameObject("ReleasePoint");
            release.transform.position = new Vector3(0, 1.55f, -20.5f);

            GameObject target = new GameObject("TargetPoint");
            target.transform.position = new Vector3(0, 0.52f, 20.8f);

            GameObject battingCam = CreateCameraRig("BattingCameraRig", new Vector3(0, 8.5f, 36f), new Vector3(15f, 180f, 0));
            GameObject replayCam = CreateCameraRig("ReplayCameraRig", new Vector3(-14f, 8f, 12f), new Vector3(18f, 130f, 0));

            Camera mainCamera = new GameObject("Main Camera").AddComponent<Camera>();
            mainCamera.transform.SetPositionAndRotation(battingCam.transform.position, battingCam.transform.rotation);
            mainCamera.gameObject.tag = "MainCamera";
            mainCamera.gameObject.AddComponent<AudioListener>();

            GameObject ui = CreateHud(match, batting, bowling);

            SerializedObject battingObj = new SerializedObject(batting);
            SetObject(battingObj, "matchManager", match);
            SetObject(battingObj, "ballPhysics", ballPhysics);
            SetObject(battingObj, "contactPoint", contact.transform);
            SetObject(battingObj, "haptics", haptics);
            SetObject(battingObj, "impactVfx", impactVfx);
            SetObject(battingObj, "replayRecorder", replayRecorder);
            battingObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject bowlingObj = new SerializedObject(bowling);
            SetObject(bowlingObj, "matchManager", match);
            SetObject(bowlingObj, "ballPhysics", ballPhysics);
            SetObject(bowlingObj, "releasePoint", release.transform);
            SetObject(bowlingObj, "targetPoint", target.transform);
            SetDeliveries(bowlingObj);
            bowlingObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject ballObj = new SerializedObject(ballPhysics);
            SetObject(ballObj, "body", body);
            SetObject(ballObj, "contactPoint", contact.transform);
            ballObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject cameraObj = new SerializedObject(cameraDirector);
            SetObject(cameraObj, "mainCamera", mainCamera);
            SetObject(cameraObj, "battingCamera", battingCam.transform);
            SetObject(cameraObj, "replayCamera", replayCam.transform);
            cameraObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject replayObj = new SerializedObject(replayRecorder);
            SetObject(replayObj, "cameraDirector", cameraDirector);
            SetObject(replayObj, "ball", ball.transform);
            replayObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject vfxObj = new SerializedObject(impactVfx);
            SetObject(vfxObj, "cameraDirector", cameraDirector);
            vfxObj.ApplyModifiedPropertiesWithoutUndo();

            Selection.activeObject = game;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorUtility.DisplayDialog("Cricket Arena", "Playable prototype scene generated. Save it as Assets/Scenes/ArenaPrototype.unity.", "OK");
        }

        private static void CreateLighting()
        {
            RenderSettings.ambientLight = new Color(0.18f, 0.2f, 0.24f);

            GameObject sun = new GameObject("Key Stadium Light");
            Light key = sun.AddComponent<Light>();
            key.type = LightType.Directional;
            key.intensity = 2.6f;
            key.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            for (int i = 0; i < 4; i++)
            {
                GameObject tower = new GameObject($"Floodlight {i + 1}");
                Light light = tower.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 95f;
                light.intensity = 8500f;
                tower.transform.position = new Vector3(i < 2 ? -42f : 42f, 28f, i % 2 == 0 ? -36f : 36f);
            }
        }

        private static void CreateGround(Material grass, Material pitch)
        {
            GameObject field = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            field.name = "Oval Field";
            field.transform.localScale = new Vector3(54f, 0.12f, 54f);
            field.GetComponent<Renderer>().sharedMaterial = grass;

            GameObject strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strip.name = "Pitch";
            strip.transform.position = new Vector3(0, 0.08f, 0);
            strip.transform.localScale = new Vector3(4.2f, 0.08f, 34f);
            strip.GetComponent<Renderer>().sharedMaterial = pitch;
        }

        private static void CreateStadium()
        {
            Material standMat = Material("Stand Blue Steel", new Color(0.08f, 0.15f, 0.22f));
            for (int ring = 0; ring < 3; ring++)
            {
                float radius = 58f + ring * 5f;
                for (int i = 0; i < 28; i++)
                {
                    float angle = (i / 28f) * Mathf.PI * 2f;
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.name = $"Stand R{ring + 1}-{i + 1}";
                    block.transform.position = new Vector3(Mathf.Cos(angle) * radius, 2f + ring * 1.5f, Mathf.Sin(angle) * radius);
                    block.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);
                    block.transform.localScale = new Vector3(4.2f, 2.2f + ring, 3.2f);
                    block.GetComponent<Renderer>().sharedMaterial = standMat;
                }
            }
        }

        private static GameObject CreatePlayer(string name, Material kit, Material legs, Vector3 position)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform);
            body.transform.localPosition = new Vector3(0, 1.25f, 0);
            body.transform.localScale = new Vector3(0.62f, 0.9f, 0.62f);
            body.GetComponent<Renderer>().sharedMaterial = kit;

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform);
            head.transform.localPosition = new Vector3(0, 2.45f, 0);
            head.transform.localScale = Vector3.one * 0.42f;

            for (int i = 0; i < 2; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                leg.name = i == 0 ? "Left Leg" : "Right Leg";
                leg.transform.SetParent(root.transform);
                leg.transform.localPosition = new Vector3(i == 0 ? -0.24f : 0.24f, 0.42f, 0);
                leg.transform.localScale = new Vector3(0.18f, 0.45f, 0.18f);
                leg.GetComponent<Renderer>().sharedMaterial = legs;
            }

            return root;
        }

        private static GameObject CreateCameraRig(string name, Vector3 position, Vector3 rotation)
        {
            GameObject rig = new GameObject(name);
            rig.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
            return rig;
        }

        private static GameObject CreateHud(MatchManager match, BattingController batting, BowlingController bowling)
        {
            GameObject canvasObj = new GameObject("ScoreHUD");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject scoreObj = CreateHudText("ScoreText", canvasObj.transform, "0/0", 34, new Vector2(0.04f, 0.88f), new Vector2(0.24f, 0.98f), TextAnchor.MiddleLeft);
            GameObject equationObj = CreateHudText("EquationText", canvasObj.transform, "24 from 6", 28, new Vector2(0.76f, 0.88f), new Vector2(0.96f, 0.98f), TextAnchor.MiddleRight);
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(canvasObj.transform);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = "Cricket Arena";
            text.color = Color.white;
            text.fontSize = 32;
            text.alignment = TextAnchor.UpperCenter;
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.1f, 0.84f);
            textRect.anchorMax = new Vector2(0.9f, 0.98f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var hud = canvasObj.AddComponent<ScoreHudController>();
            hud.Bind(match);
            hud.BindText(scoreObj.GetComponent<Text>(), equationObj.GetComponent<Text>(), text);

            var controls = canvasObj.AddComponent<MobileControlsController>();
            SerializedObject controlsObj = new SerializedObject(controls);
            SetObject(controlsObj, "battingController", batting);
            SetObject(controlsObj, "bowlingController", bowling);
            controlsObj.ApplyModifiedPropertiesWithoutUndo();

            return canvasObj;
        }

        private static GameObject CreateHudText(string name, Transform parent, string value, int size, Vector2 anchorMin, Vector2 anchorMax, TextAnchor anchor)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = value;
            text.color = Color.white;
            text.fontSize = size;
            text.alignment = anchor;
            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return textObj;
        }

        private static Material Material(string name, Color color)
        {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (material.shader == null)
            {
                material = new Material(Shader.Find("Standard"));
            }
            material.name = name;
            material.color = color;
            return material;
        }

        private static void SetObject(SerializedObject obj, string property, Object value)
        {
            obj.FindProperty(property).objectReferenceValue = value;
        }

        private static void SetDeliveries(SerializedObject obj)
        {
            SerializedProperty deliveries = obj.FindProperty("deliveries");
            deliveries.arraySize = 4;
            SetDelivery(deliveries.GetArrayElementAtIndex(0), "Fast yorker", 31f, -0.08f, 0.38f, 0.86f);
            SetDelivery(deliveries.GetArrayElementAtIndex(1), "Outswinger", 26f, 0.16f, 0.62f, 0.72f);
            SetDelivery(deliveries.GetArrayElementAtIndex(2), "Leg cutter", 23f, -0.18f, 0.52f, 0.78f);
            SetDelivery(deliveries.GetArrayElementAtIndex(3), "Slower bouncer", 20f, 0.04f, 0.95f, 0.66f);
        }

        private static void SetDelivery(SerializedProperty prop, string name, float speed, float swing, float bounce, float difficulty)
        {
            prop.FindPropertyRelative("Name").stringValue = name;
            prop.FindPropertyRelative("Speed").floatValue = speed;
            prop.FindPropertyRelative("Swing").floatValue = swing;
            prop.FindPropertyRelative("Bounce").floatValue = bounce;
            prop.FindPropertyRelative("Difficulty").floatValue = difficulty;
        }
    }
}
#endif
