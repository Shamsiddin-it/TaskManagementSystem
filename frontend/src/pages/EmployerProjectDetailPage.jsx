import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';

export default function EmployerProjectDetailPage() {
  const navigate = useNavigate();

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="p-4 flex flex-col gap-6 max-w-5xl mx-auto w-full">
      {/* Breadcrumb */}
      <nav className="text-xs text-gray-500 flex items-center gap-1.5">
        <button onClick={() => navigate('/employer')} className="hover:text-gray-300 transition-colors">Projects</button>
        <span>/</span>
        <strong className="text-white">Project Beta</strong>
      </nav>

      <h1 className="text-2xl font-bold text-white">Project Beta Details</h1>
      <p className="text-gray-400 text-sm -mt-4">Gantt · risks · discussion</p>

      {/* Gantt chart */}
      <section className="glass-panel rounded-2xl p-6">
        <h3 className="text-base font-semibold text-white mb-4">Timeline (Gantt)</h3>
        <div className="flex flex-col gap-3">
          {[
            { name: 'UI/UX Design', offset: '0%', width: '30%', color: 'bg-purple-500' },
            { name: 'API integration', offset: '25%', width: '35%', color: 'bg-blue-500' },
            { name: 'Swift refactor', offset: '20%', width: '40%', color: 'bg-green-500' },
            { name: 'QA testing', offset: '55%', width: '30%', color: 'bg-amber-500' },
          ].map(row => (
            <div key={row.name} className="flex items-center gap-4">
              <span className="text-sm text-gray-400 w-36 flex-shrink-0">{row.name}</span>
              <div className="flex-1 h-6 bg-[#0D1117] rounded-lg relative overflow-hidden border border-[#30363D]">
                <div
                  className={`absolute h-full ${row.color} opacity-80 rounded-lg`}
                  style={{ left: row.offset, width: row.width }}
                />
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Risks */}
        <section className="glass-panel rounded-2xl p-5">
          <h3 className="text-base font-semibold text-white mb-3">Critical risks (2)</h3>
          <ul className="flex flex-col gap-2">
            <li className="flex items-start gap-2 text-sm">
              <span className="text-xs px-2 py-0.5 rounded-full bg-red-500/10 border border-red-500/30 text-red-400 flex-shrink-0">High</span>
              <span className="text-gray-300">Vendor API rate limits not contracted</span>
            </li>
            <li className="flex items-start gap-2 text-sm">
              <span className="text-xs px-2 py-0.5 rounded-full bg-amber-500/10 border border-amber-500/30 text-amber-400 flex-shrink-0">Medium</span>
              <span className="text-gray-300">iOS signing certificate expiry</span>
            </li>
          </ul>
        </section>

        {/* Task breakdown */}
        <section className="glass-panel rounded-2xl p-5">
          <h3 className="text-base font-semibold text-white mb-3">Task breakdown</h3>
          <p className="text-xs text-gray-400 mb-3">12 / 28 complete</p>
          <div className="w-full bg-[#0D1117] border border-[#30363D] rounded-lg h-2 mb-4">
            <div className="h-2 bg-purple-500 rounded-lg" style={{ width: '43%' }} />
          </div>
          <ul className="flex flex-col gap-1.5">
            {[
              { done: true, text: 'Provision environments' },
              { done: true, text: 'CI pipeline' },
              { done: false, text: 'App Store review', blocked: true },
            ].map(item => (
              <li key={item.text} className={`flex items-center gap-2 text-xs ${item.done ? 'text-gray-500 line-through' : 'text-gray-300'}`}>
                <span className={`w-3.5 h-3.5 rounded-sm border flex items-center justify-center flex-shrink-0 ${item.done ? 'bg-green-500/20 border-green-500/40 text-green-400' : item.blocked ? 'bg-red-500/10 border-red-500/30' : 'border-[#30363D]'}`}>
                  {item.done && '✓'}
                </span>
                {item.text}
                {item.blocked && <span className="text-[10px] px-1.5 py-0.5 bg-red-500/10 border border-red-500/30 text-red-400 rounded">Blocked</span>}
              </li>
            ))}
          </ul>
        </section>
      </div>

      {/* Discussion */}
      <section className="glass-panel rounded-2xl p-5">
        <h3 className="text-base font-semibold text-white mb-4">Team discussion</h3>
        <div className="flex flex-col gap-3">
          {[
            { author: 'Umar', time: '1d ago', text: 'Can we pull forward the security review?' },
            { author: 'Zulhija', time: '20h ago', text: 'Yes — I blocked Tuesday afternoon.' },
          ].map(c => (
            <div key={c.author} className="bg-[#0D1117] border border-[#30363D] rounded-xl p-3">
              <div className="flex items-center gap-2 mb-1">
                <strong className="text-xs text-white">{c.author}</strong>
                <span className="text-xs text-gray-500">· {c.time}</span>
              </div>
              <p className="text-xs text-gray-400">{c.text}</p>
            </div>
          ))}
        </div>
        <input
          type="text"
          className="mt-3 w-full bg-[#0D1117] border border-[#30363D] text-gray-300 rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
          placeholder="Add a comment…"
        />
      </section>
    </motion.div>
  );
}
