import { useState } from "react";
import { api } from "../lib/api.js";
import { ACTOR_ID } from "../lib/config.js";

const priorityOptions = [
  { value: 1, label: "Low" },
  { value: 2, label: "Medium" },
  { value: 3, label: "High" },
  { value: 4, label: "Critical" }
];

const ticketTypeOptions = [
  { value: 1, label: "Feature" },
  { value: 2, label: "Bug" },
  { value: 3, label: "Task" },
  { value: 4, label: "Docs" },
  { value: 5, label: "QA" },
  { value: 6, label: "Infra" }
];

export default function TaskCreateModal({
  open,
  onClose,
  onCreated,
  teamId,
  sprintId,
  teamMembers
}) {
  const [form, setForm] = useState({
    title: "",
    description: "",
    assignedToId: teamMembers?.[0]?.UserId || 1,
    priority: 2,
    ticketType: 3,
    storyPoints: "",
    estimatedHours: "",
    deadline: ""
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  if (!open) return null;

  const submit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    const payload = {
      Title: form.title,
      Description: form.description || null,
      TeamId: teamId,
      AssignedToId: Number(form.assignedToId),
      CreatedById: ACTOR_ID,
      SprintId: sprintId ?? null,
      Priority: Number(form.priority),
      TicketType: Number(form.ticketType),
      Deadline: form.deadline ? new Date(form.deadline).toISOString() : null,
      EstimatedHours: form.estimatedHours
        ? Number(form.estimatedHours)
        : null,
      StoryPoints: form.storyPoints ? Number(form.storyPoints) : null
    };

    const res = await api.post("/api/tasks", payload);
    if (!res.ok) {
      setError(res.message || "Failed to create task");
      setLoading(false);
      return;
    }

    setLoading(false);
    onCreated?.(res.data);
    onClose?.();
  };

  return (
    <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/70">
      <div className="glass-panel w-[520px] p-5">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-[14px]">Create Task</h3>
          <button
            className="text-[11px] text-[var(--text-secondary)] hover:text-white"
            onClick={onClose}
          >
            Close
          </button>
        </div>
        <form className="flex flex-col gap-3" onSubmit={submit}>
          <input
            className="w-full bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-[12px]"
            placeholder="Task title"
            value={form.title}
            onChange={(e) => setForm({ ...form, title: e.target.value })}
            required
          />
          <textarea
            className="w-full bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-[12px] min-h-[80px]"
            placeholder="Description (optional)"
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
          />
          <div className="grid grid-cols-2 gap-3">
            <select
              className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-2 py-2 text-[12px]"
              value={form.assignedToId}
              onChange={(e) =>
                setForm({ ...form, assignedToId: e.target.value })
              }
            >
              {teamMembers?.length ? (
                teamMembers.map((m) => (
                  <option key={m.Id} value={m.UserId}>
                    User #{m.UserId}
                  </option>
                ))
              ) : (
                <option value={1}>User #1</option>
              )}
            </select>
            <select
              className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-2 py-2 text-[12px]"
              value={form.priority}
              onChange={(e) => setForm({ ...form, priority: e.target.value })}
            >
              {priorityOptions.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
            <select
              className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-2 py-2 text-[12px]"
              value={form.ticketType}
              onChange={(e) => setForm({ ...form, ticketType: e.target.value })}
            >
              {ticketTypeOptions.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
            <input
              className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-[12px]"
              type="number"
              placeholder="Story points"
              value={form.storyPoints}
              onChange={(e) =>
                setForm({ ...form, storyPoints: e.target.value })
              }
            />
            <input
              className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-[12px]"
              type="number"
              placeholder="Estimated hours"
              value={form.estimatedHours}
              onChange={(e) =>
                setForm({ ...form, estimatedHours: e.target.value })
              }
            />
            <input
              className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-[12px]"
              type="date"
              value={form.deadline}
              onChange={(e) =>
                setForm({ ...form, deadline: e.target.value })
              }
            />
          </div>
          {error ? (
            <div className="text-[11px] text-[var(--color-red)]">{error}</div>
          ) : null}
          <div className="flex items-center justify-end gap-2">
            <button
              type="button"
              className="px-3 py-2 text-[11px] bg-[var(--bg-element)] border border-[var(--border-subtle)] rounded-md"
              onClick={onClose}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-3 py-2 text-[11px] bg-[#8A2BE2]/20 border border-[#8A2BE2]/40 text-[#B475FF] rounded-md"
              disabled={loading}
            >
              {loading ? "Saving..." : "Create Task"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
