const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "..");

const requiredFiles = [
  "Packages/manifest.json",
  "ProjectSettings/ProjectVersion.txt",
  "Assets/Scripts/Core/MatchManager.cs",
  "Assets/Scripts/Core/ShotOutcome.cs",
  "Assets/Scripts/Core/ReplayEvent.cs",
  "Assets/Scripts/Core/ReplayRecorder.cs",
  "Assets/Scripts/Gameplay/BattingController.cs",
  "Assets/Scripts/Gameplay/BowlingController.cs",
  "Assets/Scripts/Gameplay/BallPhysicsController.cs",
  "Assets/Scripts/Presentation/CameraDirector.cs",
  "Assets/Scripts/Presentation/MobileHaptics.cs",
  "Assets/Scripts/Presentation/ImpactVfxController.cs",
  "Assets/Scripts/Networking/RealtimeMatchClient.cs",
  "Assets/Scripts/Networking/MatchProtocol.cs",
  "Assets/Scripts/UI/ScoreHudController.cs",
  "Assets/Scripts/UI/MobileControlsController.cs",
  "Assets/Scripts/UI/MultiplayerLobbyController.cs",
  "Assets/Scripts/Editor/ArenaSceneBuilder.cs",
  "Assets/Scripts/Editor/AssetReadinessValidator.cs",
  "Assets/Scripts/Editor/MobileBuildConfigurator.cs"
];

for (const file of requiredFiles) {
  if (!fs.existsSync(path.join(root, file))) {
    throw new Error(`Missing Unity scaffold file: ${file}`);
  }
}

const matchManager = fs.readFileSync(path.join(root, "Assets/Scripts/Core/MatchManager.cs"), "utf8");
const batting = fs.readFileSync(path.join(root, "Assets/Scripts/Gameplay/BattingController.cs"), "utf8");
const ball = fs.readFileSync(path.join(root, "Assets/Scripts/Gameplay/BallPhysicsController.cs"), "utf8");
const network = fs.readFileSync(path.join(root, "Assets/Scripts/Networking/RealtimeMatchClient.cs"), "utf8");
const protocol = fs.readFileSync(path.join(root, "Assets/Scripts/Networking/MatchProtocol.cs"), "utf8");
const lobby = fs.readFileSync(path.join(root, "Assets/Scripts/UI/MultiplayerLobbyController.cs"), "utf8");
const server = fs.readFileSync(path.join(root, "server/authoritative-server.js"), "utf8");
const sceneBuilder = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/ArenaSceneBuilder.cs"), "utf8");
const validator = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/AssetReadinessValidator.cs"), "utf8");
const mobileBuild = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/MobileBuildConfigurator.cs"), "utf8");
const replay = fs.readFileSync(path.join(root, "Assets/Scripts/Core/ReplayRecorder.cs"), "utf8");

for (const symbol of ["MatchPhase", "ApplyOutcome", "OnScoreChanged"]) {
  if (!matchManager.includes(symbol)) throw new Error(`MatchManager missing ${symbol}`);
}

for (const symbol of ["ShotIntent", "PlayShot", "ResolveOutcome", "MobileHaptics"]) {
  if (!batting.includes(symbol)) throw new Error(`BattingController missing ${symbol}`);
}

for (const symbol of ["Launch", "ResolveShot", "Rigidbody"]) {
  if (!ball.includes(symbol)) throw new Error(`BallPhysicsController missing ${symbol}`);
}

for (const symbol of ["ClientWebSocket", "ConnectAsync", "SendAsync", "ReceiveLoop", "Disconnect", "JoinRoom", "SetReady", "RequestDelivery", "SendShot", "ReceiveJson", "LastOutboundJson"]) {
  if (!network.includes(symbol)) throw new Error(`RealtimeMatchClient missing ${symbol}`);
}

for (const symbol of ["MatchMessage", "DeliveryDto", "RoomDto", "MatchEvents", "request_delivery"]) {
  if (!protocol.includes(symbol)) throw new Error(`MatchProtocol missing ${symbol}`);
}

for (const symbol of ["MultiplayerLobbyController", "Ready", "RequestDelivery", "SetShotTiming"]) {
  if (!lobby.includes(symbol)) throw new Error(`MultiplayerLobbyController missing ${symbol}`);
}

for (const symbol of ["join_room", "request_delivery", "resolveOutcome", "match_state"]) {
  if (!server.includes(symbol)) throw new Error(`authoritative-server missing ${symbol}`);
}

for (const symbol of ["MenuItem", "BuildScene", "CreateStadium", "CreatePlayer", "CreateHud", "RealtimeMatchClient", "CreateButton", "RoomCodeInput"]) {
  if (!sceneBuilder.includes(symbol)) throw new Error(`ArenaSceneBuilder missing ${symbol}`);
}

for (const symbol of ["Validate Asset Readiness", "Create Recommended Asset Folders", "FindAssets"]) {
  if (!validator.includes(symbol)) throw new Error(`AssetReadinessValidator missing ${symbol}`);
}

for (const symbol of ["Configure Android Mobile Build", "IL2CPP", "ARM64"]) {
  if (!mobileBuild.includes(symbol)) throw new Error(`MobileBuildConfigurator missing ${symbol}`);
}

for (const symbol of ["Record", "PlayLastHighlight", "ReplayEvent"]) {
  if (!replay.includes(symbol)) throw new Error(`ReplayRecorder missing ${symbol}`);
}

console.log("Cricket Arena Unity smoke test passed");
