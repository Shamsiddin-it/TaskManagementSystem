import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { getUserSettings, addUserSettings, updateUserSettings } from '../../api/workspaceApi.js';

const ACCENT_OPTIONS = ['#7c3aed', '#a78bfa', '#3fb950', '#38bdf8', '#f85149', '#f59e0b'];

function getUserId() {
  try { return JSON.parse(localStorage.getItem('user') || '{}').userId || 0; } catch { return 0; }
}

export default function WorkerSettingsPage() {
  const navigate = useNavigate();
  const [theme, setTheme] = useState(() => localStorage.getItem('nexus_theme') || 'dark');
  const [accent, setAccent] = useState('#7c3aed');
  const [settingsId, setSettingsId] = useState(null);
  const [autoFocus, setAutoFocus] = useState(false);
  const [blockNotif, setBlockNotif] = useState(false);
  const [focusDuration, setFocusDuration] = useState(25);
  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    const userId = getUserId();
    if (!userId) return;
    getUserSettings(userId).then(s => {
      if (s) {
        setSettingsId(s.id);
        setAutoFocus(s.autoFocusEnabled ?? false);
        setBlockNotif(s.blockNotificationsDuringFocus ?? false);
        setFocusDuration(s.defaultFocusDurationMinutes ?? 25);
      }
    });
  }, []);

  function applyTheme(t) {
    setTheme(t);
    localStorage.setItem('nexus_theme', t);
    if (t === 'light') {
      document.documentElement.setAttribute('data-theme', 'light');
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
  }

  async function handleSave() {
    const userId = getUserId();
    if (!userId) return;
    setSaving(true);
    const dto = {
      autoFocusEnabled: autoFocus,
      blockNotificationsDuringFocus: blockNotif,
      defaultFocusDurationMinutes: focusDuration,
    };
    if (settingsId) {
      await updateUserSettings(settingsId, dto);
    } else {
      await addUserSettings({ userId, ...dto });
      const fresh = await getUserSettings(userId);
      if (fresh) setSettingsId(fresh.id);
    }
    setSaving(false);
    setSaved(true);
    setTimeout(() => setSaved(false), 2000);
  }

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold text-white">System settings</h2>
        <button
          onClick={handleSave}
          disabled={saving}
          className="bg-purple-600 hover:bg-purple-500 disabled:opacity-50 text-white px-5 py-2 rounded-xl text-sm font-medium transition-all"
        >
          {saved ? '✓ Saved!' : saving ? 'Saving…' : 'Save changes'}
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-[200px_1fr] gap-6">
        {/* Nav */}
        <nav className="flex flex-col gap-1">
          {['General profile', 'Focus mode', 'Interface & theme', 'Notifications'].map(item => (
            <button
              key={item}
              onClick={() => item === 'Notifications' && navigate('/workspace/notifications')}
              className="text-left px-4 py-2.5 rounded-xl text-sm text-gray-400 hover:text-white hover:bg-white/5 transition-all"
            >
              {item}
            </button>
          ))}
        </nav>

        {/* Body */}
        <div className="flex flex-col gap-8">
          {/* Theme section */}
          <section className="glass-panel rounded-2xl p-6">
            <h3 className="text-base font-semibold text-white mb-4">Interface &amp; theme</h3>
            <div className="flex gap-3 mb-6">
              {[['dark', 'Black mode'], ['light', 'White mode']].map(([t, label]) => (
                <button
                  key={t}
                  onClick={() => applyTheme(t)}
                  className={`flex-1 py-2.5 rounded-xl text-sm border transition-all ${
                    theme === t
                      ? 'bg-purple-600/20 border-purple-500/40 text-white font-medium'
                      : 'border-[#30363D] text-gray-400 hover:border-gray-500'
                  }`}
                >
                  {label}
                </button>
              ))}
            </div>
            <p className="text-xs text-gray-500 mb-3">Accent color</p>
            <div className="flex gap-3 flex-wrap">
              {ACCENT_OPTIONS.map(hex => (
                <button
                  key={hex}
                  onClick={() => setAccent(hex)}
                  style={{ backgroundColor: hex }}
                  className={`w-8 h-8 rounded-full transition-all ${
                    accent === hex ? 'ring-2 ring-white ring-offset-2 ring-offset-[#0D1117] scale-110' : 'opacity-70 hover:opacity-100'
                  }`}
                />
              ))}
            </div>
          </section>

          {/* Focus section */}
          <section className="glass-panel rounded-2xl p-6">
            <h3 className="text-base font-semibold text-white mb-4">Focus mode</h3>
            <div className="flex flex-col gap-4">
              <label className="flex items-center gap-3 cursor-pointer">
                <input
                  type="checkbox"
                  className="w-4 h-4 accent-purple-500"
                  checked={autoFocus}
                  onChange={e => setAutoFocus(e.target.checked)}
                />
                <span className="text-sm text-gray-300">Auto-activate focus mode</span>
              </label>
              <label className="flex items-center gap-3 cursor-pointer">
                <input
                  type="checkbox"
                  className="w-4 h-4 accent-purple-500"
                  checked={blockNotif}
                  onChange={e => setBlockNotif(e.target.checked)}
                />
                <span className="text-sm text-gray-300">Block desktop notifications</span>
              </label>
              <label className="flex flex-col gap-1 max-w-xs">
                <span className="text-xs text-gray-400">Default session (minutes)</span>
                <input
                  type="number"
                  min={5}
                  max={120}
                  className="bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
                  value={focusDuration}
                  onChange={e => setFocusDuration(Number(e.target.value))}
                />
              </label>
            </div>
          </section>
        </div>
      </div>
    </motion.div>
  );
}
