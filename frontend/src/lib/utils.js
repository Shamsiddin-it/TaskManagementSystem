export function getInitials(name) {
  if (!name) return "U";
  const parts = name.trim().split(/\s+/);
  const letters = parts.slice(0, 2).map((p) => p[0].toUpperCase());
  return letters.join("");
}

export function formatDateRange(start, end) {
  if (!start || !end) return "—";
  const startDate = new Date(start);
  const endDate = new Date(end);
  const opts = { month: "short", day: "2-digit" };
  const startLabel = startDate.toLocaleDateString(undefined, opts);
  const endLabel = endDate.toLocaleDateString(undefined, opts);
  return `${startLabel} - ${endLabel}`;
}

export function formatDateShort(value) {
  if (!value) return "—";
  const date = new Date(value);
  return date.toLocaleDateString(undefined, {
    month: "short",
    day: "2-digit"
  });
}

export function clampPct(value) {
  return Math.max(0, Math.min(100, value));
}

export function priorityLabel(priority) {
  if (typeof priority === "string") {
    switch (priority.toLowerCase()) {
      case "low":
        return "P2";
      case "medium":
        return "P1";
      case "high":
      case "critical":
        return "P0";
      default:
        return priority;
    }
  }
  switch (priority) {
    case 1:
      return "P2";
    case 2:
      return "P1";
    case 3:
      return "P0";
    case 4:
      return "P0";
    default:
      return "P2";
  }
}

export function priorityClass(priority) {
  if (typeof priority === "string") {
    switch (priority.toLowerCase()) {
      case "medium":
        return "medium";
      case "high":
      case "critical":
        return "high";
      default:
        return "low";
    }
  }
  switch (priority) {
    case 1:
      return "low";
    case 2:
      return "medium";
    case 3:
    case 4:
      return "high";
    default:
      return "low";
  }
}

export function statusLabel(status) {
  if (typeof status === "string") {
    switch (status.toLowerCase()) {
      case "inprogress":
      case "in progress":
        return "In Progress";
      case "review":
        return "Review";
      case "done":
        return "Done";
      case "blocked":
        return "Blocked";
      default:
        return "Todo";
    }
  }
  switch (status) {
    case 1:
      return "Todo";
    case 2:
      return "In Progress";
    case 3:
      return "Review";
    case 4:
      return "Done";
    case 5:
      return "Blocked";
    default:
      return "Todo";
  }
}

export function ticketTypeLabel(type) {
  if (typeof type === "string") {
    return type;
  }
  switch (type) {
    case 1:
      return "Feature";
    case 2:
      return "Bug";
    case 3:
      return "Task";
    case 4:
      return "Docs";
    case 5:
      return "QA";
    case 6:
      return "Infra";
    default:
      return "Task";
  }
}

export function devRoleLabel(role) {
  if (typeof role === "string") {
    return role;
  }
  switch (role) {
    case 1:
      return "Frontend";
    case 2:
      return "Backend";
    case 3:
      return "Designer";
    case 4:
      return "Tester";
    case 5:
      return "DevOps";
    case 6:
      return "Fullstack";
    default:
      return "Member";
  }
}
