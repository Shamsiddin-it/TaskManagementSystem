import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router-dom";
import { api } from "../lib/api.js";
import { TEAM_ID } from "../lib/config.js";
import { clampPct } from "../lib/utils.js";

const PAGE_SIZE = 200;

export default function AnalyticsPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = Number(query.get("teamId")) || TEAM_ID;

  const [sprints, setSprints] = useState([]);
  const [statsBySprint, setStatsBySprint] = useState([]);
  const [tasksBySprint, setTasksBySprint] = useState(new Map());
  const [teamMembers, setTeamMembers] = useState([]);

  const loadData = async () => {
    const sprintRes = await api.get(
      `/api/sprints?TeamId=${teamId}&Page=1&PageSize=50`
    );
    const sprintItems = sprintRes.data?.Items || [];
    setSprints(sprintItems);

    const latest = sprintItems.slice(0, 4);
    const statPromises = latest.map((s) =>
      api.get(`/api/sprints/${s.Id}/stats`)
    );
    const statResponses = await Promise.all(statPromises);
    const stats = statResponses
      .filter((r) => r.ok)
      .map((r) => r.data || null)
      .filter(Boolean);
    setStatsBySprint(stats);

    const tasksMap = new Map();
    for (const s of latest) {
      const tasksRes = await api.get(
        `/api/tasks/team/${teamId}?SprintId=${s.Id}&Page=1&PageSize=${PAGE_SIZE}`
      );
      tasksMap.set(s.Id, tasksRes.data?.Items || []);
    }
    setTasksBySprint(tasksMap);

    const membersRes = await api.get(
      `/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`
    );
    setTeamMembers(membersRes.data?.Items || []);
  };

  useEffect(() => {
    loadData();
  }, [teamId]);

  const currentSprint =
    sprints.find((s) => s.Status === 2) || sprints[0] || null;

  const currentTasks = currentSprint
    ? tasksBySprint.get(currentSprint.Id) || []
    : [];

  const resolutionRates = useMemo(() => {
    const totals = { 1: 0, 2: 0, 3: 0, 4: 0 };
    const done = { 1: 0, 2: 0, 3: 0, 4: 0 };
    tasksBySprint.forEach((tasks) => {
      tasks.forEach((task) => {
        const priority = task.Priority ?? 2;
        totals[priority] = (totals[priority] || 0) + 1;
        if (task.Status === 4) {
          done[priority] = (done[priority] || 0) + 1;
        }
      });
    });

    const rate = (p) =>
      totals[p] ? Math.round((done[p] / totals[p]) * 100) : 0;

    return {
      p0: rate(4),
      p1: rate(3),
      p2: rate(1) || rate(2)
    };
  }, [tasksBySprint]);

  const totalClosed = useMemo(() => {
    let count = 0;
    tasksBySprint.forEach((tasks) => {
      count += tasks.filter((t) => t.Status === 4).length;
    });
    return count;
  }, [tasksBySprint]);

  const velocitySeries = useMemo(() => {
    const sprintMap = new Map(sprints.map((s) => [s.Id, s]));
    return statsBySprint.map((s) => ({
      id: s.SprintId,
      label: sprintMap.get(s.SprintId)?.Name || `Sprint ${s.SprintId}`,
      planned: s.PlannedPoints ?? 0,
      completed: s.CompletedPoints ?? 0
    }));
  }, [statsBySprint, sprints]);

  const avgVelocity = velocitySeries.length
    ? Math.round(
        velocitySeries.reduce((sum, s) => sum + s.completed, 0) /
          velocitySeries.length
      )
    : 0;

  const workload = useMemo(() => {
    return teamMembers.map((member) => {
      const points = currentTasks
        .filter((t) => t.AssignedToId === member.UserId)
        .reduce((sum, t) => sum + (t.StoryPoints ?? 0), 0);
      const cap = member.ThroughputPtsPerWk || 0;
      const pct = cap ? clampPct(Math.round((points / cap) * 100)) : 0;
      return {
        member,
        points,
        cap,
        pct
      };
    });
  }, [teamMembers, currentTasks]);

  const burndownProgress = currentSprint
    ? Math.round(
        ((statsBySprint.find((s) => s.SprintId === currentSprint.Id)
          ?.CompletedPoints || 0) /
          (statsBySprint.find((s) => s.SprintId === currentSprint.Id)
            ?.PlannedPoints || 1)) *
          100
      )
    : 0;

  return (
    <>
      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div>
          <h2 className="text-[15px]">Team Performance Analytics</h2>
          <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
            {currentSprint ? `Sprint ${currentSprint.Number} • Live` : "No data"}
          </div>
        </div>
        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2 bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-1.5">
            <span className="text-[11px] text-[var(--text-secondary)]">
              Period:
            </span>
            <select className="bg-transparent text-[11px] outline-none text-white appearance-none pr-4 cursor-pointer">
              <option>Last 4 Sprints</option>
              <option>Current Quarter</option>
            </select>
          </div>
          <button className="px-3 py-1.5 rounded-md text-[12px] bg-[var(--bg-element)] border border-[var(--border-subtle)] text-white hover:bg-[var(--bg-element-hover)] transition-all flex items-center gap-1.5">
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M7 10l5 5 5-5M12 15V3"></path>
            </svg>
            Export PDF
          </button>
        </div>
      </header>

      <div className="grid grid-cols-12 grid-rows-[auto_1fr] gap-4 flex-1 overflow-hidden">
        <div className="col-span-8 glass-panel flex flex-col p-5 overflow-hidden">
          <div className="flex justify-between items-center mb-6">
            <div>
              <span className="label-caps">Team Velocity</span>
              <div className="text-[11px] text-[var(--text-secondary)] mt-0.5">
                Average: {avgVelocity} pts / sprint
              </div>
            </div>
            <div className="flex gap-4">
              <div className="flex items-center gap-1.5 text-[10px] text-[var(--text-secondary)]">
                <div className="w-2 h-2 rounded-full bg-[var(--accent-purple)]"></div>{" "}
                Committed
              </div>
              <div className="flex items-center gap-1.5 text-[10px] text-[var(--text-secondary)]">
                <div className="w-2 h-2 rounded-full bg-[var(--color-green)]"></div>{" "}
                Completed
              </div>
            </div>
          </div>
          <div className="flex-1 flex items-end justify-between gap-6 px-4 pb-4">
            {velocitySeries.length ? (
              velocitySeries.map((s) => (
                <div
                  key={s.id}
                  className="flex flex-col items-center flex-1 gap-3 h-full justify-end"
                >
                  <div className="w-full flex justify-center gap-1 items-end h-[160px]">
                    <div
                      className="w-4 bg-[var(--accent-purple)] chart-bar"
                      style={{ height: `${s.planned * 2}px`, opacity: 0.4 }}
                    ></div>
                    <div
                      className="w-4 bg-[var(--color-green)] chart-bar"
                      style={{ height: `${s.completed * 2}px` }}
                    ></div>
                  </div>
                  <span className="text-[10px] text-[var(--text-tertiary)]">
                    {s.label}
                  </span>
                </div>
              ))
            ) : (
              <div className="text-[11px] text-[var(--text-tertiary)]">
                No velocity data.
              </div>
            )}
          </div>
        </div>

        <div className="col-span-4 glass-panel flex flex-col p-5">
          <span className="label-caps mb-6">Resolution Rate by Priority</span>
          <div className="flex flex-col gap-6">
            <ResolutionBar label="P0 Critical" color="var(--color-red)" value={resolutionRates.p0} />
            <ResolutionBar label="P1 High" color="var(--color-amber)" value={resolutionRates.p1} />
            <ResolutionBar label="P2 Low/Medium" color="var(--color-green)" value={resolutionRates.p2} />
          </div>
          <div className="mt-auto pt-4 border-t border-[var(--border-subtle)] flex items-center justify-between">
            <span className="text-[10px] text-[var(--text-tertiary)]">
              Total Closed: {totalClosed} Tasks
            </span>
            <div className="flex items-center gap-1 text-[var(--color-green)] text-[10px]">
              <svg
                width="10"
                height="10"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="3"
              >
                <path d="M12 19V5M5 12l7-7 7 7"></path>
              </svg>
              Based on sprint data
            </div>
          </div>
        </div>

        <div className="col-span-6 glass-panel flex flex-col p-5">
          <div className="flex justify-between items-center mb-6">
            <span className="label-caps">
              {currentSprint ? `${currentSprint.Name} Burndown` : "Burndown"}
            </span>
            <div className="flex gap-4">
              <div className="flex items-center gap-1.5 text-[10px] text-[var(--text-tertiary)]">
                <div className="w-2 h-0.5 bg-[var(--text-tertiary)]"></div>{" "}
                Ideal
              </div>
              <div className="flex items-center gap-1.5 text-[10px] text-[var(--accent-purple-light)]">
                <div className="w-2 h-0.5 bg-[var(--accent-purple-light)]"></div>{" "}
                Actual
              </div>
            </div>
          </div>
          <div className="flex-1 relative mt-2">
            <svg className="w-full h-full" viewBox="0 0 400 180">
              <line x1="0" y1="0" x2="400" y2="0" stroke="var(--border-subtle)" strokeWidth="0.5"></line>
              <line x1="0" y1="60" x2="400" y2="60" stroke="var(--border-subtle)" strokeWidth="0.5"></line>
              <line x1="0" y1="120" x2="400" y2="120" stroke="var(--border-subtle)" strokeWidth="0.5"></line>
              <line x1="0" y1="180" x2="400" y2="180" stroke="var(--border-subtle)" strokeWidth="1"></line>
              <line x1="0" y1="20" x2="400" y2="180" stroke="var(--text-tertiary)" strokeWidth="1.5" strokeDasharray="4 2"></line>
              {currentSprint ? (
                <>
                  <path
                    className="line-chart-path"
                    d={`M0 20 L100 ${
                      180 - burndownProgress * 1.5
                    } L200 ${180 - burndownProgress * 1.5} L300 ${
                      180 - burndownProgress * 1.5
                    }`}
                    fill="none"
                    stroke="var(--accent-purple-light)"
                    strokeWidth="2.5"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  ></path>
                  <circle
                    cx="300"
                    cy={180 - burndownProgress * 1.5}
                    r="3"
                    fill="var(--accent-purple-light)"
                  ></circle>
                  <line
                    x1="300"
                    y1="0"
                    x2="300"
                    y2="180"
                    stroke="var(--accent-purple)"
                    strokeWidth="1"
                    strokeDasharray="2 2"
                    opacity="0.4"
                  ></line>
                </>
              ) : null}
            </svg>
            <div className="absolute bottom-[-15px] w-full flex justify-between text-[9px] text-[var(--text-tertiary)] px-1">
              <span>Day 1</span>
              <span>Day 5</span>
              <span>Day 10</span>
              <span>Day 14</span>
            </div>
          </div>
        </div>

        <div className="col-span-6 glass-panel flex flex-col p-5">
          <span className="label-caps mb-4">Workload Distribution</span>
          <div className="flex-1 overflow-y-auto pr-1">
            {workload.length ? (
              workload.map((item) => (
                <div
                  key={item.member.Id}
                  className="flex items-center gap-4 py-3 border-b border-[var(--border-subtle)]"
                >
                  <div className="avatar">U{item.member.UserId}</div>
                  <div className="flex-1">
                    <div className="flex justify-between text-[11px] mb-1">
                      <span className="text-white">
                        User #{item.member.UserId}
                      </span>
                      <span className="text-[var(--text-secondary)] font-mono">
                        {item.points} pts / {item.cap || "—"} caps
                      </span>
                    </div>
                    <div className="h-1.5 w-full bg-[var(--bg-element)] rounded-full overflow-hidden">
                      <div
                        className="h-full bg-[var(--accent-purple)]"
                        style={{ width: `${item.pct}%` }}
                      ></div>
                    </div>
                  </div>
                </div>
              ))
            ) : (
              <div className="text-[11px] text-[var(--text-tertiary)]">
                No workload data.
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

function ResolutionBar({ label, color, value }) {
  return (
    <div>
      <div className="flex justify-between text-[11px] mb-2">
        <span className="font-mono" style={{ color }}>
          {label}
        </span>
        <span className="text-white">{value}% Resolved</span>
      </div>
      <div className="h-1 bg-[var(--bg-element)] rounded-full overflow-hidden">
        <div className="h-full" style={{ width: `${value}%`, background: color }}></div>
      </div>
    </div>
  );
}
