(function () {
  const root = document.getElementById("note-chat");
  if (!root) return;

  const form = document.getElementById("note-chat-form");
  const input = document.getElementById("note-chat-input");
  const messagesEl = document.getElementById("note-chat-messages");
  const statusEl = document.getElementById("note-chat-status");
  const emptyEl = document.getElementById("note-chat-empty");
  const contextEl = document.getElementById("note-chat-context");

  const apiBase = (root.dataset.apiBase || "").replace(/\/$/, "");
  const apiPath = root.dataset.apiPath || "/api/chat";
  const noteId = Number(root.dataset.noteId || 0);
  const noteTitle = root.dataset.noteTitle || "";
  const noteContext = contextEl ? contextEl.value : "";
  const saveUrl = root.dataset.saveUrl || "";

  const storageKey = "cc-note-chat-" + noteId;
  let messages = [];
  try {
    messages = JSON.parse(localStorage.getItem(storageKey) || "[]");
  } catch {
    messages = [];
  }

  function appendBubble(role, content) {
    if (emptyEl) emptyEl.remove();
    const div = document.createElement("div");
    div.className =
      "rounded-xl px-3 py-2 " +
      (role === "user" ? "bg-purple-500/30 ml-6" : "bg-white/10 mr-6");
    div.innerHTML =
      '<span class="mb-1 block text-[10px] uppercase tracking-wide text-white/50">' +
      role +
      "</span>" +
      escapeHtml(content);
    messagesEl.appendChild(div);
    messagesEl.scrollTop = messagesEl.scrollHeight;
  }

  function escapeHtml(text) {
    return String(text)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  function persistLocal() {
    localStorage.setItem(storageKey, JSON.stringify(messages.slice(-20)));
  }

  async function saveServer(role, content) {
    if (!saveUrl) return;
    try {
      const body = new URLSearchParams({
        noteId: String(noteId),
        role,
        content,
      });
      await fetch(saveUrl, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body,
      });
    } catch {
      /* ignore */
    }
  }

  // paint stored (if DOM was empty of server history, still show local extras — skip if server already rendered)
  if (!messagesEl.querySelector(".rounded-xl") && messages.length) {
    messages.forEach((m) => appendBubble(m.role, m.content));
  }

  form.addEventListener("submit", async (e) => {
    e.preventDefault();
    const text = (input.value || "").trim();
    if (!text) return;

    input.value = "";
    statusEl.textContent = "Думаю...";
    appendBubble("user", text);
    messages.push({ role: "user", content: text });
    persistLocal();
    saveServer("user", text);

    try {
      const res = await fetch(apiBase + apiPath, {
        method: "POST",
        headers: { "Content-Type": "application/json", Accept: "application/json" },
        body: JSON.stringify({
          noteId,
          noteTitle,
          noteContext,
          messages: messages.slice(-12),
        }),
      });
      if (!res.ok) throw new Error("HTTP " + res.status);
      const data = await res.json();
      const reply = data.reply || data.Reply || "Пустой ответ.";
      appendBubble("assistant", reply);
      messages.push({ role: "assistant", content: reply });
      persistLocal();
      saveServer("assistant", reply);
      statusEl.textContent = "";
    } catch (err) {
      statusEl.textContent =
        "Не удалось связаться с AI API. Проверьте, что CodeCareer.AiChat запущен (" +
        apiBase +
        ").";
      console.error(err);
    }
  });
})();
