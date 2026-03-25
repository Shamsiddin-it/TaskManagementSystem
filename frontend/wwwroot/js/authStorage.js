window.nexusAuthStorage = {
  getToken: () => localStorage.getItem('nexus_token'),
  setToken: (token) => localStorage.setItem('nexus_token', token),
  getRole: () => localStorage.getItem('nexus_role'),
  setRole: (role) => localStorage.setItem('nexus_role', role),
  clearToken: () => {
    localStorage.removeItem('nexus_token');
    localStorage.removeItem('nexus_role');
  }
};
