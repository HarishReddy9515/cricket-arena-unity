const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "..");

const requiredFiles = [
  "Packages/manifest.json",
  "ProjectSettings/ProjectVersion.txt",
  "Assets/Scripts/Core/MatchManager.cs",
  "Assets/Scripts/Core/GameModeConfig.cs",
  "Assets/Scripts/Core/CareerProgressionManager.cs",
  "Assets/Scripts/Core/TournamentManager.cs",
  "Assets/Scripts/Core/ShotOutcome.cs",
  "Assets/Scripts/Core/ReplayEvent.cs",
  "Assets/Scripts/Core/ReplayRecorder.cs",
  "Assets/Scripts/Gameplay/BattingController.cs",
  "Assets/Scripts/Gameplay/BowlingController.cs",
  "Assets/Scripts/Gameplay/BallPhysicsController.cs",
  "Assets/Scripts/Presentation/CameraDirector.cs",
  "Assets/Scripts/Presentation/CricketAssetManifest.cs",
  "Assets/Scripts/Presentation/MobileHaptics.cs",
  "Assets/Scripts/Presentation/ImpactVfxController.cs",
  "Assets/Scripts/Presentation/MobilePerformanceManager.cs",
  "Assets/Scripts/Presentation/PlayerAnimationDirector.cs",
  "Assets/Scripts/Presentation/RuntimeAssetBinder.cs",
  "Assets/Scripts/Networking/RealtimeMatchClient.cs",
  "Assets/Scripts/Networking/MatchProtocol.cs",
  "Assets/Scripts/Networking/NetworkGameplaySynchronizer.cs",
  "Assets/Scripts/UI/ScoreHudController.cs",
  "Assets/Scripts/UI/MobileControlsController.cs",
  "Assets/Scripts/UI/MultiplayerLobbyController.cs",
  "Assets/Scripts/UI/GameModeMenuController.cs",
  "Assets/Scripts/Editor/ArenaSceneBuilder.cs",
  "Assets/Scripts/Editor/AssetReadinessValidator.cs",
  "Assets/Scripts/Editor/MobileBuildConfigurator.cs",
  "Assets/Scripts/Editor/AndroidBuildPipeline.cs",
  "docs/ASSET_PIPELINE.md",
  "docs/ANIMATION_PIPELINE.md",
  "docs/BUILD_AND_RELEASE.md",
  "docs/MULTIPLAYER_OPERATIONS.md",
  "docs/MOBILE_PERFORMANCE.md",
  "docs/GAME_MODES.md",
  "scripts/run-checks.ps1",
  ".github/workflows/smoke.yml",
  "server/Dockerfile",
  "server/docker-compose.yml",
  "server/protocol-smoke-test.js"
];

for (const file of requiredFiles) {
  if (!fs.existsSync(path.join(root, file))) {
    throw new Error(`Missing Unity scaffold file: ${file}`);
  }
}

const matchManager = fs.readFileSync(path.join(root, "Assets/Scripts/Core/MatchManager.cs"), "utf8");
const gameMode = fs.readFileSync(path.join(root, "Assets/Scripts/Core/GameModeConfig.cs"), "utf8");
const career = fs.readFileSync(path.join(root, "Assets/Scripts/Core/CareerProgressionManager.cs"), "utf8");
const tournament = fs.readFileSync(path.join(root, "Assets/Scripts/Core/TournamentManager.cs"), "utf8");
const batting = fs.readFileSync(path.join(root, "Assets/Scripts/Gameplay/BattingController.cs"), "utf8");
const ball = fs.readFileSync(path.join(root, "Assets/Scripts/Gameplay/BallPhysicsController.cs"), "utf8");
const network = fs.readFileSync(path.join(root, "Assets/Scripts/Networking/RealtimeMatchClient.cs"), "utf8");
const protocol = fs.readFileSync(path.join(root, "Assets/Scripts/Networking/MatchProtocol.cs"), "utf8");
const networkSync = fs.readFileSync(path.join(root, "Assets/Scripts/Networking/NetworkGameplaySynchronizer.cs"), "utf8");
const lobby = fs.readFileSync(path.join(root, "Assets/Scripts/UI/MultiplayerLobbyController.cs"), "utf8");
const modeMenu = fs.readFileSync(path.join(root, "Assets/Scripts/UI/GameModeMenuController.cs"), "utf8");
const server = fs.readFileSync(path.join(root, "server/authoritative-server.js"), "utf8");
const sceneBuilder = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/ArenaSceneBuilder.cs"), "utf8");
const validator = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/AssetReadinessValidator.cs"), "utf8");
const mobileBuild = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/MobileBuildConfigurator.cs"), "utf8");
const androidBuild = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/AndroidBuildPipeline.cs"), "utf8");
const replay = fs.readFileSync(path.join(root, "Assets/Scripts/Core/ReplayRecorder.cs"), "utf8");
const manifest = fs.readFileSync(path.join(root, "Assets/Scripts/Presentation/CricketAssetManifest.cs"), "utf8");
const binder = fs.readFileSync(path.join(root, "Assets/Scripts/Presentation/RuntimeAssetBinder.cs"), "utf8");
const impact = fs.readFileSync(path.join(root, "Assets/Scripts/Presentation/ImpactVfxController.cs"), "utf8");
const animation = fs.readFileSync(path.join(root, "Assets/Scripts/Presentation/PlayerAnimationDirector.cs"), "utf8");
const performance = fs.readFileSync(path.join(root, "Assets/Scripts/Presentation/MobilePerformanceManager.cs"), "utf8");

for (const symbol of ["MatchPhase", "ApplyOutcome", "OnScoreChanged", "SyncAuthoritativeState", "Configure", "CricketGameMode"]) {
  if (!matchManager.includes(symbol)) throw new Error(`MatchManager missing ${symbol}`);
}

for (const symbol of ["CricketGameMode", "QuickMatch", "PracticeNets", "CareerChase", "TournamentChase", "OnlineRoom"]) {
  if (!gameMode.includes(symbol)) throw new Error(`GameModeConfig missing ${symbol}`);
}

for (const symbol of ["CareerProgressionManager", "StartCareerMatch", "SkillPoints", "OnCareerChanged"]) {
  if (!career.includes(symbol)) throw new Error(`CareerProgressionManager missing ${symbol}`);
}

for (const symbol of ["TournamentManager", "StartTournament", "StartRound", "OnTournamentChanged"]) {
  if (!tournament.includes(symbol)) throw new Error(`TournamentManager missing ${symbol}`);
}

for (const symbol of ["ShotIntent", "PlayShot", "ResolveOutcome", "MobileHaptics"]) {
  if (!batting.includes(symbol)) throw new Error(`BattingController missing ${symbol}`);
}

if (!batting.includes("PlayerAnimationDirector")) {
  throw new Error("BattingController missing PlayerAnimationDirector");
}

for (const symbol of ["Launch", "ResolveShot", "Rigidbody"]) {
  if (!ball.includes(symbol)) throw new Error(`BallPhysicsController missing ${symbol}`);
}

if (!fs.readFileSync(path.join(root, "Assets/Scripts/Gameplay/BowlingController.cs"), "utf8").includes("PlayerAnimationDirector")) {
  throw new Error("BowlingController missing PlayerAnimationDirector");
}

for (const symbol of ["ClientWebSocket", "ConnectAsync", "SendAsync", "ReceiveLoop", "Disconnect", "JoinRoom", "SetReady", "RequestDelivery", "SendShot", "ReceiveJson", "LastOutboundJson", "LastLatencyMs", "OnServerError", "ScheduleReconnect", "ReconnectAttempts", "heartbeatIntervalSeconds"]) {
  if (!network.includes(symbol)) throw new Error(`RealtimeMatchClient missing ${symbol}`);
}

for (const symbol of ["MatchMessage", "DeliveryDto", "RoomDto", "OutcomeDto", "ErrorMessage", "MatchEvents", "request_delivery"]) {
  if (!protocol.includes(symbol)) throw new Error(`MatchProtocol missing ${symbol}`);
}

for (const symbol of ["NetworkGameplaySynchronizer", "HandleDelivery", "HandleMatchState", "BowlServerDelivery"]) {
  if (!networkSync.includes(symbol)) throw new Error(`NetworkGameplaySynchronizer missing ${symbol}`);
}

for (const symbol of ["MultiplayerLobbyController", "Ready", "RequestDelivery", "SetShotTiming", "OnServerError", "LastLatencyMs"]) {
  if (!lobby.includes(symbol)) throw new Error(`MultiplayerLobbyController missing ${symbol}`);
}

for (const symbol of ["GameModeMenuController", "StartQuickMatch", "StartPracticeNets", "StartCareer", "StartTournament", "StartOnlineRoom"]) {
  if (!modeMenu.includes(symbol)) throw new Error(`GameModeMenuController missing ${symbol}`);
}

for (const symbol of ["join_room", "request_delivery", "resolveOutcome", "match_state", "sanitizeRoomCode", "MAX_ROOM_PLAYERS", "cleanupRooms", "RATE_LIMIT_MAX_MESSAGES", "/metrics", "decodeFrames"]) {
  if (!server.includes(symbol)) throw new Error(`authoritative-server missing ${symbol}`);
}

for (const symbol of ["MenuItem", "BuildScene", "CreateStadium", "CreatePlayer", "CreateHud", "RealtimeMatchClient", "NetworkGameplaySynchronizer", "RuntimeAssetBinder", "MobilePerformanceManager", "GameModeMenuController", "QuickMatchButton", "CareerButton", "TournamentButton", "RoomCodeInput"]) {
  if (!sceneBuilder.includes(symbol)) throw new Error(`ArenaSceneBuilder missing ${symbol}`);
}

for (const symbol of ["Validate Asset Readiness", "Create Recommended Asset Folders", "Create Asset Manifest", "CricketAssetManifest", "FindAssets"]) {
  if (!validator.includes(symbol)) throw new Error(`AssetReadinessValidator missing ${symbol}`);
}

for (const symbol of ["CricketAssetManifest", "stadiumPrefab", "batterPrefab", "bowlerPrefab", "HasPlayableCoreAssets"]) {
  if (!manifest.includes(symbol)) throw new Error(`CricketAssetManifest missing ${symbol}`);
}

for (const symbol of ["RuntimeAssetBinder", "ApplyManifest", "PlayBatHit", "PlayWicket", "PlayBoundary"]) {
  if (!binder.includes(symbol)) throw new Error(`RuntimeAssetBinder missing ${symbol}`);
}

for (const symbol of ["PlayerAnimationDirector", "PlayBowling", "PlayShot", "DeliverySpeed", "ShotIntent", "MatchPhase"]) {
  if (!animation.includes(symbol)) throw new Error(`PlayerAnimationDirector missing ${symbol}`);
}

for (const symbol of ["MobilePerformanceManager", "ApplyBattery", "ApplyBalanced", "ApplyPerformance", "targetFrameRate", "shadowDistance"]) {
  if (!performance.includes(symbol)) throw new Error(`MobilePerformanceManager missing ${symbol}`);
}

for (const symbol of ["RuntimeAssetBinder", "PlayBoundary", "PlayWicket"]) {
  if (!impact.includes(symbol)) throw new Error(`ImpactVfxController missing ${symbol}`);
}

for (const symbol of ["Configure Android Mobile Build", "IL2CPP", "ARM64", "ASTC", "antiAliasing"]) {
  if (!mobileBuild.includes(symbol)) throw new Error(`MobileBuildConfigurator missing ${symbol}`);
}

for (const symbol of ["BuildAndroidApk", "BuildAndroidAab", "BuildAndroidFromCommandLine", "BuildPipeline.BuildPlayer", "BuildOptions.StrictMode"]) {
  if (!androidBuild.includes(symbol)) throw new Error(`AndroidBuildPipeline missing ${symbol}`);
}

for (const symbol of ["Record", "PlayLastHighlight", "ReplayEvent"]) {
  if (!replay.includes(symbol)) throw new Error(`ReplayRecorder missing ${symbol}`);
}

console.log("Cricket Arena Unity smoke test passed");
