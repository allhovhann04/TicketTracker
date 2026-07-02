import { useEffect, useState } from 'react'
import { TicketFormModal } from '../../components/TicketFormModal'
import { listEpics } from '../../api/epics'
import { getErrorMessage } from '../../api/errors'
import { listTeams } from '../../api/teams'
import { listTickets } from '../../api/tickets'
import type { Epic } from '../../types/epic'
import type { Team } from '../../types/team'
import { TICKET_STATES, TICKET_STATE_LABELS, TICKET_TYPES, TICKET_TYPE_LABELS, type Ticket, type TicketState, type TicketType } from '../../types/ticket'

export function BoardPage() {
  const [teams, setTeams] = useState<Team[]>([])
  const [isLoadingTeams, setIsLoadingTeams] = useState(true)
  const [teamsError, setTeamsError] = useState<string | null>(null)

  const [filterTeamId, setFilterTeamId] = useState<string | null>(null)
  const [filterType, setFilterType] = useState<TicketType | ''>('')
  const [filterEpicId, setFilterEpicId] = useState('')
  const [filterState, setFilterState] = useState<TicketState | ''>('')
  const [titleSearchInput, setTitleSearchInput] = useState('')
  const [titleSearch, setTitleSearch] = useState('')

  const [filterEpics, setFilterEpics] = useState<Epic[]>([])

  const [tickets, setTickets] = useState<Ticket[]>([])
  const [isLoadingTickets, setIsLoadingTickets] = useState(false)
  const [ticketsError, setTicketsError] = useState<string | null>(null)

  const [modalMode, setModalMode] = useState<'create' | 'edit' | null>(null)
  const [selectedTicket, setSelectedTicket] = useState<Ticket | null>(null)

  async function loadTeams() {
    setIsLoadingTeams(true)
    setTeamsError(null)

    try {
      const result = await listTeams()
      setTeams(result)
      setFilterTeamId((current) => current ?? result[0]?.id ?? null)
    } catch (err) {
      setTeamsError(getErrorMessage(err, 'Failed to load teams.'))
    } finally {
      setIsLoadingTeams(false)
    }
  }

  useEffect(() => {
    void loadTeams()
  }, [])

  // Debounce the title search so it doesn't fire a request on every keystroke.
  useEffect(() => {
    const handle = setTimeout(() => setTitleSearch(titleSearchInput), 300)
    return () => clearTimeout(handle)
  }, [titleSearchInput])

  useEffect(() => {
    if (!filterTeamId) {
      setFilterEpics([])
      return
    }

    setFilterEpicId('')
    listEpics(filterTeamId)
      .then(setFilterEpics)
      .catch(() => setFilterEpics([]))
  }, [filterTeamId])

  async function loadTickets() {
    if (!filterTeamId) {
      setTickets([])
      return
    }

    setIsLoadingTickets(true)
    setTicketsError(null)

    try {
      const result = await listTickets({
        teamId: filterTeamId,
        type: filterType || undefined,
        epicId: filterEpicId || undefined,
        state: filterState || undefined,
        titleSearch: titleSearch || undefined,
      })
      setTickets(result)
    } catch (err) {
      setTicketsError(getErrorMessage(err, 'Failed to load tickets.'))
    } finally {
      setIsLoadingTickets(false)
    }
  }

  useEffect(() => {
    void loadTickets()
  }, [filterTeamId, filterType, filterEpicId, filterState, titleSearch])

  function openCreateModal() {
    setModalMode('create')
    setSelectedTicket(null)
  }

  function openEditModal(ticket: Ticket) {
    setModalMode('edit')
    setSelectedTicket(ticket)
  }

  function closeModal() {
    setModalMode(null)
    setSelectedTicket(null)
  }

  function handleSaved() {
    closeModal()
    void loadTickets()
  }

  return (
    <div className="page">
      <div className="page-header">
        <h1>Board</h1>
        <button type="button" onClick={openCreateModal} disabled={!filterTeamId}>
          + New ticket
        </button>
      </div>

      {isLoadingTeams ? <p>Loading teams…</p> : null}
      {teamsError ? <p className="error-message">{teamsError}</p> : null}

      {!isLoadingTeams && !teamsError && teams.length === 0 ? (
        <p className="empty-state">No teams yet. Create one on the Teams page first.</p>
      ) : null}

      {!isLoadingTeams && teams.length > 0 ? (
        <div className="filters">
          <label className="field">
            <span>Team</span>
            <select value={filterTeamId ?? ''} onChange={(event) => setFilterTeamId(event.target.value || null)}>
              {teams.map((team) => (
                <option key={team.id} value={team.id}>
                  {team.name}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Type</span>
            <select value={filterType} onChange={(event) => setFilterType(event.target.value as TicketType | '')}>
              <option value="">All types</option>
              {TICKET_TYPES.map((value) => (
                <option key={value} value={value}>
                  {TICKET_TYPE_LABELS[value]}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Epic</span>
            <select value={filterEpicId} onChange={(event) => setFilterEpicId(event.target.value)}>
              <option value="">All epics</option>
              {filterEpics.map((epic) => (
                <option key={epic.id} value={epic.id}>
                  {epic.title}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>State</span>
            <select value={filterState} onChange={(event) => setFilterState(event.target.value as TicketState | '')}>
              <option value="">All states</option>
              {TICKET_STATES.map((value) => (
                <option key={value} value={value}>
                  {TICKET_STATE_LABELS[value]}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Search title</span>
            <input
              type="text"
              value={titleSearchInput}
              onChange={(event) => setTitleSearchInput(event.target.value)}
              placeholder="Search…"
            />
          </label>
        </div>
      ) : null}

      {isLoadingTickets ? <p>Loading tickets…</p> : null}
      {ticketsError ? <p className="error-message">{ticketsError}</p> : null}

      {!isLoadingTickets && !ticketsError && filterTeamId && tickets.length === 0 ? (
        <p className="empty-state">No tickets match the current filters.</p>
      ) : null}

      {!isLoadingTickets && !ticketsError && tickets.length > 0 ? (
        <table className="table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Type</th>
              <th>State</th>
              <th>Epic</th>
              <th>Modified</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {tickets.map((ticket) => (
              <tr key={ticket.id}>
                <td>{ticket.title}</td>
                <td>{TICKET_TYPE_LABELS[ticket.type]}</td>
                <td>{TICKET_STATE_LABELS[ticket.state]}</td>
                <td>{filterEpics.find((epic) => epic.id === ticket.epicId)?.title ?? '—'}</td>
                <td>{new Date(ticket.updatedAt).toLocaleString()}</td>
                <td>
                  <button type="button" onClick={() => openEditModal(ticket)}>
                    Open
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : null}

      {modalMode ? (
        <TicketFormModal
          mode={modalMode}
          ticket={selectedTicket ?? undefined}
          teams={teams}
          defaultTeamId={filterTeamId ?? undefined}
          onClose={closeModal}
          onSaved={handleSaved}
        />
      ) : null}
    </div>
  )
}
