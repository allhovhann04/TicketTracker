import { useCallback, useEffect, useState, type FormEvent } from 'react'
import { createComment, listComments } from '../api/comments'
import { getErrorMessage } from '../api/errors'
import { useAuth } from '../auth/AuthContext'
import type { Comment } from '../types/comment'

interface CommentsSectionProps {
  ticketId: string
}

export function CommentsSection({ ticketId }: CommentsSectionProps) {
  const { userId } = useAuth()

  const [comments, setComments] = useState<Comment[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)

  const [newComment, setNewComment] = useState('')
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const loadComments = useCallback(async () => {
    setIsLoading(true)
    setLoadError(null)

    try {
      // Oldest first, as required - the backend already returns them in this order.
      setComments(await listComments(ticketId))
    } catch (err) {
      setLoadError(getErrorMessage(err, 'Failed to load comments.'))
    } finally {
      setIsLoading(false)
    }
  }, [ticketId])

  useEffect(() => {
    void loadComments()
  }, [loadComments])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSubmitError(null)
    setIsSubmitting(true)

    try {
      await createComment(ticketId, { body: newComment })
      setNewComment('')
      await loadComments()
    } catch (err) {
      setSubmitError(getErrorMessage(err))
    } finally {
      setIsSubmitting(false)
    }
  }

  function authorLabel(authorUserId: string): string {
    // The backend only returns the author's user id, not a display name.
    if (userId && authorUserId === userId) {
      return 'You'
    }
    return `User ${authorUserId.slice(0, 8)}`
  }

  return (
    <div className="comments">
      <h3>Comments</h3>

      {isLoading ? <p>Loading comments…</p> : null}
      {loadError ? <p className="error-message">{loadError}</p> : null}

      {!isLoading && !loadError && comments.length === 0 ? (
        <p className="empty-state">No comments yet.</p>
      ) : null}

      {!isLoading && !loadError && comments.length > 0 ? (
        <ul className="comment-list">
          {comments.map((comment) => (
            <li key={comment.id} className="comment">
              <div className="comment-meta">
                <span className="comment-author">{authorLabel(comment.authorUserId)}</span>
                <span className="comment-date">{new Date(comment.createdAt).toLocaleString()}</span>
              </div>
              <p className="comment-body">{comment.body}</p>
            </li>
          ))}
        </ul>
      ) : null}

      <form className="inline-form" onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Write a comment…"
          value={newComment}
          onChange={(event) => setNewComment(event.target.value)}
          required
        />
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Posting…' : 'Post comment'}
        </button>
      </form>
      {submitError ? <p className="error-message">{submitError}</p> : null}
    </div>
  )
}
