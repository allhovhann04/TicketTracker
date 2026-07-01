import { useEffect, useState, type FormEvent } from 'react'
import { createEpic, deleteEpic, listEpics, updateEpic } from '../../api/epics'
import { getErrorMessage } from '../../api/errors'
import { listTeams } from '../../api/teams'
import type { Epic } from '../../types/epic'
import type { Team } from '../../types/team'

export function EpicsPage() {
  const [teams, setTeams] = useState<Team[]>([])
  const [isLoadingTeams, setIsLoadingTeams] = useState(true)
  const [teamsError, setTeamsError] = useState<string | null>(null)
  const [selectedTeamId, setSelectedTeamId] = useState<string | null>(null)

  const [epics, setEpics] = useState<Epic[]>([])
  const [isLoadingEpics, setIsLoadingEpics] = useState(false)
  const [epicsError, setEpicsError] = useState<string | null>(null)

  const [newTitle, setNewTitle] = useState('')
  const [newDescription, setNewDescription] = useState('')
  const [createError, setCreateError] = useState<string | null>(null)
  const [isCreating, setIsCreating] = useState(false)

  const [editingId, setEditingId] = useState<string | null>(null)
  const [editingTitle, setEditingTitle] = useState('')
  const [editingDescription, setEditingDescription] = useState('')
  const [editError, setEditError] = useState<string | null>(null)
  const [isSavingEdit, setIsSavingEdit] = useState(false)

  const [deletingId, setDeletingId] = useState<string | null>(null)
  const [deleteError, setDeleteError] = useState<{ epicId: string; message: string } | null>(null)

  async function loadTeams() {
    setIsLoadingTeams(true)
    setTeamsError(null)

    try {
      const result = await listTeams()
      setTeams(result)
      setSelectedTeamId((current) => current ?? result[0]?.id ?? null)
    } catch (err) {
      setTeamsError(getErrorMessage(err, 'Failed to load teams.'))
    } finally {
      setIsLoadingTeams(false)
    }
  }

  async function loadEpics(teamId: string) {
    setIsLoadingEpics(true)
    setEpicsError(null)

    try {
      setEpics(await listEpics(teamId))
    } catch (err) {
      setEpicsError(getErrorMessage(err, 'Failed to load epics.'))
    } finally {
      setIsLoadingEpics(false)
    }
  }

  useEffect(() => {
    void loadTeams()
  }, [])

  useEffect(() => {
    if (selectedTeamId) {
      void loadEpics(selectedTeamId)
    } else {
      setEpics([])
    }
  }, [selectedTeamId])

  async function handleCreateSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!selectedTeamId) {
      return
    }

    setCreateError(null)
    setIsCreating(true)

    try {
      await createEpic({ teamId: selectedTeamId, title: newTitle, description: newDescription })
      setNewTitle('')
      setNewDescription('')
      await loadEpics(selectedTeamId)
    } catch (err) {
      setCreateError(getErrorMessage(err))
    } finally {
      setIsCreating(false)
    }
  }

  function startEditing(epic: Epic) {
    setEditingId(epic.id)
    setEditingTitle(epic.title)
    setEditingDescription(epic.description ?? '')
    setEditError(null)
  }

  function cancelEditing() {
    setEditingId(null)
    setEditingTitle('')
    setEditingDescription('')
    setEditError(null)
  }

  async function handleEditSubmit(event: FormEvent<HTMLFormElement>, epicId: string) {
    event.preventDefault()
    if (!selectedTeamId) {
      return
    }

    setEditError(null)
    setIsSavingEdit(true)

    try {
      await updateEpic(epicId, { title: editingTitle, description: editingDescription })
      cancelEditing()
      await loadEpics(selectedTeamId)
    } catch (err) {
      setEditError(getErrorMessage(err))
    } finally {
      setIsSavingEdit(false)
    }
  }

  async function handleDelete(epic: Epic) {
    if (!selectedTeamId) {
      return
    }

    if (!window.confirm(`Delete epic "${epic.title}"? This cannot be undone.`)) {
      return
    }

    setDeleteError(null)
    setDeletingId(epic.id)

    try {
      await deleteEpic(epic.id)
      await loadEpics(selectedTeamId)
    } catch (err) {
      setDeleteError({ epicId: epic.id, message: getErrorMessage(err, 'Failed to delete epic.') })
    } finally {
      setDeletingId(null)
    }
  }

  return (
    <div className="page">
      <div className="page-header">
        <h1>Epics</h1>
      </div>

      {isLoadingTeams ? <p>Loading teams…</p> : null}
      {teamsError ? <p className="error-message">{teamsError}</p> : null}

      {!isLoadingTeams && !teamsError && teams.length === 0 ? (
        <p className="empty-state">No teams yet. Create one on the Teams page first.</p>
      ) : null}

      {!isLoadingTeams && teams.length > 0 ? (
        <label className="field team-selector">
          <span>Team</span>
          <select
            value={selectedTeamId ?? ''}
            onChange={(event) => setSelectedTeamId(event.target.value || null)}
          >
            {teams.map((team) => (
              <option key={team.id} value={team.id}>
                {team.name}
              </option>
            ))}
          </select>
        </label>
      ) : null}

      {selectedTeamId ? (
        <>
          <form className="inline-form" onSubmit={handleCreateSubmit}>
            <input
              type="text"
              placeholder="Epic title"
              value={newTitle}
              onChange={(event) => setNewTitle(event.target.value)}
              required
            />
            <input
              type="text"
              placeholder="Description (optional)"
              value={newDescription}
              onChange={(event) => setNewDescription(event.target.value)}
            />
            <button type="submit" disabled={isCreating}>
              {isCreating ? 'Creating…' : 'Create epic'}
            </button>
          </form>
          {createError ? <p className="error-message">{createError}</p> : null}

          {isLoadingEpics ? <p>Loading epics…</p> : null}
          {epicsError ? <p className="error-message">{epicsError}</p> : null}

          {!isLoadingEpics && !epicsError && epics.length === 0 ? (
            <p className="empty-state">No epics yet for this team.</p>
          ) : null}

          {!isLoadingEpics && !epicsError && epics.length > 0 ? (
            <table className="table">
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Description</th>
                  <th>Modified</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {epics.map((epic) => (
                  <tr key={epic.id}>
                    {editingId === epic.id ? (
                      <td colSpan={4}>
                        <form className="inline-form" onSubmit={(event) => handleEditSubmit(event, epic.id)}>
                          <input
                            type="text"
                            value={editingTitle}
                            onChange={(event) => setEditingTitle(event.target.value)}
                            required
                            autoFocus
                          />
                          <input
                            type="text"
                            placeholder="Description (optional)"
                            value={editingDescription}
                            onChange={(event) => setEditingDescription(event.target.value)}
                          />
                          <button type="submit" disabled={isSavingEdit}>
                            {isSavingEdit ? 'Saving…' : 'Save'}
                          </button>
                          <button type="button" onClick={cancelEditing} disabled={isSavingEdit}>
                            Cancel
                          </button>
                        </form>
                        {editError ? <p className="error-message">{editError}</p> : null}
                      </td>
                    ) : (
                      <>
                        <td>{epic.title}</td>
                        <td>{epic.description ?? '—'}</td>
                        <td>{new Date(epic.updatedAt).toLocaleString()}</td>
                        <td>
                          <button type="button" onClick={() => startEditing(epic)}>
                            Edit
                          </button>
                          <button
                            type="button"
                            onClick={() => handleDelete(epic)}
                            disabled={deletingId === epic.id}
                          >
                            {deletingId === epic.id ? 'Deleting…' : 'Delete'}
                          </button>
                          {deleteError?.epicId === epic.id ? (
                            <p className="error-message">{deleteError.message}</p>
                          ) : null}
                        </td>
                      </>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          ) : null}
        </>
      ) : null}
    </div>
  )
}
