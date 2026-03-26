const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5125";

async function request(path, options = {}) {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options
  });

  if (res.status === 204) {
    return { ok: true, data: null, raw: null };
  }

  const raw = await res.json().catch(() => null);
  const ok = res.ok;
  const data = raw?.Data ?? null;
  const message = raw?.Description?.join(" ") || raw?.Description || null;

  return { ok, data, raw, message, status: res.status };
}

export const api = {
  get: (path) => request(path),
  post: (path, body) =>
    request(path, { method: "POST", body: JSON.stringify(body) }),
  put: (path, body) =>
    request(path, { method: "PUT", body: JSON.stringify(body) }),
  patch: (path, body) =>
    request(path, {
      method: "PATCH",
      body: body === undefined ? undefined : JSON.stringify(body)
    }),
  delete: (path) => request(path, { method: "DELETE" })
};
