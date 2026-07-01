import { apiClient } from './client'
import type { CreateTeamRequest, Team, UpdateTeamRequest } from '../types/team'

export async function listTeams(): Promise<Team[]> {
  const response = await apiClient.get<Team[]>('/api/teams')
  return response.data
}

export async function createTeam(request: CreateTeamRequest): Promise<Team> {
  const response = await apiClient.post<Team>('/api/teams', request)
  return response.data
}

export async function updateTeam(id: string, request: UpdateTeamRequest): Promise<Team> {
  const response = await apiClient.put<Team>(`/api/teams/${id}`, request)
  return response.data
}

export async function deleteTeam(id: string): Promise<void> {
  await apiClient.delete(`/api/teams/${id}`)
}
