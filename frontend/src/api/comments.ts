import { apiClient } from './client'
import type { Comment, CreateCommentRequest } from '../types/comment'

export async function listComments(ticketId: string): Promise<Comment[]> {
  const response = await apiClient.get<Comment[]>(`/api/tickets/${ticketId}/comments`)
  return response.data
}

export async function createComment(ticketId: string, request: CreateCommentRequest): Promise<Comment> {
  const response = await apiClient.post<Comment>(`/api/tickets/${ticketId}/comments`, request)
  return response.data
}
