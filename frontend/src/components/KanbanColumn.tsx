import { useState, type DragEvent } from 'react'
import type { Epic } from '../types/epic'
import { TICKET_STATE_LABELS, TICKET_TYPE_LABELS, type Ticket, type TicketState } from '../types/ticket'
import { formatRelativeTime } from '../utils/formatRelativeTime'

interface KanbanColumnProps {
  state: TicketState
  tickets: Ticket[]
  epics: Epic[]
  onCardClick: (ticket: Ticket) => void
  onDropTicket: (ticketId: string, newState: TicketState) => void
}

export function KanbanColumn({ state, tickets, epics, onCardClick, onDropTicket }: KanbanColumnProps) {
  const [isDragOver, setIsDragOver] = useState(false)

  function handleDragOver(event: DragEvent<HTMLDivElement>) {
    event.preventDefault()
    setIsDragOver(true)
  }

  function handleDrop(event: DragEvent<HTMLDivElement>) {
    event.preventDefault()
    setIsDragOver(false)
    const ticketId = event.dataTransfer.getData('text/plain')
    if (ticketId) {
      onDropTicket(ticketId, state)
    }
  }

  return (
    <div
      className={isDragOver ? 'kanban-column drag-over' : 'kanban-column'}
      onDragOver={handleDragOver}
      onDragLeave={() => setIsDragOver(false)}
      onDrop={handleDrop}
    >
      <div className="kanban-column-header">
        <span>{TICKET_STATE_LABELS[state]}</span>
        <span className="kanban-column-count">{tickets.length}</span>
      </div>

      <div className="kanban-cards">
        {tickets.map((ticket) => (
          <div
            key={ticket.id}
            className="kanban-card"
            draggable
            onDragStart={(event) => event.dataTransfer.setData('text/plain', ticket.id)}
            onClick={() => onCardClick(ticket)}
          >
            <span className="kanban-card-type">{TICKET_TYPE_LABELS[ticket.type]}</span>
            <div className="kanban-card-title">{ticket.title}</div>
            <div className="kanban-card-meta">
              {ticket.epicId ? `Epic: ${epics.find((epic) => epic.id === ticket.epicId)?.title ?? '—'} · ` : ''}
              {formatRelativeTime(ticket.updatedAt)}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
