import { motion } from 'framer-motion';

export default function WorkspaceActivityPage() {
  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      <div>
        <h2 className="text-2xl font-bold text-white">Activity</h2>
        <p className="text-gray-400 text-sm mt-1">Your recent actions and workspace events.</p>
      </div>

      <div className="glass-panel rounded-2xl p-8">
        <div className="flex flex-col gap-3">
          {[
            { time: '15m ago', action: 'Started focus session', task: 'API integration', color: 'bg-purple-500' },
            { time: '1h ago', action: 'Completed task', task: 'UI wireframes', color: 'bg-green-500' },
            { time: '2h ago', action: 'Added schedule event', task: 'Team standup', color: 'bg-blue-500' },
            { time: '3h ago', action: 'Marked notification read', task: 'Sprint assignment', color: 'bg-amber-500' },
            { time: 'Yesterday', action: 'Logged in', task: '', color: 'bg-gray-500' },
          ].map((item, i) => (
            <div key={i} className="flex items-start gap-4 py-3 border-b border-[#30363D] last:border-0">
              <div className={`w-2 h-2 rounded-full ${item.color} flex-shrink-0 mt-1.5`} />
              <div>
                <p className="text-sm text-gray-300">
                  {item.action}{item.task && <span className="text-white font-medium"> · {item.task}</span>}
                </p>
                <span className="text-xs text-gray-500">{item.time}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </motion.div>
  );
}
