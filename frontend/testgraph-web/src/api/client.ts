import type {
  AcademicSampleDto,
  AcademicSampleSummaryDto,
  AnalysisResultDto,
  AuthResponseDto,
  ProjectSummaryDto,
  SavedAnalysisDto,
  TestCaseDesignResultDto,
  TestCaseDraftDto,
} from './types'

const tokenKey = 'testgraph.accessToken'

async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const token = localStorage.getItem(tokenKey)
  const headers = new Headers(init?.headers)

  if (!headers.has('Content-Type') && init?.body) {
    headers.set('Content-Type', 'application/json')
  }

  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  const response = await fetch(path, { ...init, headers })

  if (!response.ok) {
    let message = `${response.status} ${response.statusText}`
    try {
      const payload = (await response.json()) as { error?: string }
      if (payload.error) message = payload.error
    } catch {
      // Keep the HTTP status when the response body is not JSON.
    }
    throw new Error(message)
  }

  if (response.status === 204) return undefined as T
  return (await response.json()) as T
}

export function getStoredToken() {
  return localStorage.getItem(tokenKey)
}

export function clearStoredToken() {
  localStorage.removeItem(tokenKey)
}

export function storeAuth(response: AuthResponseDto) {
  localStorage.setItem(tokenKey, response.accessToken)
  return response
}

export function analyzeSource(sourceCode: string) {
  return api<AnalysisResultDto>('/api/analysis', {
    method: 'POST',
    body: JSON.stringify({ sourceCode }),
  })
}

export function designTestCases(sourceCode: string, testCases: TestCaseDraftDto[]) {
  return api<TestCaseDesignResultDto>('/api/analysis/test-cases/design', {
    method: 'POST',
    body: JSON.stringify({ sourceCode, testCases }),
  })
}

export function getSamples() {
  return api<AcademicSampleSummaryDto[]>('/api/samples')
}

export function getSample(id: string) {
  return api<AcademicSampleDto>(`/api/samples/${encodeURIComponent(id)}`)
}

export function register(email: string, password: string) {
  return api<AuthResponseDto>('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  }).then(storeAuth)
}

export function login(email: string, password: string) {
  return api<AuthResponseDto>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  }).then(storeAuth)
}

export function getMe() {
  return api<{ id: string; email: string }>('/api/auth/me')
}

export function getProjects() {
  return api<ProjectSummaryDto[]>('/api/projects')
}

export function createProject(name: string, description?: string) {
  return api<{ id: string; name: string; description?: string }>('/api/projects', {
    method: 'POST',
    body: JSON.stringify({ name, description }),
  })
}

export function saveAnalysis(projectId: string, sourceCode: string) {
  return api<SavedAnalysisDto>(`/api/projects/${projectId}/analyses`, {
    method: 'POST',
    body: JSON.stringify({ sourceCode }),
  })
}
