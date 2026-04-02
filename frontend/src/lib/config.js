// Dynamic config — pulls real user identity from localStorage instead of hardcoded integers.
function getUser() {
  try {
    const raw = localStorage.getItem("user");
    return raw ? JSON.parse(raw) : {};
  } catch { return {}; }
}

export const TEAM_ID = (() => {
  return localStorage.getItem("activeTeamId") || getUser().teamId || null;
})();

export const ACTOR_ID = (() => {
  return getUser().userId || getUser().id || null;
})();
