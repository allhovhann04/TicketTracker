import { apiClient } from './client'
import type { CreateTicketRequest, Ticket, TicketListFilters, UpdateTicketRequest } from '../types/ticket'

export async function listTickets(filters: TicketListFilters): Promise<Ticket[]> {
  const response = await apiClient.get<Ticket[]>('/api/tickets', {
    params: {
      teamId: filters.teamId || undefined,
      type: filters.type || undefined,
      epicId: filters.epicId || undefined,
      state: filters.state || undefined,
      titleSearch: filters.titleSearch || undefined,
    },
  })
  return response.data
}

export async function getTicket(id: string): Promise<Ticket> {
  const response = await apiClient.get<Ticket>(`/api/tickets/${id}`)
  return response.data
}

export async function createTicket(request: CreateTicketRequest): Promise<Ticket> {
  const response = await apiClient.post<Ticket>('/api/tickets', request)
  return response.data
}

export async function updateTicket(id: string, request: UpdateTicketRequest): Promise<Ticket> {
  const response = await apiClient.put<Ticket>(`/api/tickets/${id}`, request)
  return response.data
}

export async function deleteTicket(id: string): Promise<void> {
  await apiClient.delete(`/api/tickets/${id}`)
}
