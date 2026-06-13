const http = require("http");
const { server, resolveOutcome, sanitizeRoomCode } = require("./authoritative-server");

const PORT = 8791;

server.listen(PORT, async () => {
  try {
    const health = await getJson(`http://localhost:${PORT}/health`);
    if (!health.ok) throw new Error("Health endpoint did not return ok.");

    const metrics = await getJson(`http://localhost:${PORT}/metrics`);
    if (typeof metrics.startedAt !== "number") throw new Error("Metrics endpoint missing startedAt.");

    if (sanitizeRoomCode("team one!!") !== "TEAMONE") throw new Error("Room code sanitizer failed.");

    const boundary = resolveOutcome(0.5, "straight", { difficulty: 0.66 });
    if (boundary.runs < 4) throw new Error("Perfect timing should produce a boundary-style outcome.");

    const miss = resolveOutcome(0.02, "loftLeft", { difficulty: 0.9 });
    if (miss.runs > 1 && !miss.wicket) throw new Error("Poor timing should be low value or wicket.");

    console.log("Authoritative server smoke test passed");
  } finally {
    server.close();
  }
});

function getJson(url) {
  return new Promise((resolve, reject) => {
    http.get(url, (res) => {
      let data = "";
      res.on("data", (chunk) => data += chunk);
      res.on("end", () => resolve(JSON.parse(data)));
    }).on("error", reject);
  });
}
