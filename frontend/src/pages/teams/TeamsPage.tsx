import { useEffect, useState, type FormEvent } from 'react'
import { getErrorMessage } from '../../api/errors'
import { createTeam, deleteTeam, listTeams, updateTeam } from '../../api/teams'
import type { Team } from '../../types/team'

export function TeamsPage() {
  const [teams, setTeams] = useState<Team[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)

  const [newTeamName, setNewTeamName] = useState('')
  const [createError, setCreateError] = useState<string | null>(null)
  const [isCreating, setIsCreating] = useState(false)

  const [editingId, setEditingId] = useState<string | null>(null)
  const [editingName, setEditingName] = useState('')
  const [editError, setEditError] = useState<string | null>(null)
  const [isSavingEdit, setIsSavingEdit] = useState(false)

  const [deletingId, setDeletingId] = useState<string | null>(null)
  const [deleteError, setDeleteError] = useState<{ teamId: string; message: string } | null>(null)

  async function loadTeams() {
    setIsLoading(true)
    setLoadError(null)

    try {
      setTeams(await listTeams())
    } catch (err) {
      setLoadError(getErrorMessage(err, 'Failed to load teams.'))
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadTeams()
  }, [])

  async function handleCreateSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setCreateError(null)
    setIsCreating(true)

    try {
      await createTeam({ name: newTeamName })
      setNewTeamName('')
      await loadTeams()
    } catch (err) {
      setCreateError(getErrorMessage(err))
    } finally {
      setIsCreating(false)
    }
  }

  function startEditing(team: Team) {
    setEditingId(team.id)
    setEditingName(team.name)
    setEditError(null)
  }

  function cancelEditing() {
    setEditingId(null)
    setEditingName('')
    setEditError(null)
  }

  async function handleEditSubmit(event: FormEvent<HTMLFormElement>, teamId: string) {
    event.preventDefault()
    setEditError(null)
    setIsSavingEdit(true)

    try {
      await updateTeam(teamId, { name: editingName })
      cancelEditing()
      await loadTeams()
    } catch (err) {
      setEditError(getErrorMessage(err))
    } finally {
      setIsSavingEdit(false)
    }
  }

  async function handleDelete(team: Team) {
    if (!window.confirm(`Delete team "${team.name}"? This cannot be undone.`)) {
      return
    }

    setDeleteError(null)
    setDeletingId(team.id)

    try {
      await deleteTeam(team.id)
      await loadTeams()
    } catch (err) {
      setDeleteError({ teamId: team.id, message: getErrorMessage(err, 'Failed to delete team.') })
    } finally {
      setDeletingId(null)
    }
  }

  return (
    <div className="page">
      <div className="page-header">
        <h1>Teams</h1>
      </div>

      <form className="inline-form" onSubmit={handleCreateSubmit}>
        <input
          type="text"
          placeholder="e.g. Platform Engineering"
          value={newTeamName}
          onChange={(event) => setNewTeamName(event.target.value)}
          required
        />
        <button type="submit" disabled={isCreating}>
          {isCreating ? 'Creating…' : 'Create team'}
        </button>
      </form>
      {createError ? <p className="error-message">{createError}</p> : null}

      {isLoading ? <p>Loading teams…</p> : null}
      {loadError ? <p className="error-message">{loadError}</p> : null}

      {!isLoading && !loadError && teams.length === 0 ? (
        <p className="empty-state">No teams yet. Create one to get started.</p>
      ) : null}

      {!isLoading && !loadError && teams.length > 0 ? (
        <table className="table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Created</th>
              <th>Modified</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {teams.map((team) => (
              <tr key={team.id}>
                {editingId === team.id ? (
                  <td colSpan={4}>
                    <form className="inline-form" onSubmit={(event) => handleEditSubmit(event, team.id)}>
                      <input
                        type="text"
                        value={editingName}
                        onChange={(event) => setEditingName(event.target.value)}
                        required
                        autoFocus
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
                    <td>{team.name}</td>
                    <td>{new Date(team.createdAt).toLocaleString()}</td>
                    <td>{new Date(team.updatedAt).toLocaleString()}</td>
                    <td>
                      <button type="button" onClick={() => startEditing(team)}>
                        Edit
                      </button>
                      <button
                        type="button"
                        onClick={() => handleDelete(team)}
                        disabled={deletingId === team.id}
                      >
                        {deletingId === team.id ? 'Deleting…' : 'Delete'}
                      </button>
                      {deleteError?.teamId === team.id ? (
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
    </div>
  )
}
