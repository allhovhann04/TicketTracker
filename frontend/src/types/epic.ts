export interface Epic {
  id: string
  teamId: string
  title: string
  description: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateEpicRequest {
  teamId: string
  title: string
  description?: string | null
}

export interface UpdateEpicRequest {
  title: string
  description?: string | null
}
