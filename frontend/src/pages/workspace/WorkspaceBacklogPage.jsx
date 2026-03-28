import { motion } from 'framer-motion';

export default function WorkspaceBacklogPage() {
  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      <div>
        <h2 className="text-2xl font-bold text-white">Browse backlog</h2>
        <p className="text-gray-400 text-sm mt-1">All pending tasks and issues across your projects.</p>
      </div>

      <div className="glass-panel rounded-2xl p-8 text-center">
        <div className="inline-flex items-center justify-center w-16 h-16 bg-purple-600/10 border border-purple-500/20 rounded-2xl mb-4">
          <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="#a78bfa" strokeWidth="2">
            <line x1="8" y1="6" x2="21" y2="6" /><line x1="8" y1="12" x2="21" y2="12" /><line x1="8" y1="18" x2="21" y2="18" />
            <line x1="3" y1="6" x2="3.01" y2="6" /><line x1="3" y1="12" x2="3.01" y2="12" /><line x1="3" y1="18" x2="3.01" y2="18" />
          </svg>
        </div>
        <h3 className="text-white font-semibold mb-2">Backlog coming soon</h3>
        <p className="text-gray-400 text-sm max-w-sm mx-auto">
          The full backlog columns will appear here once tasks are loaded from the backend.
          Use the Team Lead backlog view for full functionality.
        </p>
      </div>
    </motion.div>
  );
}
