// Simple theme + accent controller for Nexus UI.
// Stores selection in localStorage and updates CSS variables on <html>.
(function () {
  function hexToRgb(hex) {
    if (!hex) return { r: 124, g: 58, b: 237 }; // default purple
    const value = hex.replace('#', '').trim();
    if (value.length === 3) {
      const r = parseInt(value[0] + value[0], 16);
      const g = parseInt(value[1] + value[1], 16);
      const b = parseInt(value[2] + value[2], 16);
      return { r, g, b };
    }
    if (value.length === 6) {
      return {
        r: parseInt(value.slice(0, 2), 16),
        g: parseInt(value.slice(2, 4), 16),
        b: parseInt(value.slice(4, 6), 16)
      };
    }
    return { r: 124, g: 58, b: 237 };
  }

  function setAccent(hex) {
    const rgb = hexToRgb(hex);
    const root = document.documentElement;
    root.style.setProperty('--nx-accent', hex);
    root.style.setProperty('--nx-accent-glow', hex);
    root.style.setProperty('--nx-accent-soft', `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, 0.22)`);
  }

  function applyTheme(theme) {
    // theme: "dark" | "light"
    const html = document.documentElement;
    html.setAttribute('data-nx-theme', theme === 'light' ? 'light' : 'dark');
  }

  function init() {
    const storedTheme = localStorage.getItem('nx_theme');
    const storedAccent = localStorage.getItem('nx_accent');

    const theme = storedTheme === 'light' ? 'light' : 'dark';
    const accent = storedAccent || '#7c3aed';

    applyTheme(theme);
    setAccent(accent);
  }

  window.nexusTheme = {
    init,
    getTheme: function () {
      const storedTheme = localStorage.getItem('nx_theme');
      return storedTheme === 'light' ? 'light' : 'dark';
    },
    getAccent: function () {
      return localStorage.getItem('nx_accent') || '#7c3aed';
    },
    apply: function (theme, accent) {
      applyTheme(theme);
      if (accent) setAccent(accent);
      localStorage.setItem('nx_theme', theme === 'light' ? 'light' : 'dark');
      if (accent) localStorage.setItem('nx_accent', accent);
    },
  };

  window.nexusUi = {
    scrollToId: function (id) {
      try {
        const el = document.getElementById(id);
        if (!el) return;
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      } catch {
        // ignore
      }
    }
  };

  // Ensure variables are applied before the UI paints.
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();

