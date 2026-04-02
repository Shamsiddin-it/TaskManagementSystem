import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Sun, Moon } from 'lucide-react';
import { useTheme } from '../../context/ThemeContext.jsx';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

export default function WorkerSettings() {
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('General Profile');
  const { theme, setTheme } = useTheme();
  
  // State binding matching the screenshot
  const [settings, setSettings] = useState({
     fullName: "John Doe",
     title: "Senior Frontend Engineer",
     autoFocus: true,
     blockNotifications: true,
     defaultSessionTime: 45
  });

  useEffect(() => {
    // In actual implementation, we'd fetch UserSettingsController
    setLoading(false);
  }, []);

  const handleSave = async () => {
    // Save mutation placeholder
    const token = localStorage.getItem('token');
    try {
      // await fetch(`${API_BASE_URL}/api/usersettings/...`, ...)
    } catch(err){}
  };

  const handleThemeChange = (newTheme) => {
     if (newTheme === theme) return;
     setTheme(newTheme);
  };

  if (loading) return null;

  const isLight = theme === 'light';
  const shellClass = isLight
    ? 'bg-white/90 border-slate-200 shadow-[0_20px_60px_rgba(15,23,42,0.08)]'
    : 'bg-[#0D1117]/80 border-[#1C212B] shadow-2xl';
  const sectionClass = isLight
    ? 'bg-slate-50 border-slate-200'
    : 'bg-[#0A0D14]/50 border-[#1C212B]';
  const headingClass = isLight ? 'text-slate-900' : 'text-white';
  const subtextClass = isLight ? 'text-slate-500' : 'text-gray-400';
  const sidebarIdleClass = isLight
    ? 'text-slate-500 hover:text-slate-900 hover:bg-white'
    : 'text-gray-400 hover:text-gray-200 hover:bg-[#161B22]';
  const sidebarActiveClass = isLight
    ? 'bg-white text-slate-900 font-bold shadow-md'
    : 'bg-[#1C212B] text-white font-bold shadow-md';

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col gap-6 max-w-7xl mx-auto w-full pb-10">
      
      {/* Header */}
      <div className="flex items-center justify-between">
         <div>
            <h1 className={`text-2xl font-bold mb-1 tracking-tight ${headingClass}`}>System Settings</h1>
            <p className={`text-sm ${subtextClass}`}>Manage your profile, preferences, and workspace configuration.</p>
         </div>
         <button onClick={handleSave} className="px-6 py-2.5 bg-purple-600 hover:bg-purple-500 text-white rounded-lg text-sm font-bold transition-all shadow-[0_0_15px_rgba(168,85,247,0.4)]">
            Save Changes
         </button>
      </div>

      <div className={`${shellClass} backdrop-blur-xl border rounded-2xl flex overflow-hidden relative`}>
         
         {/* Inner Navigation Sidebar */}
         <div className={`w-[240px] shrink-0 p-4 flex flex-col gap-2 ${isLight ? 'border-r border-slate-200 bg-slate-50/80' : 'border-r border-[#1C212B]'}`}>
            {[
               'General Profile', 'Focus Mode', 'Notifications', 
               'Integrations', 'Interface & Theme', 'Security'
            ].map(tab => (
               <button 
                  key={tab} 
                  onClick={() => setActiveTab(tab)}
                  className={`w-full text-left px-4 py-3 rounded-xl text-sm transition-all duration-300 ${activeTab === tab ? sidebarActiveClass : sidebarIdleClass}`}
               >
                  {tab}
               </button>
            ))}
         </div>

         {/* Scrollable Form Content */}
         <div className="flex-1 p-10 overflow-y-auto custom-scrollbar h-[700px]">
            
            <div className="max-w-2xl space-y-12 pb-20">

               {/* Light/Dark Mode Toggle - Prominent */}
               <section>
                  <h2 className={`text-lg font-bold mb-1 ${headingClass}`}>Appearance</h2>
                  <p className={`text-sm mb-6 ${subtextClass}`}>Switch between dark and light mode.</p>
                  <div className={`${sectionClass} border rounded-2xl p-6`}>
                     <div className="flex items-center justify-between">
                        <div className="flex items-center gap-4">
                           <div className={`w-10 h-10 rounded-xl flex items-center justify-center transition-all ${theme === 'dark' ? 'bg-purple-500/20 text-purple-400' : 'bg-amber-500/20 text-amber-400'}`}>
                              {theme === 'dark' ? <Moon size={20} /> : <Sun size={20} />}
                           </div>
                           <div>
                              <h4 className={`text-[14px] font-bold ${headingClass}`}>{theme === 'dark' ? 'Dark Mode' : 'Light Mode'}</h4>
                              <p className={`text-xs ${isLight ? 'text-slate-500' : 'text-gray-500'}`}>{theme === 'dark' ? 'Optimized for low-light environments' : 'Bright interface for daytime use'}</p>
                           </div>
                        </div>
                        <div 
                           onClick={() => handleThemeChange(theme === 'dark' ? 'light' : 'dark')}
                           className={`w-14 h-7 rounded-full flex items-center transition-all duration-300 p-1 cursor-pointer ${theme === 'dark' ? 'bg-purple-600 shadow-[0_0_12px_rgba(168,85,247,0.4)]' : 'bg-amber-500 shadow-[0_0_12px_rgba(245,158,11,0.4)]'}`}
                        >
                           <div className={`w-5 h-5 bg-white rounded-full shadow-md transition-all duration-300 flex items-center justify-center ${theme === 'dark' ? 'translate-x-7' : 'translate-x-0'}`}>
                              {theme === 'dark' ? <Moon size={10} className="text-purple-600" /> : <Sun size={10} className="text-amber-500" />}
                           </div>
                        </div>
                     </div>
                  </div>
               </section>
               
               {/* Section: Profile Preferences */}
               <section>
                  <h2 className={`text-lg font-bold mb-1 ${headingClass}`}>Profile Preferences</h2>
                  <p className={`text-sm mb-6 ${subtextClass}`}>Manage how you appear to your team.</p>

                  <div className={`${sectionClass} border rounded-2xl p-6`}>
                     <div className="flex items-center gap-6 mb-8">
                        <div className="w-20 h-20 rounded-full bg-purple-500 shadow-[0_0_20px_rgba(168,85,247,0.4)] flex items-center justify-center text-2xl font-bold text-white">JD</div>
                        <div>
                           <button className="px-4 py-2 border border-[#2D3342] bg-[#161B22] text-gray-300 hover:text-white rounded-lg text-sm font-medium transition-colors mb-2">Upload Photo</button>
                           <p className="text-[11px] text-gray-500 font-medium">JPG or PNG. Max 2MB.</p>
                        </div>
                     </div>

                     <div className="grid grid-cols-2 gap-6">
                        <InputRow 
                           label="Full Name" 
                           value={settings.fullName} 
                           onChange={v => setSettings({...settings, fullName: v})} 
                        />
                        <InputRow 
                           label="Title" 
                           value={settings.title} 
                           onChange={v => setSettings({...settings, title: v})} 
                        />
                     </div>
                  </div>
               </section>

               {/* Section: Focus Mode */}
               <section>
                  <h2 className={`text-lg font-bold mb-1 ${headingClass}`}>Focus Mode Configuration</h2>
                  <p className={`text-sm mb-6 ${subtextClass}`}>Fine-tune your deep work environment.</p>

                  <div className={`${sectionClass} border rounded-2xl p-6 space-y-6`}>
                     <SwitchRow 
                        title="Auto-Activate Focus Mode"
                        desc="Enable automatically during scheduled work hours."
                        checked={settings.autoFocus}
                        onChange={() => setSettings({...settings, autoFocus: !settings.autoFocus})}
                     />
                     <div className="w-full h-[1px] bg-[#1C212B]" />
                     <SwitchRow 
                        title="Block Desktop Notifications"
                        desc="Mute all OS-level alerts when a timer is running."
                        checked={settings.blockNotifications}
                        onChange={() => setSettings({...settings, blockNotifications: !settings.blockNotifications})}
                     />
                     <div className="w-full h-[1px] bg-[#1C212B]" />
                     <div className="w-1/2">
                        <InputRow 
                           label="Default Focus Session (Minutes)" 
                           value={settings.defaultSessionTime} 
                           onChange={v => setSettings({...settings, defaultSessionTime: v})} 
                           type="number"
                        />
                     </div>
                  </div>
               </section>

               {/* Section: External Integrations */}
               <section>
                  <h2 className={`text-lg font-bold mb-1 ${headingClass}`}>External Integrations</h2>
                  <p className={`text-sm mb-6 ${subtextClass}`}>Connect Nexus to your existing development workflow.</p>

                  <div className={`${sectionClass} border rounded-2xl p-6 space-y-4`}>
                     <IntegrationRow 
                        icon={<div className="w-6 h-6 rounded-md bg-white text-black flex items-center justify-center font-bold text-lg leading-none shrink-0"><svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" fill="currentColor"><path d="M12 2C6.477 2 2 6.484 2 12.017c0 4.425 2.865 8.18 6.839 9.504.5.092.682-.217.682-.483 0-.237-.008-.868-.013-1.703-2.782.605-3.369-1.343-3.369-1.343-.454-1.158-1.11-1.466-1.11-1.466-.908-.62.069-.608.069-.608 1.003.07 1.531 1.032 1.531 1.032.892 1.53 2.341 1.088 2.91.832.092-.647.35-1.088.636-1.338-2.22-.253-4.555-1.113-4.555-4.951 0-1.093.39-1.988 1.029-2.688-.103-.253-.446-1.272.098-2.65 0 0 .84-.27 2.75 1.026A9.564 9.564 0 0112 6.844c.85.004 1.705.115 2.504.337 1.909-1.296 2.747-1.027 2.747-1.027.546 1.379.202 2.398.1 2.651.64.7 1.028 1.595 1.028 2.688 0 3.848-2.339 4.695-4.566 4.943.359.309.678.92.678 1.855 0 1.338-.012 2.419-.012 2.747 0 .268.18.58.688.482A10.019 10.019 0 0022 12.017C22 6.484 17.522 2 12 2z"></path></svg></div>}
                        title="GitHub"
                        desc="Sync PRs and commit activity."
                        status="Connected"
                     />
                     <IntegrationRow 
                        icon={<div className="w-6 h-6 rounded-md bg-[#635BFF] flex items-center justify-center font-bold text-white shrink-0">S</div>}
                        title="Stripe"
                        desc="Monitor payment gateway events."
                        status="Connect"
                     />
                  </div>
               </section>

               {/* Section: Theme */}
               <section>
                  <h2 className={`text-lg font-bold mb-1 ${headingClass}`}>Theme Customization</h2>
                  <p className={`text-sm mb-6 ${subtextClass}`}>Select the visual appearance of your workspace.</p>

                  <div className="grid grid-cols-3 gap-4">
                     <ThemeCard mode="dark" active={theme === 'dark'} onClick={() => handleThemeChange('dark')} label="Dark Mode" />
                     <ThemeCard mode="light" active={theme === 'light'} onClick={() => handleThemeChange('light')} label="Light Mode" />
                     <ThemeCard mode="system" active={theme === 'system'} onClick={() => handleThemeChange('system')} label="System" />
                  </div>
               </section>

            </div>
         </div>
      </div>
    </motion.div>
  );
}

// Sub components matching the precise aesthetic form inputs
function InputRow({ label, value, onChange, type="text" }) {
   return (
      <div className="flex flex-col gap-2">
         <span className="text-[12px] font-bold text-gray-400">{label}</span>
         <input 
            type={type}
            value={value}
            onChange={e => onChange(e.target.value)}
            className="w-full bg-[#0D1117] border border-[#1C212B] focus:border-purple-500/60 rounded-xl px-4 py-2.5 text-sm text-white outline-none transition-colors shadow-inner"
         />
      </div>
   );
}

function SwitchRow({ title, desc, checked, onChange }) {
   return (
      <div className="flex justify-between items-center cursor-pointer" onClick={onChange}>
         <div>
            <h4 className="text-[14px] font-bold text-white mb-0.5">{title}</h4>
            <p className="text-xs text-gray-500">{desc}</p>
         </div>
         {/* Custom CSS Toggle Switch imitating the purple active state */}
         <div className={`w-12 h-6 rounded-full flex items-center transition-all duration-300 p-1 ${checked ? 'bg-purple-600' : 'bg-[#1C212B]'}`}>
            <div className={`w-4 h-4 bg-white rounded-full shadow-md transition-all duration-300 ${checked ? 'translate-x-6' : 'translate-x-0'}`} />
         </div>
      </div>
   );
}

function IntegrationRow({ icon, title, desc, status }) {
   const isConnected = status === 'Connected';
   return (
      <div className="flex items-center justify-between p-4 bg-[#0D1117]/80 border border-[#1C212B] rounded-xl hover:border-[#2D3342] transition-colors">
         <div className="flex items-center gap-4">
            {icon}
            <div>
               <h4 className="text-[14px] font-bold text-white">{title}</h4>
               <p className="text-xs text-gray-500 mt-0.5">{desc}</p>
            </div>
         </div>
         <button className={`px-4 py-1.5 rounded-lg text-xs font-bold transition-all border ${isConnected ? 'bg-transparent border-[#2D3342] text-gray-400 hover:text-white' : 'bg-purple-600/10 border-purple-500/30 text-purple-400 hover:bg-purple-500/20'}`}>
            {status}
         </button>
      </div>
   );
}

function ThemeCard({ mode, active, onClick, label }) {
   let bg = "bg-[#0A0D14]";
   if (mode === 'light') bg = "bg-[#f8fafc]";
   if (mode === 'system') bg = "bg-gradient-to-br from-[#0A0D14] to-[#f8fafc]"; // diagonal split effectively

   return (
      <button 
         onClick={onClick}
         className={`p-4 rounded-xl border flex flex-col items-center gap-4 transition-all ${active ? 'border-purple-500 bg-purple-500/5 shadow-[0_0_15px_rgba(168,85,247,0.2)]' : 'border-[#1C212B] bg-[#0A0D14]/50 hover:border-[#2D3342]'}`}
      >
         <div className={`w-full aspect-video rounded-lg border border-[#1C212B] ${bg} shadow-md overflow-hidden flex flex-col relative`}>
            {/* Fake UI block rendering inside to look like theme mock */}
            <div className="w-full h-1/4 border-b border-[#1C212B]/30" />
            <div className="flex-1 w-1/3 border-r border-[#1C212B]/30" />
            {mode === 'system' && <div className="absolute inset-0 bg-transparent flex items-center justify-center text-[10px] font-bold text-gray-500 tracking-widest uppercase">Auto</div>}
         </div>
         <span className={`text-sm font-bold ${active ? 'text-purple-400' : 'text-gray-400'}`}>{label}</span>
      </button>
   );
}
