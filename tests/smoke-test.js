const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "..");

const requiredFiles = [
  "Packages/manifest.json",
  "ProjectSettings/ProjectVersion.txt",
  "Assets/Scripts/Core/MatchManager.cs",
  "Assets/Scripts/Core/ShotOutcome.cs",
  "Assets/Scripts/Gameplay/BattingController.cs",
  "Assets/Scripts/Gameplay/BowlingController.cs",
  "Assets/Scripts/Gameplay/BallPhysicsController.cs",
  "Assets/Scripts/Presentation/CameraDirector.cs",
  "Assets/Scripts/Presentation/MobileHaptics.cs",
  "Assets/Scripts/Networking/RealtimeMatchClient.cs",
  "Assets/Scripts/UI/ScoreHudController.cs",
  "Assets/Scripts/UI/MobileControlsController.cs",
  "Assets/Scripts/Editor/ArenaSceneBuilder.cs"
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
const sceneBuilder = fs.readFileSync(path.join(root, "Assets/Scripts/Editor/ArenaSceneBuilder.cs"), "utf8");

for (const symbol of ["MatchPhase", "ApplyOutcome", "OnScoreChanged"]) {
  if (!matchManager.includes(symbol)) throw new Error(`MatchManager missing ${symbol}`);
}

for (const symbol of ["ShotIntent", "PlayShot", "ResolveOutcome", "MobileHaptics"]) {
  if (!batting.includes(symbol)) throw new Error(`BattingController missing ${symbol}`);
}

for (const symbol of ["Launch", "ResolveShot", "Rigidbody"]) {
  if (!ball.includes(symbol)) throw new Error(`BallPhysicsController missing ${symbol}`);
}

for (const symbol of ["Connect", "JoinRoom", "SendShot"]) {
  if (!network.includes(symbol)) throw new Error(`RealtimeMatchClient missing ${symbol}`);
}

for (const symbol of ["MenuItem", "BuildScene", "CreateStadium", "CreatePlayer", "CreateHud"]) {
  if (!sceneBuilder.includes(symbol)) throw new Error(`ArenaSceneBuilder missing ${symbol}`);
}

console.log("Cricket Arena Unity smoke test passed");
