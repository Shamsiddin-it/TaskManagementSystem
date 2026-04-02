export type NavKey =
  | 'overview'
  | 'team'
  | 'projects'
  | 'project-detail'
  | 'reports'
  | 'notifications'
  | 'settings'

export type ProjectStatus = 'Planning' | 'Active' | 'AtRisk' | 'Archived'
export type ProjectType = 'Internal' | 'Mobile' | 'Api' | 'Web' | 'Enterprise'

export type UserSession = {
  token: string
  userId: string
  fullName: string
}

export type ApiResponse<T> = {
  statusCode: number
  description: string[]
  /** Backend serializes as 'Data' (capital D) */
  Data?: T
  data?: T
}

export type AuthResponseDto = {
  token: string
  role: string
  userId: string
  fullName: string
  firstName: string
  teamId?: string
}

export type ProjectDto = {
  id: string
  title: string
  description?: string
  status: ProjectStatus
  type: ProjectType
  globalDeadline?: string
  budgetAllocated?: number
  budgetSpent?: number
  completionPercent?: number
  createdAt: string
}

export type NotificationDto = {
  id: string
  title: string
  body: string
  type: string
  priority: string
  createdAt: string
  isRead: boolean
}

export type OrgBudgetDto = {
  period: string
  totalBudget: number
  spentAmount: number
  burnPercent: number
  estimatedRunwayMonths: number
}

export type CreateProjectForm = {
  title: string
  description: string
  type: ProjectType
  deadline: string
  budget: string
}

export type ProjectCard = {
  id: string
  title: string
  type: string
  status: 'Planning' | 'In Progress' | 'At Risk' | 'Done'
  progress: number
  dueLabel: string
  note: string
  members: string[]
  accent: string
  chipTone: 'purple' | 'amber' | 'green'
}

export type TeamMember = {
  id: string
  initials: string
  name: string
  role: string
  status: string
  statusTone: 'green' | 'amber' | 'red'
  projects: string[]
  skills: string[]
  workload: number
  accent: string
}
