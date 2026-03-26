import type { NotificationDto, ProjectCard, ProjectDto, ProjectStatus } from './types'

export function mapProjectStatus(status: ProjectStatus): ProjectCard['status'] {
  switch (status) {
    case 'Active':
      return 'In Progress'
    case 'AtRisk':
      return 'At Risk'
    case 'Archived':
      return 'Done'
    default:
      return 'Planning'
  }
}

export function mapProjectsToBoard(projects: ProjectDto[]): ProjectCard[] {
  return projects.map((project) => ({
    id: project.id,
    title: project.title,
    type: project.type.toUpperCase(),
    status: mapProjectStatus(project.status),
    progress: project.completionPercent ?? 0,
    dueLabel: project.globalDeadline
      ? new Date(project.globalDeadline).toLocaleDateString('en-US', {
          month: 'short',
          day: '2-digit',
        })
      : 'No deadline',
    note:
      project.status === 'AtRisk'
        ? 'Attention required'
        : project.status === 'Archived'
          ? 'Completed'
          : project.status === 'Planning'
            ? 'Planning stage'
            : 'In progress',
    members: [],
    accent:
      project.status === 'AtRisk'
        ? '#ffb31f'
        : project.status === 'Archived'
          ? '#2bd267'
          : '#a64bff',
    chipTone:
      project.status === 'AtRisk'
        ? 'amber'
        : project.status === 'Archived'
          ? 'green'
          : 'purple',
  }))
}

export function mapBackendNotifications(items: NotificationDto[]) {
  return items.map((item) => ({
    id: item.id,
    title: item.title,
    body: item.body,
    type: item.type.replaceAll(/([A-Z])/g, ' $1').trim() || 'System',
    createdAt: formatRelative(item.createdAt),
    tone:
      item.priority === 'High'
        ? 'amber'
        : item.type.toLowerCase().includes('budget')
          ? 'green'
          : item.type.toLowerCase().includes('team')
            ? 'blue'
            : 'violet',
    action: item.isRead ? undefined : 'Open',
  }))
}

export function formatRelative(value: string) {
  const date = new Date(value)
  const diffHours = Math.round((Date.now() - date.getTime()) / 3600000)
  if (diffHours <= 1) return '1 hour ago'
  if (diffHours < 24) return `${diffHours} hours ago`
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
}

export function formatMoney(value: number) {
  return Math.round(value).toLocaleString('en-US')
}
