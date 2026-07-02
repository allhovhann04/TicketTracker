import { useEffect, useState, type FormEvent } from 'react'
import { listEpics } from '../api/epics'
import { getErrorMessage } from '../api/errors'
import { createTicket, deleteTicket, updateTicket } from '../api/tickets'
import { CommentsSection } from './CommentsSection'
import type { Epic } from '../types/epic'
import type { Team } from '../types/team'
import {
  TICKET_STATES,
  TICKET_STATE_LABELS,
  TICKET_TYPES,
  TICKET_TYPE_LABELS,
  type Ticket,
  type TicketState,
  type TicketType,
} from '../types/ticket'

interface TicketFormModalProps {
  mode: 'create' | 'edit'
  ticket?: Ticket
  teams: Team[]
  defaultTeamId?: string
  onClose: () => void
  onSaved: () => void
}

export function TicketFormModal({ mode, ticket, teams, defaultTeamId, onClose, onSaved }: TicketFormModalProps) {
  const [teamId, setTeamId] = useState(ticket?.teamId ?? defaultTeamId ?? teams[0]?.id ?? '')
  const [epicId, setEpicId] = useState(ticket?.epicId ?? '')
  const [title, setTitle] = useState(ticket?.title ?? '')
  const [body, setBody] = useState(ticket?.body ?? '')
  const [type, setType] = useState<TicketType>(ticket?.type ?? 'bug')
  const [state, setState] = useState<TicketState>(ticket?.state ?? 'new')

  const [epics, setEpics] = useState<Epic[]>([])
  const [isLoadingEpics, setIsLoadingEpics] = useState(false)

  const [error, setError] = useState<string | null>(null)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    if (!teamId) {
      setEpics([])
      return
    }

    setIsLoadingEpics(true)
    listEpics(teamId)
      .then(setEpics)
      .catch(() => setEpics([]))
      .finally(() => setIsLoadingEpics(false))
  }, [teamId])

  function handleTeamChange(newTeamId: string) {
    setTeamId(newTeamId)
    // The epic belongs to the old team and the backend rejects a mismatched epic,
    // so clear the selection here - it's the UI's job to do this, not the backend's.
    setEpicId('')
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setIsSaving(true)

    try {
      if (mode === 'create') {
        await createTicket({ teamId, epicId: epicId || null, title, body, type })
      } else if (ticket) {
        await updateTicket(ticket.id, { teamId, epicId: epicId || null, title, body, type, state })
      }
      onSaved()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete() {
    if (!ticket) {
      return
    }

    if (!window.confirm(`Delete ticket "${ticket.title}"? This cannot be undone.`)) {
      return
    }

    setError(null)
    setIsSaving(true)

    try {
      await deleteTicket(ticket.id)
      onSaved()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={(event) => event.stopPropagation()}>
        <h2>{mode === 'create' ? 'New ticket' : 'Ticket details'}</h2>

        {mode === 'edit' && ticket ? (
          <p className="ticket-meta">
            Created {new Date(ticket.createdAt).toLocaleString()} · Modified{' '}
            {new Date(ticket.updatedAt).toLocaleString()}
          </p>
        ) : null}

        <form className="form" onSubmit={handleSubmit}>
          <div className="form-row">
            <label className="field">
              <span>Team</span>
              <select value={teamId} onChange={(event) => handleTeamChange(event.target.value)} required>
                {teams.map((team) => (
                  <option key={team.id} value={team.id}>
                    {team.name}
                  </option>
                ))}
              </select>
            </label>

            <label className="field">
              <span>Type</span>
              <select value={type} onChange={(event) => setType(event.target.value as TicketType)} required>
                {TICKET_TYPES.map((value) => (
                  <option key={value} value={value}>
                    {TICKET_TYPE_LABELS[value]}
                  </option>
                ))}
              </select>
            </label>
          </div>

          <div className="form-row">
            <label className="field">
              <span>Epic</span>
              <select value={epicId} onChange={(event) => setEpicId(event.target.value)} disabled={isLoadingEpics}>
                <option value="">No epic</option>
                {epics.map((epic) => (
                  <option key={epic.id} value={epic.id}>
                    {epic.title}
                  </option>
                ))}
              </select>
            </label>

            {mode === 'edit' ? (
              <label className="field">
                <span>State</span>
                <select value={state} onChange={(event) => setState(event.target.value as TicketState)} required>
                  {TICKET_STATES.map((value) => (
                    <option key={value} value={value}>
                      {TICKET_STATE_LABELS[value]}
                    </option>
                  ))}
                </select>
              </label>
            ) : null}
          </div>

          <label className="field">
            <span>Title</span>
            <input type="text" value={title} onChange={(event) => setTitle(event.target.value)} required />
          </label>

          <label className="field">
            <span>Body</span>
            <textarea value={body} onChange={(event) => setBody(event.target.value)} rows={6} required />
          </label>

          {error ? <p className="error-message">{error}</p> : null}

          <div className="modal-actions">
            {mode === 'edit' ? (
              <button type="button" className="danger-button" onClick={handleDelete} disabled={isSaving}>
                Delete
              </button>
            ) : (
              <span />
            )}
            <div className="modal-actions-right">
              <button type="button" onClick={onClose} disabled={isSaving}>
                Cancel
              </button>
              <button type="submit" disabled={isSaving}>
                {isSaving ? 'Saving…' : 'Save'}
              </button>
            </div>
          </div>
        </form>

        {mode === 'edit' && ticket ? <CommentsSection ticketId={ticket.id} /> : null}
      </div>
    </div>
  )
}
