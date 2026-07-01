import { apiClient } from './client'
import type { CreateEpicRequest, Epic, UpdateEpicRequest } from '../types/epic'

export async function listEpics(teamId?: string): Promise<Epic[]> {
  const response = await apiClient.get<Epic[]>('/api/epics', {
    params: teamId ? { teamId } : undefined,
  })
  return response.data
}

export async function createEpic(request: CreateEpicRequest): Promise<Epic> {
  const response = await apiClient.post<Epic>('/api/epics', request)
  return response.data
}

export async function updateEpic(id: string, request: UpdateEpicRequest): Promise<Epic> {
  const response = await apiClient.put<Epic>(`/api/epics/${id}`, request)
  return response.data
}

export async function deleteEpic(id: string): Promise<void> {
  await apiClient.delete(`/api/epics/${id}`)
}
