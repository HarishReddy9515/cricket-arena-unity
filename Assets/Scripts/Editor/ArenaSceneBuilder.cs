#if UNITY_EDITOR
using CricketArena.Core;
using CricketArena.Gameplay;
using CricketArena.Networking;
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
            CreateAtmosphere();

            GameObject game = new GameObject("Game");
            var match = game.AddComponent<MatchManager>();
            var career = game.AddComponent<CareerProgressionManager>();
            var tournament = game.AddComponent<TournamentManager>();
            var season = game.AddComponent<SeasonProgression>();
            var inventory = game.AddComponent<InventoryManager>();
            var haptics = game.AddComponent<MobileHaptics>();
            var replayRecorder = game.AddComponent<ReplayRecorder>();
            var impactVfx = game.AddComponent<ImpactVfxController>();
            var batting = game.AddComponent<BattingController>();
            var battingAssist = game.AddComponent<BattingAssistController>();
            var bowling = game.AddComponent<BowlingController>();
            var aiBowling = game.AddComponent<AIBowlingStrategy>();
            var cameraDirector = game.AddComponent<CameraDirector>();
            var assetBinder = game.AddComponent<RuntimeAssetBinder>();
            var animationDirector = game.AddComponent<PlayerAnimationDirector>();
            var showcase = game.AddComponent<LobbyShowcaseController>();
            var performance = game.AddComponent<MobilePerformanceManager>();
            var networkClient = game.AddComponent<RealtimeMatchClient>();
            var networkSync = game.AddComponent<NetworkGameplaySynchronizer>();

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
            Animator batterAnimator = batter.AddComponent<Animator>();
            Animator bowlerAnimator = bowler.AddComponent<Animator>();
            GameObject hero = CreatePlayer("LobbyHeroPlayer", blue, white, new Vector3(18f, 0, 6f));
            hero.transform.rotation = Quaternion.Euler(0, -28f, 0);
            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "LobbyShowcasePedestal";
            pedestal.transform.position = new Vector3(18f, 0.08f, 6f);
            pedestal.transform.localScale = new Vector3(2.6f, 0.12f, 2.6f);
            pedestal.GetComponent<Renderer>().sharedMaterial = Material("Showcase Dark Metal", new Color(0.025f, 0.03f, 0.035f));
            GameObject stadiumMount = new GameObject("StadiumAssetMount");
            GameObject batterMount = new GameObject("BatterAssetMount");
            GameObject bowlerMount = new GameObject("BowlerAssetMount");
            GameObject batMount = new GameObject("BatAssetMount");
            GameObject wicketMount = new GameObject("WicketAssetMount");
            stadiumMount.transform.position = Vector3.zero;
            batterMount.transform.position = batter.transform.position;
            bowlerMount.transform.position = bowler.transform.position;
            batMount.transform.position = new Vector3(0.45f, 1.15f, 21.25f);
            wicketMount.transform.position = new Vector3(0, 0.45f, 22.4f);

            var crowdAudio = game.AddComponent<AudioSource>();
            crowdAudio.playOnAwake = false;
            crowdAudio.spatialBlend = 0f;
            crowdAudio.volume = 0.28f;
            var effectsAudio = ball.AddComponent<AudioSource>();
            effectsAudio.playOnAwake = false;
            effectsAudio.spatialBlend = 0.85f;

            GameObject contact = new GameObject("ContactPoint");
            contact.transform.position = new Vector3(0, 0.85f, 20.7f);

            GameObject release = new GameObject("ReleasePoint");
            release.transform.position = new Vector3(0, 1.55f, -20.5f);

            GameObject target = new GameObject("TargetPoint");
            target.transform.position = new Vector3(0, 0.52f, 20.8f);

            GameObject battingCam = CreateCameraRig("BattingCameraRig", new Vector3(0, 8.5f, 36f), new Vector3(15f, 180f, 0));
            GameObject lobbyCam = CreateCameraRig("LobbyCameraRig", new Vector3(10f, 4.6f, 19f), new Vector3(8f, 151f, 0));
            GameObject replayCam = CreateCameraRig("ReplayCameraRig", new Vector3(-14f, 8f, 12f), new Vector3(18f, 130f, 0));

            Camera mainCamera = new GameObject("Main Camera").AddComponent<Camera>();
            mainCamera.transform.SetPositionAndRotation(lobbyCam.transform.position, lobbyCam.transform.rotation);
            mainCamera.gameObject.tag = "MainCamera";
            mainCamera.gameObject.AddComponent<AudioListener>();

            GameObject ui = CreateHud(match, batting, bowling, networkClient, career, tournament, season, cameraDirector, inventory, performance, battingAssist);

            SerializedObject careerObj = new SerializedObject(career);
            SetObject(careerObj, "matchManager", match);
            careerObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject tournamentObj = new SerializedObject(tournament);
            SetObject(tournamentObj, "matchManager", match);
            tournamentObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject seasonObj = new SerializedObject(season);
            SetObject(seasonObj, "matchManager", match);
            seasonObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject battingObj = new SerializedObject(batting);
            SetObject(battingObj, "matchManager", match);
            SetObject(battingObj, "ballPhysics", ballPhysics);
            SetObject(battingObj, "batterAnimator", batterAnimator);
            SetObject(battingObj, "animationDirector", animationDirector);
            SetObject(battingObj, "battingAssist", battingAssist);
            SetObject(battingObj, "contactPoint", contact.transform);
            SetObject(battingObj, "haptics", haptics);
            SetObject(battingObj, "impactVfx", impactVfx);
            SetObject(battingObj, "replayRecorder", replayRecorder);
            battingObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject battingAssistObj = new SerializedObject(battingAssist);
            SetObject(battingAssistObj, "seasonProgression", season);
            battingAssistObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject bowlingObj = new SerializedObject(bowling);
            SetObject(bowlingObj, "matchManager", match);
            SetObject(bowlingObj, "ballPhysics", ballPhysics);
            SetObject(bowlingObj, "bowlerAnimator", bowlerAnimator);
            SetObject(bowlingObj, "animationDirector", animationDirector);
            SetObject(bowlingObj, "aiStrategy", aiBowling);
            SetObject(bowlingObj, "releasePoint", release.transform);
            SetObject(bowlingObj, "targetPoint", target.transform);
            SetDeliveries(bowlingObj);
            bowlingObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject aiBowlingObj = new SerializedObject(aiBowling);
            SetObject(aiBowlingObj, "matchManager", match);
            aiBowlingObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject ballObj = new SerializedObject(ballPhysics);
            SetObject(ballObj, "body", body);
            SetObject(ballObj, "contactPoint", contact.transform);
            ballObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject cameraObj = new SerializedObject(cameraDirector);
            SetObject(cameraObj, "mainCamera", mainCamera);
            SetObject(cameraObj, "lobbyCamera", lobbyCam.transform);
            SetObject(cameraObj, "battingCamera", battingCam.transform);
            SetObject(cameraObj, "replayCamera", replayCam.transform);
            cameraObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject showcaseObj = new SerializedObject(showcase);
            SetObject(showcaseObj, "showcaseTarget", hero.transform);
            showcaseObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject assetBinderObj = new SerializedObject(assetBinder);
            SetObject(assetBinderObj, "stadiumMount", stadiumMount.transform);
            SetObject(assetBinderObj, "batterMount", batterMount.transform);
            SetObject(assetBinderObj, "bowlerMount", bowlerMount.transform);
            SetObject(assetBinderObj, "batMount", batMount.transform);
            SetObject(assetBinderObj, "wicketMount", wicketMount.transform);
            SetObject(assetBinderObj, "crowdAudio", crowdAudio);
            SetObject(assetBinderObj, "effectsAudio", effectsAudio);
            assetBinderObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject animationObj = new SerializedObject(animationDirector);
            SetObject(animationObj, "batterAnimator", batterAnimator);
            SetObject(animationObj, "bowlerAnimator", bowlerAnimator);
            SetObject(animationObj, "matchManager", match);
            animationObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject networkSyncObj = new SerializedObject(networkSync);
            SetObject(networkSyncObj, "client", networkClient);
            SetObject(networkSyncObj, "matchManager", match);
            SetObject(networkSyncObj, "bowlingController", bowling);
            networkSyncObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject replayObj = new SerializedObject(replayRecorder);
            SetObject(replayObj, "cameraDirector", cameraDirector);
            SetObject(replayObj, "ball", ball.transform);
            replayObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject vfxObj = new SerializedObject(impactVfx);
            SetObject(vfxObj, "cameraDirector", cameraDirector);
            SetObject(vfxObj, "assetBinder", assetBinder);
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

        private static void CreateAtmosphere()
        {
            Material gold = Material("Arena Banner Gold", new Color(0.95f, 0.62f, 0.12f));
            Material cyan = Material("Arena Crowd Cyan", new Color(0.05f, 0.45f, 0.70f));
            Material navy = Material("Arena Crowd Navy", new Color(0.02f, 0.05f, 0.09f));

            for (int i = 0; i < 12; i++)
            {
                float angle = (i / 12f) * Mathf.PI * 2f;
                GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Cube);
                banner.name = $"Arena Banner {i + 1}";
                banner.transform.position = new Vector3(Mathf.Cos(angle) * 48f, 5.2f, Mathf.Sin(angle) * 48f);
                banner.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);
                banner.transform.localScale = new Vector3(7.5f, 1.2f, 0.18f);
                banner.GetComponent<Renderer>().sharedMaterial = i % 2 == 0 ? gold : cyan;
            }

            for (int ring = 0; ring < 2; ring++)
            {
                float radius = 63f + ring * 6f;
                for (int i = 0; i < 36; i++)
                {
                    float angle = (i / 36f) * Mathf.PI * 2f;
                    GameObject crowd = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    crowd.name = $"Crowd Color Band {ring + 1}-{i + 1}";
                    crowd.transform.position = new Vector3(Mathf.Cos(angle) * radius, 6.4f + ring * 1.2f, Mathf.Sin(angle) * radius);
                    crowd.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);
                    crowd.transform.localScale = new Vector3(2.2f, 0.9f, 0.32f);
                    crowd.GetComponent<Renderer>().sharedMaterial = (i + ring) % 3 == 0 ? cyan : navy;
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

        private static GameObject CreateHud(MatchManager match, BattingController batting, BowlingController bowling, RealtimeMatchClient networkClient, CareerProgressionManager career, TournamentManager tournament, SeasonProgression season, CameraDirector cameraDirector, InventoryManager inventory, MobilePerformanceManager performance, BattingAssistController battingAssist)
        {
            GameObject canvasObj = new GameObject("ScoreHUD");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject topBar = CreatePanel("TopStatusBar", canvasObj.transform, new Vector2(0.02f, 0.90f), new Vector2(0.98f, 0.985f), new Color(0.02f, 0.025f, 0.03f, 0.88f));
            GameObject leftPanel = CreatePanel("ModePanel", canvasObj.transform, new Vector2(0.02f, 0.16f), new Vector2(0.25f, 0.88f), new Color(0.02f, 0.025f, 0.03f, 0.80f));
            GameObject rightPanel = CreatePanel("SquadPanel", canvasObj.transform, new Vector2(0.73f, 0.16f), new Vector2(0.98f, 0.88f), new Color(0.02f, 0.025f, 0.03f, 0.80f));
            GameObject bottomBar = CreatePanel("ActionBar", canvasObj.transform, new Vector2(0.02f, 0.025f), new Vector2(0.98f, 0.14f), new Color(0.02f, 0.025f, 0.03f, 0.86f));
            CanvasGroup topGroup = topBar.AddComponent<CanvasGroup>();
            CanvasGroup leftGroup = leftPanel.AddComponent<CanvasGroup>();
            CanvasGroup rightGroup = rightPanel.AddComponent<CanvasGroup>();
            CanvasGroup bottomGroup = bottomBar.AddComponent<CanvasGroup>();

            GameObject scoreObj = CreateHudText("ScoreText", topBar.transform, "0/0", 28, new Vector2(0.01f, 0.1f), new Vector2(0.18f, 0.9f), TextAnchor.MiddleLeft);
            GameObject equationObj = CreateHudText("EquationText", topBar.transform, "24 from 6", 22, new Vector2(0.78f, 0.1f), new Vector2(0.98f, 0.9f), TextAnchor.MiddleRight);
            GameObject networkObj = CreateHudText("NetworkStatusText", topBar.transform, "Offline", 16, new Vector2(0.53f, 0.1f), new Vector2(0.76f, 0.9f), TextAnchor.MiddleRight);
            GameObject modeObj = CreateHudText("ModeText", topBar.transform, "Select Mode", 20, new Vector2(0.26f, 0.1f), new Vector2(0.52f, 0.9f), TextAnchor.MiddleCenter);
            GameObject titleObj = CreateHudText("LobbyTitleText", topBar.transform, "CRICKET ARENA", 24, new Vector2(0.19f, 0.1f), new Vector2(0.35f, 0.9f), TextAnchor.MiddleLeft);
            GameObject seasonObj = CreateHudText("SeasonText", leftPanel.transform, "Season 01 | Night League", 16, new Vector2(0.08f, 0.86f), new Vector2(0.92f, 0.96f), TextAnchor.MiddleLeft);
            GameObject squadObj = CreateHudText("SquadText", rightPanel.transform, "Harish XI", 22, new Vector2(0.08f, 0.64f), new Vector2(0.92f, 0.88f), TextAnchor.UpperLeft);
            GameObject loadoutObj = CreateHudText("LoadoutText", rightPanel.transform, "Bat: Balanced", 16, new Vector2(0.08f, 0.36f), new Vector2(0.92f, 0.58f), TextAnchor.UpperLeft);
            GameObject currencyObj = CreateHudText("CurrencyText", rightPanel.transform, "XP 0 | Coins 0", 16, new Vector2(0.08f, 0.88f), new Vector2(0.92f, 0.97f), TextAnchor.MiddleRight);
            GameObject primaryActionObj = CreateHudText("PrimaryActionText", bottomBar.transform, "PLAY", 24, new Vector2(0.77f, 0.12f), new Vector2(0.94f, 0.88f), TextAnchor.MiddleCenter);
            GameObject timingObj = CreateHudText("TimingFeedbackText", bottomBar.transform, "Timing", 22, new Vector2(0.53f, 0.16f), new Vector2(0.74f, 0.84f), TextAnchor.MiddleCenter);
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(topBar.transform);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = "Ready";
            text.color = Color.white;
            text.fontSize = 18;
            text.alignment = TextAnchor.UpperCenter;
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.36f, 0.1f);
            textRect.anchorMax = new Vector2(0.54f, 0.9f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var hud = canvasObj.AddComponent<ScoreHudController>();
            hud.Bind(match);
            hud.BindText(scoreObj.GetComponent<Text>(), equationObj.GetComponent<Text>(), text);
            battingAssist.OnTimingFeedback.AddListener(value => timingObj.GetComponent<Text>().text = value);

            var controls = canvasObj.AddComponent<MobileControlsController>();
            SerializedObject controlsObj = new SerializedObject(controls);
            SetObject(controlsObj, "battingController", batting);
            SetObject(controlsObj, "bowlingController", bowling);
            controlsObj.ApplyModifiedPropertiesWithoutUndo();

            CreateButton(leftPanel.transform, "QuickMatchButton", "Quick Match", new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.82f), nameof(GameModeMenuController.StartQuickMatch));
            CreateButton(leftPanel.transform, "PracticeNetsButton", "Practice Nets", new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.68f), nameof(GameModeMenuController.StartPracticeNets));
            CreateButton(leftPanel.transform, "CareerButton", "Career", new Vector2(0.08f, 0.44f), new Vector2(0.92f, 0.54f), nameof(GameModeMenuController.StartCareer));
            CreateButton(leftPanel.transform, "TournamentButton", "Tournament", new Vector2(0.08f, 0.30f), new Vector2(0.92f, 0.40f), nameof(GameModeMenuController.StartTournament));
            CreateButton(leftPanel.transform, "OnlineModeButton", "Online Room", new Vector2(0.08f, 0.16f), new Vector2(0.92f, 0.26f), nameof(GameModeMenuController.StartOnlineRoom));

            var modes = canvasObj.AddComponent<GameModeMenuController>();
            var screenDirector = canvasObj.AddComponent<ArenaScreenDirector>();
            SerializedObject modesObj = new SerializedObject(modes);
            SetObject(modesObj, "matchManager", match);
            SetObject(modesObj, "career", career);
            SetObject(modesObj, "tournament", tournament);
            SetObject(modesObj, "screenDirector", screenDirector);
            SetObject(modesObj, "modeText", modeObj.GetComponent<Text>());
            modesObj.ApplyModifiedPropertiesWithoutUndo();

            InputField roomInput = CreateInput(rightPanel.transform, "ARENA-24", new Vector2(0.08f, 0.22f), new Vector2(0.92f, 0.31f));
            CreateButton(rightPanel.transform, "ConnectButton", "Connect", new Vector2(0.08f, 0.10f), new Vector2(0.45f, 0.19f), nameof(MultiplayerLobbyController.Connect));
            CreateButton(rightPanel.transform, "JoinButton", "Join", new Vector2(0.55f, 0.10f), new Vector2(0.92f, 0.19f), nameof(MultiplayerLobbyController.JoinRoom));
            CreateButton(bottomBar.transform, "ReadyButton", "Ready", new Vector2(0.04f, 0.16f), new Vector2(0.17f, 0.84f), nameof(MultiplayerLobbyController.Ready));
            CreateButton(bottomBar.transform, "DeliveryButton", "Delivery", new Vector2(0.19f, 0.16f), new Vector2(0.34f, 0.84f), nameof(MultiplayerLobbyController.RequestDelivery));
            CreateButton(bottomBar.transform, "ShotButton", "Shot", new Vector2(0.36f, 0.16f), new Vector2(0.50f, 0.84f), nameof(MultiplayerLobbyController.SendShot));
            CreateButton(bottomBar.transform, "PrimaryPlayButton", "PLAY", new Vector2(0.76f, 0.10f), new Vector2(0.95f, 0.90f), nameof(MultiplayerLobbyController.RequestDelivery));

            var lobby = canvasObj.AddComponent<MultiplayerLobbyController>();
            SerializedObject lobbyObj = new SerializedObject(lobby);
            SetObject(lobbyObj, "client", networkClient);
            SetObject(lobbyObj, "battingController", batting);
            SetObject(lobbyObj, "bowlingController", bowling);
            SetObject(lobbyObj, "roomCodeInput", roomInput);
            SetObject(lobbyObj, "statusText", networkObj.GetComponent<Text>());
            lobbyObj.ApplyModifiedPropertiesWithoutUndo();

            var skin = canvasObj.AddComponent<ArenaLobbySkin>();
            var loadout = canvasObj.AddComponent<LoadoutController>();
            SerializedObject skinObj = new SerializedObject(skin);
            SetObject(skinObj, "titleText", titleObj.GetComponent<Text>());
            SetObject(skinObj, "seasonText", seasonObj.GetComponent<Text>());
            SetObject(skinObj, "currencyText", currencyObj.GetComponent<Text>());
            SetObject(skinObj, "squadText", squadObj.GetComponent<Text>());
            SetObject(skinObj, "loadoutText", loadoutObj.GetComponent<Text>());
            SetObject(skinObj, "primaryActionText", primaryActionObj.GetComponent<Text>());
            skinObj.ApplyModifiedPropertiesWithoutUndo();

            CreateButton(rightPanel.transform, "BatLoadoutButton", "Bat", new Vector2(0.08f, 0.49f), new Vector2(0.31f, 0.57f), nameof(LoadoutController.NextBat));
            CreateButton(rightPanel.transform, "KitLoadoutButton", "Kit", new Vector2(0.38f, 0.49f), new Vector2(0.61f, 0.57f), nameof(LoadoutController.NextKit));
            CreateButton(rightPanel.transform, "BoostLoadoutButton", "Boost", new Vector2(0.68f, 0.49f), new Vector2(0.92f, 0.57f), nameof(LoadoutController.NextBoost));
            CreateButton(rightPanel.transform, "BatteryGraphicsButton", "30", new Vector2(0.08f, 0.01f), new Vector2(0.28f, 0.08f), nameof(GraphicsSettingsController.SetBattery));
            CreateButton(rightPanel.transform, "BalancedGraphicsButton", "60", new Vector2(0.40f, 0.01f), new Vector2(0.60f, 0.08f), nameof(GraphicsSettingsController.SetBalanced));
            CreateButton(rightPanel.transform, "PerformanceGraphicsButton", "Max", new Vector2(0.72f, 0.01f), new Vector2(0.92f, 0.08f), nameof(GraphicsSettingsController.SetPerformance));

            SerializedObject loadoutObj = new SerializedObject(loadout);
            SetObject(loadoutObj, "lobbySkin", skin);
            SetObject(loadoutObj, "inventory", inventory);
            loadoutObj.ApplyModifiedPropertiesWithoutUndo();

            var graphics = canvasObj.AddComponent<GraphicsSettingsController>();
            SerializedObject graphicsObj = new SerializedObject(graphics);
            SetObject(graphicsObj, "performanceManager", performance);
            SetObject(graphicsObj, "statusText", networkObj.GetComponent<Text>());
            graphicsObj.ApplyModifiedPropertiesWithoutUndo();

            var saveGame = canvasObj.AddComponent<SaveGameManager>();
            SerializedObject saveObj = new SerializedObject(saveGame);
            SetObject(saveObj, "loadoutController", loadout);
            SetObject(saveObj, "seasonProgression", season);
            SetObject(saveObj, "performanceManager", performance);
            saveObj.ApplyModifiedPropertiesWithoutUndo();

            var rewardBridge = canvasObj.AddComponent<SeasonRewardBridge>();
            SerializedObject rewardObj = new SerializedObject(rewardBridge);
            SetObject(rewardObj, "seasonProgression", season);
            SetObject(rewardObj, "loadoutController", loadout);
            rewardObj.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject screenObj = new SerializedObject(screenDirector);
            SetObject(screenObj, "cameraDirector", cameraDirector);
            SerializedProperty lobbyGroups = screenObj.FindProperty("lobbyGroups");
            lobbyGroups.arraySize = 2;
            lobbyGroups.GetArrayElementAtIndex(0).objectReferenceValue = leftGroup;
            lobbyGroups.GetArrayElementAtIndex(1).objectReferenceValue = rightGroup;
            SerializedProperty gameplayGroups = screenObj.FindProperty("gameplayGroups");
            gameplayGroups.arraySize = 2;
            gameplayGroups.GetArrayElementAtIndex(0).objectReferenceValue = topGroup;
            gameplayGroups.GetArrayElementAtIndex(1).objectReferenceValue = bottomGroup;
            screenObj.ApplyModifiedPropertiesWithoutUndo();

            foreach (Button button in canvasObj.GetComponentsInChildren<Button>())
            {
                button.onClick.RemoveAllListeners();
                string method = button.gameObject.name.Replace("Button", string.Empty);
                if (method == "Connect") button.onClick.AddListener(lobby.Connect);
                if (method == "Join") button.onClick.AddListener(lobby.JoinRoom);
                if (method == "Ready") button.onClick.AddListener(lobby.Ready);
                if (method == "Delivery") button.onClick.AddListener(lobby.RequestDelivery);
                if (method == "Shot") button.onClick.AddListener(lobby.SendShot);
                if (method == "PrimaryPlay") button.onClick.AddListener(lobby.RequestDelivery);
                if (method == "BatLoadout") button.onClick.AddListener(loadout.NextBat);
                if (method == "KitLoadout") button.onClick.AddListener(loadout.NextKit);
                if (method == "BoostLoadout") button.onClick.AddListener(loadout.NextBoost);
                if (method == "BatteryGraphics") button.onClick.AddListener(graphics.SetBattery);
                if (method == "BalancedGraphics") button.onClick.AddListener(graphics.SetBalanced);
                if (method == "PerformanceGraphics") button.onClick.AddListener(graphics.SetPerformance);
                if (method == "QuickMatch") button.onClick.AddListener(modes.StartQuickMatch);
                if (method == "PracticeNets") button.onClick.AddListener(modes.StartPracticeNets);
                if (method == "Career") button.onClick.AddListener(modes.StartCareer);
                if (method == "Tournament") button.onClick.AddListener(modes.StartTournament);
                if (method == "OnlineMode") button.onClick.AddListener(modes.StartOnlineRoom);
            }

            return canvasObj;
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return panel;
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

        private static InputField CreateInput(Transform parent, string value, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject root = new GameObject("RoomCodeInput");
            root.transform.SetParent(parent);
            Image image = root.AddComponent<Image>();
            image.color = new Color(0.04f, 0.06f, 0.08f, 0.86f);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            GameObject textObj = CreateHudText("Text", root.transform, value, 18, Vector2.zero, Vector2.one, TextAnchor.MiddleLeft);
            Text text = textObj.GetComponent<Text>();
            text.color = Color.white;
            text.supportRichText = false;
            text.rectTransform.offsetMin = new Vector2(12f, 0f);
            text.rectTransform.offsetMax = new Vector2(-8f, 0f);

            InputField field = root.AddComponent<InputField>();
            field.text = value;
            field.textComponent = text;
            return field;
        }

        private static void CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, string methodName)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            Image image = root.AddComponent<Image>();
            image.color = new Color(0.03f, 0.32f, 0.75f, 0.9f);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Button button = root.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;

            GameObject labelObj = CreateHudText($"{methodName}Label", root.transform, label, 16, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter);
            labelObj.GetComponent<Text>().color = Color.white;
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
