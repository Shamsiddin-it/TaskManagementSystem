import type {
  ApiResponse,
  AuthResponseDto,
  NotificationDto,
  OrgBudgetDto,
  ProjectDto,
} from './types'

const apiBase = import.meta.env.VITE_API_BASE_URL ?? '/api'

async function parseResponse<T>(response: Response) {
  if (!response.ok) return null
  const payload = (await response.json()) as ApiResponse<T>
  return (payload as any)?.Data ?? (payload as any)?.data ?? (payload as any) ?? null
}


export function logout() {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  window.location.href = '/login'
}

export function getCurrentUser() {
  const user = localStorage.getItem('user')
  return user ? JSON.parse(user) : null
}

export async function registerEmployer(body: { fullName: string; email: string; password: string }) {
  const response = await fetch(`${apiBase}/auth/register/employer`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  return parseResponse<AuthResponseDto>(response)
}

export async function loginEmployer(body: { email: string; password: string }) {
  const response = await fetch(`${apiBase}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  return parseResponse<AuthResponseDto>(response)
}

export async function getProjects(token: string) {
  const response = await fetch(`${apiBase}/projects`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<ProjectDto[]>(response)
}

export async function getProjectById(token: string, projectId: string) {
  const response = await fetch(`${apiBase}/projects/${projectId}`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<ProjectDto>(response)
}

export async function updateProject(token: string, projectId: string, body: object) {
  const response = await fetch(`${apiBase}/projects/${projectId}`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  return parseResponse<ProjectDto>(response)
}

export async function createProject(token: string, body: object) {
  const response = await fetch(`${apiBase}/projects`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  return parseResponse<ProjectDto>(response)
}

export async function getNotifications(token: string) {
  const response = await fetch(`${apiBase}/employer/notifications`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<NotificationDto[]>(response)
}

export async function getWorkspaceOverview(token: string) {
  const response = await fetch(`${apiBase}/workspace/overview`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<{
    totalProjects: number
    activeProjects: number
    activeMembers: number
    monthlyCost: number
    completionRate: number
    unreadNotifications: number
  }>(response)
}

export async function getWorkspaceSettings(token: string) {
  const response = await fetch(`${apiBase}/workspace/settings`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<{
    profile: {
      organizationName: string
      organizationCode: string
      primaryContactName: string
      contactEmailAddress: string
      companyWebsite: string
      industry: string
      companySize: string
      planName: string
    }
    teamDefaults: {
      defaultTeamSizeLimit: number
      defaultPtoPolicy: string
      defaultWorkSchedule: string
      primaryTimezone: string
      autoProvisionNewHires: boolean
      requireManagerApprovalForTimeOff: boolean
    }
    billing: {
      planName: string
      planPriceMonthly: number
      teamMembersUsed: number
      teamMembersLimit: number
      activeProjectsUsed: number
      activeProjectsLimit: number
      nextBillingDate: string
      paymentMethodLast4: string
      billingEmail: string
      taxIdOrVatNumber: string
    }
    security: {
      requireTwoFactorAuthentication: boolean
      enforceIpAllowlist: boolean
      dataEncryptionAtRest: boolean
      idleSessionTimeout: string
      auditLogRetention: string
      ssoProviderName: string
      ssoConnected: boolean
    }
    integrations: Array<{
      key: string
      name: string
      status: string
      isConnected: boolean
      accent: 'okta' | 'google' | 'slack' | 'workday' | 'greenhouse' | 'salesforce'
    }>
  }>(response)
}

export async function updateWorkspaceSettings(token: string, body: object) {
  const response = await fetch(`${apiBase}/workspace/settings`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  return parseResponse<{
    profile: {
      organizationName: string
      organizationCode: string
      primaryContactName: string
      contactEmailAddress: string
      companyWebsite: string
      industry: string
      companySize: string
      planName: string
    }
    teamDefaults: {
      defaultTeamSizeLimit: number
      defaultPtoPolicy: string
      defaultWorkSchedule: string
      primaryTimezone: string
      autoProvisionNewHires: boolean
      requireManagerApprovalForTimeOff: boolean
    }
    billing: {
      planName: string
      planPriceMonthly: number
      teamMembersUsed: number
      teamMembersLimit: number
      activeProjectsUsed: number
      activeProjectsLimit: number
      nextBillingDate: string
      paymentMethodLast4: string
      billingEmail: string
      taxIdOrVatNumber: string
    }
    security: {
      requireTwoFactorAuthentication: boolean
      enforceIpAllowlist: boolean
      dataEncryptionAtRest: boolean
      idleSessionTimeout: string
      auditLogRetention: string
      ssoProviderName: string
      ssoConnected: boolean
    }
    integrations: Array<{
      key: string
      name: string
      status: string
      isConnected: boolean
      accent: 'okta' | 'google' | 'slack' | 'workday' | 'greenhouse' | 'salesforce'
    }>
  }>(response)
}

export async function workspaceAction(token: string, path: string, method: 'POST' | 'DELETE' = 'POST') {
  const response = await fetch(`${apiBase}${path}`, {
    method,
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  })

  return parseResponse<{ message: string }>(response)
}

export async function integrationAction(token: string, key: string, action: string) {
  const response = await fetch(`${apiBase}/workspace/settings/integrations/${key}`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ action }),
  })

  return parseResponse<{
    profile: {
      organizationName: string
      organizationCode: string
      primaryContactName: string
      contactEmailAddress: string
      companyWebsite: string
      industry: string
      companySize: string
      planName: string
    }
    teamDefaults: {
      defaultTeamSizeLimit: number
      defaultPtoPolicy: string
      defaultWorkSchedule: string
      primaryTimezone: string
      autoProvisionNewHires: boolean
      requireManagerApprovalForTimeOff: boolean
    }
    billing: {
      planName: string
      planPriceMonthly: number
      teamMembersUsed: number
      teamMembersLimit: number
      activeProjectsUsed: number
      activeProjectsLimit: number
      nextBillingDate: string
      paymentMethodLast4: string
      billingEmail: string
      taxIdOrVatNumber: string
    }
    security: {
      requireTwoFactorAuthentication: boolean
      enforceIpAllowlist: boolean
      dataEncryptionAtRest: boolean
      idleSessionTimeout: string
      auditLogRetention: string
      ssoProviderName: string
      ssoConnected: boolean
    }
    integrations: Array<{
      key: string
      name: string
      status: string
      isConnected: boolean
      accent: 'okta' | 'google' | 'slack' | 'workday' | 'greenhouse' | 'salesforce'
    }>
  }>(response)
}

export async function downloadInvoices(token: string) {
  const response = await fetch(`${apiBase}/workspace/settings/export/invoices`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<{
    fileName: string
    contentType: string
    content: string
  }>(response)
}

export async function getUsers(token: string) {
  const response = await fetch(`${apiBase}/users`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<Array<{
    id: string
    fullName: string
    email: string
    role: string
    onlineStatus: string
    avatarInitials?: string
    avatarColor?: string
    currentProjects: string[]
    skills: string[]
    workloadPercent: number
  }>>(response)
}

export async function createUser(token: string, body: object) {
  const response = await fetch(`${apiBase}/users`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  return parseResponse(response)
}

export async function getReports(token: string) {
  const response = await fetch(`${apiBase}/reports/dashboard`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<{
    completionTrend: Array<{ label: string; actual: number; target: number }>
    budgetSpendByProject: Array<{ projectId: string; projectTitle: string; amount: number }>
    teamHeatmap: number[]
  }>(response)
}

export async function getProjectStats(token: string, projectId: string) {
  const response = await fetch(`${apiBase}/projects/${projectId}/stats`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<{
    totalTasks: number
    completedTasks: number
    blockedTasks: number
    totalMembers: number
    completionPercent: number
    budgetBurnPercent: number
    risks: Array<{ id: string; description: string; severity: string; status: string }>
  }>(response)
}

export async function getProjectMembers(token: string, projectId: string) {
  const response = await fetch(`${apiBase}/projects/${projectId}/members`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<Array<{
    userId: string
    fullName?: string
    avatarInitials?: string
    avatarColor?: string
    projectRole: string
    availability: string
  }>>(response)
}

export async function addProjectMember(token: string, projectId: string, body: object) {
  const response = await fetch(`${apiBase}/projects/${projectId}/members`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  return parseResponse(response)
}

export async function getProjectTimeline(token: string, projectId: string) {
  const response = await fetch(`${apiBase}/projects/${projectId}/timeline`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<Array<{
    id: string
    phaseName: string
    startDate: string
    endDate: string
    colorHex: string
    status: string
    orderIndex: number
  }>>(response)
}

export async function getProjectRisks(token: string, projectId: string) {
  const response = await fetch(`${apiBase}/projects/${projectId}/risks`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<Array<{
    id: string
    description: string
    severity: string
    status: string
    createdAt: string
  }>>(response)
}

export async function getProjectBudgetHistory(token: string, projectId: string) {
  const response = await fetch(`${apiBase}/budget/projects/${projectId}`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<Array<{ amount: number; description: string; recordDate: string }>>(response)
}

export async function markAllNotifications(token: string) {
  const response = await fetch(`${apiBase}/employer/notifications/read-all`, {
    method: 'PATCH',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  })

  return parseResponse<boolean>(response)
}

export async function getBudget(token: string) {
  const response = await fetch(`${apiBase}/budget/org`, {
    headers: { Authorization: `Bearer ${token}` },
  })

  return parseResponse<OrgBudgetDto>(response)
}
