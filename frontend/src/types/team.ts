export interface Team {
  id: string
  name: string
  createdAt: string
  updatedAt: string
}

export interface CreateTeamRequest {
  name: string
}

export interface UpdateTeamRequest {
  name: string
}
