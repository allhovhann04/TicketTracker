export type TicketType = 'bug' | 'feature' | 'fix'

export type TicketState =
  | 'new'
  | 'ready_for_implementation'
  | 'in_progress'
  | 'ready_for_acceptance'
  | 'done'

export const TICKET_TYPES: TicketType[] = ['bug', 'feature', 'fix']

export const TICKET_STATES: TicketState[] = [
  'new',
  'ready_for_implementation',
  'in_progress',
  'ready_for_acceptance',
  'done',
]

export const TICKET_TYPE_LABELS: Record<TicketType, string> = {
  bug: 'Bug',
  feature: 'Feature',
  fix: 'Fix',
}

export const TICKET_STATE_LABELS: Record<TicketState, string> = {
  new: 'New',
  ready_for_implementation: 'Ready for implementation',
  in_progress: 'In progress',
  ready_for_acceptance: 'Ready for acceptance',
  done: 'Done',
}

export interface Ticket {
  id: string
  teamId: string
  epicId: string | null
  title: string
  body: string
  type: TicketType
  state: TicketState
  createdByUserId: string
  assigneeUserId: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateTicketRequest {
  teamId: string
  epicId?: string | null
  title: string
  body: string
  type: TicketType
}

export interface UpdateTicketRequest {
  teamId: string
  epicId?: string | null
  title: string
  body: string
  type: TicketType
  state: TicketState
}

export interface TicketListFilters {
  teamId?: string
  type?: TicketType
  epicId?: string
  state?: TicketState
  titleSearch?: string
}
