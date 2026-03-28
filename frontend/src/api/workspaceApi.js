// workspaceApi.js — All fetch helpers for the Worker/Workspace section
// Mirrors the Blazor service layer. All endpoints return { date: T } from backend.

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

function getToken() {
  return localStorage.getItem('token') || '';
}

function authHeaders() {
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${getToken()}`,
  };
}

async function apiFetch(path, options = {}) {
  const res = await fetch(`${API_BASE}/${path}`, {
    headers: authHeaders(),
    ...options,
  });
  if (!res.ok) return null;
  const body = await res.json();
  // Backend wraps data in { date: T } for list/detail endpoints
  if (body && 'date' in body) return body.date;
  return body; // some endpoints return flat objects
}

// ─── Focus Sessions ──────────────────────────────────────────────────────────
export async function getFocusSessions() {
  return (await apiFetch('api/FocusSession')) ?? [];
}

export async function getFocusSessionById(id) {
  return apiFetch(`api/FocusSession/${id}`);
}

export async function pauseFocusSession(id) {
  return apiFetch(`api/FocusSession/${id}/pause`, { method: 'PATCH' });
}

export async function completeFocusSession(id) {
  return apiFetch(`api/FocusSession/${id}/complete`, { method: 'PATCH' });
}

// ─── Day Summaries ───────────────────────────────────────────────────────────
export async function getDaySummaries() {
  return (await apiFetch('api/DaySummary')) ?? [];
}

export async function addDaySummary(dto) {
  return apiFetch('api/DaySummary', {
    method: 'POST',
    body: JSON.stringify(dto),
  });
}

export async function updateDaySummary(id, dto) {
  return apiFetch(`api/DaySummary/${id}`, {
    method: 'PUT',
    body: JSON.stringify(dto),
  });
}

// ─── Notifications ───────────────────────────────────────────────────────────
export async function getWorkerNotifications() {
  return (await apiFetch('api/Notification')) ?? [];
}

export async function markNotificationRead(id) {
  return apiFetch(`api/Notification/${id}/read`, { method: 'PATCH' });
}

// ─── Schedule Events ─────────────────────────────────────────────────────────
export async function getScheduleEvents() {
  return (await apiFetch('api/ScheduleEvent')) ?? [];
}

export async function addScheduleEvent(dto) {
  return apiFetch('api/ScheduleEvent', {
    method: 'POST',
    body: JSON.stringify(dto),
  });
}

export async function deleteScheduleEvent(id) {
  return fetch(`${API_BASE}/api/ScheduleEvent/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  }).then(r => r.ok);
}

// ─── User Settings ────────────────────────────────────────────────────────────
export async function getUserSettings(userId) {
  return apiFetch(`api/UserSettings/user/${userId}`);
}

export async function addUserSettings(dto) {
  return apiFetch('api/UserSettings', {
    method: 'POST',
    body: JSON.stringify(dto),
  });
}

export async function updateUserSettings(id, dto) {
  return apiFetch(`api/UserSettings/${id}`, {
    method: 'PUT',
    body: JSON.stringify(dto),
  });
}

// ─── Helper ───────────────────────────────────────────────────────────────────
export function getCurrentUserId() {
  const user = localStorage.getItem('user');
  if (!user) return null;
  try {
    const parsed = JSON.parse(user);
    return parsed.userId ?? null;
  } catch {
    return null;
  }
}

export function doLogout() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  localStorage.removeItem('nexus_role');
  window.location.href = '/login';
}
