import { motion } from 'framer-motion';

export default function WorkspaceTeamPage() {
  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-white">Team directory</h2>
          <p className="text-gray-400 text-sm mt-1">Members in your workspace.</p>
        </div>
        <button className="bg-purple-600 hover:bg-purple-500 text-white px-4 py-2 rounded-xl text-sm font-medium transition-all">
          + Invite member
        </button>
      </div>

      <div className="glass-panel rounded-2xl p-8 text-center">
        <div className="inline-flex items-center justify-center w-16 h-16 bg-purple-600/10 border border-purple-500/20 rounded-2xl mb-4">
          <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="#a78bfa" strokeWidth="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
          </svg>
        </div>
        <h3 className="text-white font-semibold mb-2">Team directory</h3>
        <p className="text-gray-400 text-sm max-w-sm mx-auto">
          Team members will appear here once loaded from the backend. 
          For full team management use the Employer dashboard.
        </p>
      </div>
    </motion.div>
  );
}
