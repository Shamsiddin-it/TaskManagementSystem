import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { useTheme } from '../../context/ThemeContext.jsx';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

function applyTheme(theme) {
  if (theme === "light") {
    document.documentElement.setAttribute("data-theme", "light");
  } else {
    document.documentElement.removeAttribute("data-theme");
  }
}

export default function EmployerSettingsView() {
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const { theme, toggleTheme } = useTheme();
  const [settings, setSettings] = useState(null);

  useEffect(() => {
    // Theme is managed globally by ThemeContext
  }, [theme]);

  // Load actual backend details
  useEffect(() => {
    async function loadSettings() {
      const token = localStorage.getItem('token');
      try {
        const response = await fetch(`${API_BASE_URL}/api/workspace/settings`, {
          headers: { 'Authorization': `Bearer ${token}` }
        });
        if (response.ok) {
           const data = await response.json();
           setSettings({
             profile: data.profile || {},
             teamDefaults: data.teamDefaults || {},
             billing: data.billing || {},
             security: data.security || {},
             integrations: data.integrations || []
           });
        }
      } catch (err) {
        console.error("Settings load failed", err);
      } finally {
        setLoading(false);
      }
    }
    loadSettings();
  }, []);

  const toggleThemeLocal = () => {
    toggleTheme();
  };

  const handleChange = (section, field, value) => {
    setSettings(prev => ({
       ...prev,
       [section]: {
          ...prev[section],
          [field]: value
       }
    }));
  };

  const handleSave = async () => {
     setSaving(true);
     const token = localStorage.getItem('token');
     try {
       await fetch(`${API_BASE_URL}/api/workspace/settings`, {
         method: 'PUT',
         headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
         body: JSON.stringify(settings)
       });
     } catch(err) { }
     setSaving(false);
  };

  const handleAction = async (endpoint) => {
     const token = localStorage.getItem('token');
     try {
        await fetch(`${API_BASE_URL}/api/workspace/settings/actions/${endpoint}`, {
           method: 'POST',
           headers: { 'Authorization': `Bearer ${token}` }
        });
     } catch (err) { }
  };

  if (loading) {
     return (
        <div className="flex h-full items-center justify-center">
          <div className="w-10 h-10 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin" />
        </div>
     );
  }

  // Ensure robust defaults if backend is totally empty
  const profile = settings?.profile || {
     organizationName: 'TechCorp Industries',
     organizationCode: 'ORG 7392',
     primaryContactName: 'Sarah Chen',
     contactEmailAddress: 'sarah.chen@techcorp.io',
     companyWebsite: 'https://techcorp.industries',
     industry: 'Technology',
     companySize: '250-500 employees',
     planName: 'Enterprise Plan'
  };

  const team = settings?.teamDefaults || {
     defaultTeamSizeLimit: 25,
     defaultPtoPolicy: '20 days/year',
     defaultWorkSchedule: 'Mon-Fri 9AM-5PM',
     primaryTimezone: 'America/New_York',
     autoProvisionNewHires: true,
     requireManagerApprovalForTimeOff: true
  };

  const billing = settings?.billing || {
     planName: 'Enterprise',
     planPriceMonthly: 499,
     teamMembersUsed: 347,
     teamMembersLimit: 500,
     activeProjectsUsed: 12,
     activeProjectsLimit: 20,
     nextBillingDate: 'March 15, 2025',
     paymentMethodLast4: '4242',
     billingEmail: 'accounting@techcorp.io',
     taxIdOrVatNumber: 'US123456789'
  };

  const security = settings?.security || {
     requireTwoFactorAuthentication: true,
     enforceIpAllowlist: false,
     dataEncryptionAtRest: true,
     idleSessionTimeout: '30 Minutes',
     auditLogRetention: '90 Days',
     ssoProviderName: 'Okta',
     ssoConnected: true
  };

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="max-w-4xl mx-auto space-y-8 pb-12">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Settings</h1>
          <p className="text-gray-400 text-sm">Manage your organization preferences</p>
        </div>
        <div className="flex gap-3">
          <button className="px-5 py-2.5 bg-[#1C212B] hover:bg-[#2D3342] text-white border border-[#2D3342] rounded-lg text-sm font-medium transition-colors">
            Discard Changes
          </button>
          <button disabled={saving} onClick={handleSave} className="px-5 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)] text-white font-medium text-sm rounded-lg transition-all disabled:opacity-50">
            {saving ? 'Saving...' : 'Save Configuration'}
          </button>
        </div>
      </div>

      <div className="space-y-6">

         {/* Appearance Panel */}
         <TerminalPanel header="~/nexus/org/appearance">
            <div className="flex items-center justify-between p-6 rounded-xl border border-purple-500/20 bg-purple-500/5 hover:border-purple-500/50 transition-colors">
              <div className="flex items-center gap-4">
                 <div className="w-12 h-12 rounded-xl flex items-center justify-center bg-gradient-to-br from-purple-500 to-indigo-600 shadow-[0_0_15px_rgba(168,85,247,0.3)]">
                    {theme === "dark" ? (
                       <svg className="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" /></svg>
                    ) : (
                       <svg className="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" /></svg>
                    )}
                 </div>
                 <div>
                    <h3 className="text-white font-semibold text-sm mb-1">{theme === "dark" ? "Dark Theme" : "Light Theme"}</h3>
                    <p className="text-xs text-gray-500">{theme === "dark" ? "Enhanced high-contrast glowing elements." : "Crisp, bright workspace orientation."}</p>
                 </div>
              </div>
              <button
                onClick={toggleThemeLocal}
                className="relative w-14 h-7 rounded-full transition-colors focus:outline-none bg-purple-500/20 border border-purple-500/50"
              >
                <div
                  className="absolute top-1 w-5 h-5 rounded-full bg-purple-400 shadow-[0_0_10px_rgba(168,85,247,0.8)] transition-transform duration-300"
                  style={{ transform: theme === "light" ? "translateX(28px)" : "translateX(4px)" }}
                />
              </button>
            </div>
         </TerminalPanel>

         {/* Profile Panel */}
         <TerminalPanel header="~/nexus/org/profile">
            <div className="flex items-center gap-6 mb-8">
               <div className="w-20 h-20 rounded-full border border-purple-500/30 bg-[#161B22] flex items-center justify-center text-3xl font-bold text-purple-400 shadow-[0_0_20px_rgba(168,85,247,0.15)]">
                  {profile.organizationName?.slice(0, 2).toUpperCase() || "TC"}
               </div>
               <div>
                  <div className="flex items-center gap-3">
                     <span className="text-[10px] font-bold text-gray-500 tracking-wider font-mono">ID: {profile.organizationCode}</span>
                  </div>
                  <h2 className="text-xl font-bold text-white mb-2">{profile.organizationName}</h2>
                  <span className="text-xs font-semibold text-green-400 bg-green-500/10 border border-green-500/20 px-2 py-1 rounded flex items-center gap-1.5 inline-flex shadow-[0_0_10px_rgba(34,197,94,0.1)]">
                     <div className="w-1.5 h-1.5 rounded-full bg-green-400" />
                     {profile.planName}
                  </span>
               </div>
            </div>

            <div className="grid grid-cols-2 gap-6">
               <InputBlock label="Primary Contact Name" value={profile.primaryContactName} onChange={(e) => handleChange('profile', 'primaryContactName', e.target.value)} />
               <InputBlock label="Contact Email Address" value={profile.contactEmailAddress} onChange={(e) => handleChange('profile', 'contactEmailAddress', e.target.value)} type="email" />
               <div className="col-span-2">
                  <InputBlock label="Company Website" value={profile.companyWebsite} onChange={(e) => handleChange('profile', 'companyWebsite', e.target.value)} type="url" />
               </div>
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Industry</label>
                  <select value={profile.industry} onChange={(e) => handleChange('profile', 'industry', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-sm text-white rounded-lg p-3 transition-colors appearance-none outline-none">
                     <option>Technology</option><option>Finance</option><option>Healthcare</option><option>Education</option>
                  </select>
               </div>
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Company Size</label>
                  <select value={profile.companySize} onChange={(e) => handleChange('profile', 'companySize', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-sm text-white rounded-lg p-3 transition-colors appearance-none outline-none">
                     <option>1-50 employees</option><option>51-249 employees</option><option>250-500 employees</option><option>500+ employees</option>
                  </select>
               </div>
            </div>
         </TerminalPanel>

         {/* Defaults Panel */}
         <TerminalPanel header="~/nexus/org/team_defaults">
            <div className="grid grid-cols-2 gap-6 mb-8">
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Default Team Size Limit</label>
                  <div className="relative">
                     <input type="number" value={team.defaultTeamSizeLimit} onChange={(e) => handleChange('teamDefaults', 'defaultTeamSizeLimit', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 text-sm text-white rounded-lg p-3 pr-20 transition-colors" />
                     <span className="absolute right-4 top-3.5 text-[10px] text-gray-600 font-mono">members</span>
                  </div>
               </div>
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Default PTO Policy</label>
                  <select value={team.defaultPtoPolicy} onChange={e => handleChange('teamDefaults', 'defaultPtoPolicy', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 text-sm text-white rounded-lg p-3 appearance-none">
                     <option>10 days/year</option><option>15 days/year</option><option>20 days/year</option><option>Unlimited</option>
                  </select>
               </div>
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Default Work Schedule</label>
                  <select value={team.defaultWorkSchedule} onChange={e => handleChange('teamDefaults', 'defaultWorkSchedule', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 text-sm text-white rounded-lg p-3 appearance-none">
                     <option>Mon-Fri 9AM-5PM</option><option>Mon-Fri 8AM-4PM</option><option>Flexible</option>
                  </select>
               </div>
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Primary Timezone</label>
                  <select value={team.primaryTimezone} onChange={e => handleChange('teamDefaults', 'primaryTimezone', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 text-sm text-white rounded-lg p-3 appearance-none">
                     <option>America/New_York</option><option>America/Los_Angeles</option><option>Europe/London</option><option>Asia/Tokyo</option>
                  </select>
               </div>
            </div>

            <div className="space-y-6 pt-6 border-t border-[#1C212B]">
               <ToggleRow label="Auto-provision new hires" desc="Automatically create Nexus accounts via HR integration" value={team.autoProvisionNewHires} onToggle={() => handleChange('teamDefaults', 'autoProvisionNewHires', !team.autoProvisionNewHires)} />
               <ToggleRow label="Require manager approval for time off" desc="Route all PTO requests to direct managers" value={team.requireManagerApprovalForTimeOff} onToggle={() => handleChange('teamDefaults', 'requireManagerApprovalForTimeOff', !team.requireManagerApprovalForTimeOff)} />
            </div>
         </TerminalPanel>

         {/* Billing */}
         <TerminalPanel header="~/nexus/org/billing">
            <div className="bg-[#0A0D14] p-6 rounded-xl border border-[#1C212B] flex items-center justify-between mb-8 shadow-inner">
               <div>
                  <p className="text-[10px] text-purple-400 font-bold uppercase tracking-widest mb-1 font-mono">Current Plan</p>
                  <h3 className="text-2xl font-bold text-white">{billing.planName}</h3>
               </div>
               <div className="text-right">
                  <span className="text-3xl font-bold text-white tracking-tight">${billing.planPriceMonthly}</span>
                  <span className="text-gray-500 font-medium">/mo</span>
               </div>
            </div>

            <div className="space-y-6 mb-8">
               <ProgressBar title="Team Members" current={billing.teamMembersUsed} max={billing.teamMembersLimit} />
               <ProgressBar title="Active Projects" current={billing.activeProjectsUsed} max={billing.activeProjectsLimit} />
            </div>

            <div className="grid grid-cols-2 gap-x-6 gap-y-4 pt-6 border-t border-[#1C212B] mb-8">
               <InputBlock label="Next Billing Date" value={billing.nextBillingDate} readOnly />
               <div className="relative">
                  <InputBlock label="Payment Method" value={`•••• •••• •••• ${billing.paymentMethodLast4}`} readOnly />
                  <svg className="absolute top-[35px] left-3 w-5 h-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" /></svg>
               </div>
               <InputBlock label="Billing Email" value={billing.billingEmail} onChange={e => handleChange('billing', 'billingEmail', e.target.value)} />
               <InputBlock label="Tax ID / VAT Number" value={billing.taxIdOrVatNumber} onChange={e => handleChange('billing', 'taxIdOrVatNumber', e.target.value)} />
            </div>

            <button onClick={() => window.open(`${API_BASE_URL}/api/workspace/settings/export/invoices`)} className="w-full py-3 bg-[#161B22] border border-[#2D3342] hover:bg-[#1C212B] text-white font-medium text-sm rounded-lg transition-colors flex justify-center items-center gap-2">
               <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" /></svg>
               Download Past Invoices
            </button>
         </TerminalPanel>

         {/* Security */}
         <TerminalPanel header="~/nexus/org/security">
            <div className="space-y-6 mb-8">
               <ToggleRow label="Require Two-Factor Authentication" desc="Mandatory 2FA for all organization members" value={security.requireTwoFactorAuthentication} onToggle={() => handleChange('security', 'requireTwoFactorAuthentication', !security.requireTwoFactorAuthentication)} />
               <ToggleRow label="Enforce IP Allowlist" desc="Restrict access to specific corporate IP addresses" value={security.enforceIpAllowlist} onToggle={() => handleChange('security', 'enforceIpAllowlist', !security.enforceIpAllowlist)} />
               <ToggleRow label="Data Encryption at Rest" desc="AES-256 bit encryption (Enterprise feature)" value={security.dataEncryptionAtRest} onToggle={() => handleChange('security', 'dataEncryptionAtRest', !security.dataEncryptionAtRest)} />
            </div>

            <div className="grid grid-cols-2 gap-6 pt-6 border-t border-[#1C212B] mb-8">
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Idle Session Timeout</label>
                  <select value={security.idleSessionTimeout} onChange={e => handleChange('security', 'idleSessionTimeout', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 text-sm text-white rounded-lg p-3 appearance-none">
                     <option>15 Minutes</option><option>30 Minutes</option><option>1 Hour</option><option>Never</option>
                  </select>
               </div>
               <div>
                  <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">Audit Log Retention</label>
                  <select value={security.auditLogRetention} onChange={e => handleChange('security', 'auditLogRetention', e.target.value)} className="w-full bg-[#06080A] border border-[#1C212B] focus:border-purple-500/50 text-sm text-white rounded-lg p-3 appearance-none">
                     <option>30 Days</option><option>90 Days</option><option>1 Year</option><option>Indefinite</option>
                  </select>
               </div>
            </div>

            <div className="p-4 border border-[#2D3342] bg-[#161B22]/50 rounded-xl flex items-center justify-between">
               <div className="flex items-center gap-4">
                  <div className="w-10 h-10 bg-white rounded-lg flex items-center justify-center shadow-inner">
                     <div className="w-6 h-6 border-[3px] border-blue-500 rounded-full flex items-center justify-center ml-[-2px] mt-[-2px]">
                        <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
                     </div>
                  </div>
                  <div>
                     <h3 className="text-white font-semibold text-sm">Okta SSO Connected</h3>
                     <p className="text-[11px] font-medium text-green-500 tracking-wide">Identity provider active</p>
                  </div>
               </div>
               <button onClick={() => handleAction('manage-sso')} className="px-4 py-2 border border-[#2D3342] text-white hover:bg-[#1C212B] rounded-lg text-sm font-medium transition-colors">
                  Manage SSO
               </button>
            </div>
         </TerminalPanel>

         {/* Integrations Module Fake */}
         <TerminalPanel header="~/nexus/system/integrations">
            <div className="p-4 border border-[#2D3342] bg-[#161B22]/50 rounded-xl flex items-center justify-between">
               <div className="flex items-center gap-4">
                  <div className="w-10 h-10 bg-white rounded-lg flex items-center justify-center p-1">
                     <img src="https://upload.wikimedia.org/wikipedia/commons/4/41/Slack_icon_2019.svg" alt="Slack" className="w-full h-full" onError={(e) => e.target.style.display='none'}/>
                  </div>
                  <div>
                     <h3 className="text-white font-semibold text-sm">Slack Workspace</h3>
                     <p className="text-[11px] font-medium text-gray-500 tracking-wide hover:text-green-500 cursor-pointer transition-colors">Not Connected</p>
                  </div>
               </div>
               <button className="px-4 py-2 border border-[#2D3342] text-white hover:bg-[#1C212B] rounded-lg text-sm font-medium transition-colors">
                  Configure
               </button>
            </div>
         </TerminalPanel>

         {/* Danger Zone */}
         <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-red-500/30 rounded-2xl overflow-hidden shadow-[0_0_20px_rgba(239,68,68,0.05)]">
            <div className="bg-[#0A0D14] p-3 border-b border-red-500/20 flex items-center gap-4">
               <div className="flex gap-1.5 ml-1">
                  <div className="w-3 h-3 rounded-full bg-red-500 opacity-50" />
                  <div className="w-3 h-3 rounded-full bg-amber-500 opacity-50" />
                  <div className="w-3 h-3 rounded-full bg-green-500 opacity-50" />
               </div>
               <span className="text-xs font-mono text-red-500/80">~/nexus/system/danger_zone</span>
            </div>
            
            <div className="p-6 divide-y divide-[#1C212B]">
               <DangerRow 
                  title="Cancel Subscription" 
                  desc="Downgrade to free tier at end of billing cycle" 
                  btnLabel="Cancel Plan" 
                  action={() => handleAction('cancel-plan')} 
               />
               <DangerRow 
                  title="Export Organization Data" 
                  desc="Download all member details, projects, and history" 
                  btnLabel="Request Export" 
                  action={() => handleAction('request-export')} 
               />
               <DangerRow 
                  title="Close Organization Account" 
                  desc="Permanently delete all data and revoke team access" 
                  btnLabel="Delete Organization" 
                  critical
                  action={async () => {
                     await fetch(`${API_BASE_URL}/api/workspace/settings/actions/close-organization`, { 
                        method: 'DELETE', headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
                     });
                  }} 
               />
            </div>
         </div>

      </div>
    </motion.div>
  );
}


/* Helper Sub-components */

function TerminalPanel({ header, children }) {
  return (
    <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl overflow-hidden shadow-2xl">
      <div className="bg-[#0A0D14] p-3 border-b border-[#1C212B] flex items-center gap-4">
         <div className="flex gap-1.5 ml-1">
            <div className="w-3 h-3 rounded-full bg-red-500 shadow-[0_0_8px_rgba(239,68,68,0.4)]" />
            <div className="w-3 h-3 rounded-full bg-amber-500 shadow-[0_0_8px_rgba(245,158,11,0.4)]" />
            <div className="w-3 h-3 rounded-full bg-green-500 shadow-[0_0_8px_rgba(34,197,94,0.4)]" />
         </div>
         <span className="text-xs font-mono text-gray-400">{header}</span>
      </div>
      <div className="p-8">
         {children}
      </div>
    </div>
  );
}

function InputBlock({ label, value, onChange, type = "text", readOnly = false }) {
  return (
    <div>
      <label className="block text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2 font-mono">{label}</label>
      <input 
          type={type} 
          value={value || ''} 
          onChange={onChange} 
          readOnly={readOnly}
          className={`w-full bg-[#06080A] border border-[#1C212B] text-sm text-white rounded-lg py-3 px-4 outline-none transition-colors ${readOnly ? 'opacity-70 cursor-not-allowed' : 'focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30'} ${type==='email'||label.includes('Method') ? 'pl-10' : ''}`} 
      />
    </div>
  );
}

function ToggleRow({ label, desc, value, onToggle }) {
  return (
    <div className="flex items-center justify-between">
      <div>
         <h3 className="text-white font-semibold text-[15px] mb-1 tracking-tight">{label}</h3>
         <p className="text-xs text-gray-500">{desc}</p>
      </div>
      <button 
         onClick={onToggle}
         className={`relative w-12 h-[26px] rounded-full transition-colors flex items-center px-1 ${value ? 'bg-purple-600' : 'bg-[#1C212B] border border-[#2D3342]'}`}
      >
         <div className={`w-4 h-4 rounded-full bg-white transition-transform ${value ? 'translate-x-6 shadow-[0_0_10px_rgba(255,255,255,0.8)]' : 'translate-x-0'}`} />
      </button>
    </div>
  );
}

function ProgressBar({ title, current, max }) {
  const percent = Math.min((current/max)*100, 100);
  return (
    <div>
      <div className="flex items-center justify-between text-xs font-medium text-gray-400 mb-2">
         <span>{title}</span>
         <span className="font-mono">{current} / {max}</span>
      </div>
      <div className="h-1.5 w-full bg-[#161B22] rounded-full overflow-hidden">
         <div className="h-full bg-purple-500 rounded-full shadow-[0_0_10px_rgba(168,85,247,0.5)]" style={{ width: `${percent}%` }} />
      </div>
    </div>
  );
}

function DangerRow({ title, desc, btnLabel, critical, action }) {
  return (
    <div className="flex items-center justify-between py-5 first:pt-4 last:pb-4">
      <div>
         <h3 className={`font-semibold text-[15px] mb-0.5 tracking-tight ${critical ? 'text-red-400' : 'text-white'}`}>{title}</h3>
         <p className="text-xs text-gray-500">{desc}</p>
      </div>
      <button 
        onClick={action}
        className={`px-4 py-2 border text-xs font-semibold rounded-lg transition-colors bg-[#0D1117] ${critical ? 'border-red-500/50 text-red-500 hover:bg-red-500/10' : 'border-[#2D3342] text-gray-400 hover:text-white hover:border-gray-500'}`}
      >
         {btnLabel}
      </button>
    </div>
  );
}
