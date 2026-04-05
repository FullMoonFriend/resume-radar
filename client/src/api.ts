export interface UserInfo {
  username: string
  avatarUrl: string | null
  hasResume: boolean
}

export interface AnalysisSummary {
  id: number
  jobTitle: string | null
  company: string | null
  matchScore: number
  createdAt: string
}

export interface SkillGap {
  skill: string
  note: string
}

export interface AnalysisDetail {
  id: number
  jobTitle: string | null
  company: string | null
  matchScore: number
  strengthMatches: string[]
  skillGaps: SkillGap[]
  tailoredBullets: string[]
  createdAt: string
}

export interface RemainingInfo {
  remaining: number
  limit: number
}

export interface PdfExtractResult {
  success: boolean
  text: string | null
  error: string | null
}

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    ...options
  })
  if (res.status === 401) throw new Error('Unauthorized')
  if (!res.ok) {
    const body = await res.json().catch(() => ({}))
    throw new Error(body.error || `Request failed: ${res.status}`)
  }
  return res.json()
}

export const api = {
  getMe: () => request<UserInfo>('/api/auth/me'),
  logout: () => fetch('/api/auth/logout', { method: 'POST', credentials: 'include' }),

  getResume: () => request<{ resumeText: string | null }>('/api/resume'),
  saveResume: (resumeText: string) => request<void>('/api/resume', {
    method: 'POST',
    body: JSON.stringify({ resumeText })
  }),
  extractPdf: async (file: File): Promise<PdfExtractResult> => {
    const form = new FormData()
    form.append('file', file)
    const res = await fetch('/api/resume/extract-pdf', {
      method: 'POST',
      credentials: 'include',
      body: form
    })
    return res.json()
  },

  analyze: (jobPostingText: string, jobTitle?: string, company?: string) =>
    request<AnalysisDetail>('/api/analysis', {
      method: 'POST',
      body: JSON.stringify({ jobPostingText, jobTitle, company })
    }),
  getHistory: () => request<AnalysisSummary[]>('/api/analysis'),
  getAnalysis: (id: number) => request<AnalysisDetail>(`/api/analysis/${id}`),
  getRemaining: () => request<RemainingInfo>('/api/analysis/remaining')
}
