const crypto = require("crypto");
const http = require("http");

const PORT = Number(process.env.PORT || 8790);
const clients = new Map();
const rooms = new Map();

const deliveries = [
  { name: "Fast yorker", speed: 31, swing: -0.08, bounce: 0.38, difficulty: 0.86 },
  { name: "Outswinger", speed: 26, swing: 0.16, bounce: 0.62, difficulty: 0.72 },
  { name: "Leg cutter", speed: 23, swing: -0.18, bounce: 0.52, difficulty: 0.78 },
  { name: "Slower bouncer", speed: 20, swing: 0.04, bounce: 0.95, difficulty: 0.66 }
];

const server = http.createServer((req, res) => {
  if (req.url === "/health") {
    writeJson(res, 200, { ok: true, clients: clients.size, rooms: rooms.size });
    return;
  }

  if (req.url === "/rooms") {
    writeJson(res, 200, {
      rooms: [...rooms.values()].map((room) => ({
        code: room.code,
        players: room.players.length,
        status: room.status,
        score: room.score,
        wickets: room.wickets,
        balls: room.balls
      }))
    });
    return;
  }

  writeJson(res, 200, { name: "Cricket Arena authoritative server", ws: true });
});

server.on("upgrade", (req, socket) => {
  const key = req.headers["sec-websocket-key"];
  if (!key) {
    socket.destroy();
    return;
  }

  const accept = crypto.createHash("sha1").update(`${key}258EAFA5-E914-47DA-95CA-C5AB0DC85B11`).digest("base64");
  socket.write([
    "HTTP/1.1 101 Switching Protocols",
    "Upgrade: websocket",
    "Connection: Upgrade",
    `Sec-WebSocket-Accept: ${accept}`,
    "",
    ""
  ].join("\r\n"));

  const id = crypto.randomUUID();
  clients.set(socket, { id, roomCode: null, ready: false });
  send(socket, { type: "connected", playerId: id });

  socket.on("data", (buffer) => {
    const message = decodeFrame(buffer);
    if (!message) return;
    handleMessage(socket, safeJson(message));
  });

  socket.on("close", () => leave(socket));
  socket.on("error", () => leave(socket));
});

function handleMessage(socket, message) {
  const client = clients.get(socket);
  if (!client || !message.type) return;

  if (message.type === "join_room") {
    const code = String(message.roomCode || "ARENA-24").toUpperCase().slice(0, 16);
    const room = getRoom(code);
    if (!room.players.includes(socket)) room.players.push(socket);
    client.roomCode = code;
    broadcastRoom(room, { type: "room_state", room: publicRoom(room) });
  }

  if (message.type === "ready") {
    client.ready = Boolean(message.ready);
    const room = currentRoom(client);
    if (!room) return;
    if (room.players.length >= 2 && room.players.every((player) => clients.get(player)?.ready)) {
      room.status = "live";
      queueDelivery(room);
    }
    broadcastRoom(room, { type: "room_state", room: publicRoom(room) });
  }

  if (message.type === "request_delivery") {
    const room = currentRoom(client);
    if (room && room.status === "live") queueDelivery(room);
  }

  if (message.type === "shot") {
    const room = currentRoom(client);
    if (!room || room.status !== "live" || !room.delivery) return;
    const outcome = resolveOutcome(Number(message.timing || 0.5), String(message.intent || "straight"), room.delivery);
    room.score += outcome.runs;
    room.wickets += outcome.wicket ? 1 : 0;
    room.balls += 1;
    room.timeline.push({ ball: room.balls, delivery: room.delivery.name, outcome });
    room.delivery = null;
    if (room.score >= room.target || room.balls >= 6 || room.wickets >= 2) {
      room.status = "finished";
    }
    broadcastRoom(room, { type: "match_state", room: publicRoom(room), outcome });
  }

  if (message.type === "ping") {
    send(socket, { type: "pong", clientTime: message.clientTime || Date.now(), serverTime: Date.now() });
  }
}

function getRoom(code) {
  if (!rooms.has(code)) {
    rooms.set(code, {
      code,
      players: [],
      status: "waiting",
      target: 24,
      score: 0,
      wickets: 0,
      balls: 0,
      delivery: null,
      timeline: []
    });
  }
  return rooms.get(code);
}

function currentRoom(client) {
  return client.roomCode ? rooms.get(client.roomCode) : null;
}

function queueDelivery(room) {
  if (room.status !== "live") return;
  const need = room.target - room.score;
  room.delivery = need <= 6 ? deliveries[0] : deliveries[Math.floor(Math.random() * deliveries.length)];
  broadcastRoom(room, { type: "delivery", delivery: room.delivery, room: publicRoom(room) });
}

function resolveOutcome(timing, intent, delivery) {
  const timingQuality = Math.max(0, 1 - Math.abs(timing - 0.5) * 2.2);
  const intentRisk = intent === "defensive" ? -0.08 : intent === "loftLeft" || intent === "cutRight" ? 0.08 : 0;
  const quality = Math.max(0, timingQuality - (delivery.difficulty - 0.65) * 0.22 - intentRisk);
  if (quality < 0.18) return { runs: 0, wicket: true, message: "Caught behind", quality };
  if (quality < 0.36) return { runs: 0, wicket: false, message: "Beaten", quality };
  if (quality < 0.55) return { runs: 1, wicket: false, message: "Single", quality };
  if (quality < 0.74) return { runs: 2, wicket: false, message: "Two runs", quality };
  if (quality < 0.9) return { runs: 4, wicket: false, message: "Four", quality };
  return { runs: 6, wicket: false, message: "Six", quality };
}

function publicRoom(room) {
  return {
    code: room.code,
    status: room.status,
    target: room.target,
    score: room.score,
    wickets: room.wickets,
    balls: room.balls,
    players: room.players.map((player) => {
      const client = clients.get(player);
      return { id: client.id, ready: client.ready };
    }),
    timeline: room.timeline.slice(-12)
  };
}

function leave(socket) {
  const client = clients.get(socket);
  if (client?.roomCode && rooms.has(client.roomCode)) {
    const room = rooms.get(client.roomCode);
    room.players = room.players.filter((player) => player !== socket);
    if (room.players.length === 0) rooms.delete(room.code);
    else broadcastRoom(room, { type: "room_state", room: publicRoom(room) });
  }
  clients.delete(socket);
}

function broadcastRoom(room, message) {
  room.players.forEach((socket) => send(socket, message));
}

function send(socket, message) {
  if (!socket.destroyed) socket.write(encodeFrame(JSON.stringify(message)));
}

function writeJson(res, status, body) {
  res.writeHead(status, { "content-type": "application/json" });
  res.end(JSON.stringify(body));
}

function safeJson(message) {
  try {
    return JSON.parse(message);
  } catch {
    return {};
  }
}

function decodeFrame(buffer) {
  const second = buffer[1];
  let length = second & 127;
  let offset = 2;
  if (length === 126) {
    length = buffer.readUInt16BE(offset);
    offset += 2;
  } else if (length === 127) {
    length = Number(buffer.readBigUInt64BE(offset));
    offset += 8;
  }
  const mask = buffer.subarray(offset, offset + 4);
  offset += 4;
  const data = buffer.subarray(offset, offset + length);
  const decoded = Buffer.alloc(data.length);
  for (let i = 0; i < data.length; i++) decoded[i] = data[i] ^ mask[i % 4];
  return decoded.toString("utf8");
}

function encodeFrame(message) {
  const payload = Buffer.from(message);
  if (payload.length < 126) {
    return Buffer.concat([Buffer.from([0x81, payload.length]), payload]);
  }

  if (payload.length <= 65535) {
    const header = Buffer.alloc(4);
    header[0] = 0x81;
    header[1] = 126;
    header.writeUInt16BE(payload.length, 2);
    return Buffer.concat([header, payload]);
  }

  const header = Buffer.alloc(10);
  header[0] = 0x81;
  header[1] = 127;
  header.writeBigUInt64BE(BigInt(payload.length), 2);
  return Buffer.concat([header, payload]);
}

if (require.main === module) {
  server.listen(PORT, () => {
    console.log(`Cricket Arena authoritative server on http://localhost:${PORT}`);
  });
}

module.exports = { server, resolveOutcome };
