const crypto = require("crypto");
const net = require("net");
const { server } = require("./authoritative-server");

const PORT = 8792;

server.listen(PORT, async () => {
  const sockets = [];
  try {
    const batter = await connectClient(PORT);
    const bowler = await connectClient(PORT);
    sockets.push(batter, bowler);

    batter.send({ type: "join_room", roomCode: "TEST-ROOM" });
    await batter.waitFor("room_state");
    bowler.send({ type: "join_room", roomCode: "TEST-ROOM" });
    await bowler.waitFor("room_state");
    batter.send({ type: "ready", ready: true });
    await batter.waitFor("room_state");
    bowler.send({ type: "ready", ready: true });

    const delivery = await batter.waitFor("delivery");
    if (!delivery.delivery || !delivery.room) throw new Error("Delivery payload missing data.");

    batter.send({ type: "shot", timing: 0.5, intent: "straight" });
    const state = await batter.waitFor("match_state");
    if (!state.room || state.room.balls !== 1) throw new Error("Match state did not advance after shot.");
    if (!state.outcome) throw new Error("Shot outcome missing from match_state.");

    console.log("Authoritative protocol smoke test passed");
  } finally {
    sockets.forEach((client) => client.close());
    server.close();
  }
});

function connectClient(port) {
  return new Promise((resolve, reject) => {
    const socket = net.connect(port, "127.0.0.1");
    const key = crypto.randomBytes(16).toString("base64");
    const queue = [];
    const waiters = [];
    let handshake = "";
    let upgraded = false;

    socket.on("connect", () => {
      socket.write([
        "GET / HTTP/1.1",
        "Host: localhost",
        "Upgrade: websocket",
        "Connection: Upgrade",
        `Sec-WebSocket-Key: ${key}`,
        "Sec-WebSocket-Version: 13",
        "",
        ""
      ].join("\r\n"));
    });

    socket.on("data", (buffer) => {
      if (!upgraded) {
        handshake += buffer.toString("utf8");
        if (!handshake.includes("\r\n\r\n")) return;
        upgraded = true;
        resolve(client);
        return;
      }

      const messages = decodeFrames(buffer);
      messages.forEach((message) => queue.push(JSON.parse(message)));
      flush();
    });

    socket.on("error", reject);

    const client = {
      send(payload) {
        socket.write(encodeMaskedFrame(JSON.stringify(payload)));
      },
      waitFor(type) {
        return new Promise((resolveWait, rejectWait) => {
          const existingIndex = queue.findIndex((message) => message.type === type);
          if (existingIndex >= 0) {
            resolveWait(queue.splice(existingIndex, 1)[0]);
            return;
          }

          const timeout = setTimeout(() => {
            rejectWait(new Error(`Timed out waiting for ${type}`));
          }, 2500);
          waiters.push({ type, resolve: resolveWait, timeout });
        });
      },
      close() {
        socket.destroy();
      }
    };

    function flush() {
      for (let i = waiters.length - 1; i >= 0; i--) {
        const waiter = waiters[i];
        const messageIndex = queue.findIndex((message) => message.type === waiter.type);
        if (messageIndex >= 0) {
          clearTimeout(waiter.timeout);
          waiter.resolve(queue.splice(messageIndex, 1)[0]);
          waiters.splice(i, 1);
        }
      }
    }
  });
}

function decodeFrames(buffer) {
  const messages = [];
  let cursor = 0;

  while (cursor < buffer.length) {
    let length = buffer[cursor + 1] & 127;
    let offset = cursor + 2;
    if (length === 126) {
      length = buffer.readUInt16BE(offset);
      offset += 2;
    } else if (length === 127) {
      length = Number(buffer.readBigUInt64BE(offset));
      offset += 8;
    }

    messages.push(buffer.subarray(offset, offset + length).toString("utf8"));
    cursor = offset + length;
  }

  return messages;
}

function encodeMaskedFrame(message) {
  const payload = Buffer.from(message);
  const mask = crypto.randomBytes(4);
  const header = payload.length < 126 ? Buffer.from([0x81, payload.length | 0x80]) : Buffer.alloc(4);

  if (payload.length >= 126) {
    header[0] = 0x81;
    header[1] = 126 | 0x80;
    header.writeUInt16BE(payload.length, 2);
  }

  const masked = Buffer.alloc(payload.length);
  for (let i = 0; i < payload.length; i++) masked[i] = payload[i] ^ mask[i % 4];
  return Buffer.concat([header, mask, masked]);
}
