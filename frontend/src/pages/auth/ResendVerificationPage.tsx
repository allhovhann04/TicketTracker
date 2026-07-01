import { useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { resendVerification } from '../../api/auth'
import { getErrorMessage } from '../../api/errors'

export function ResendVerificationPage() {
  const [email, setEmail] = useState('')
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setMessage(null)
    setIsSubmitting(true)

    try {
      const result = await resendVerification({ email })
      setMessage(result.message)
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="auth-page">
      <h1>Resend verification email</h1>

      <form className="form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Email</span>
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="email"
            required
          />
        </label>

        {error ? <p className="error-message">{error}</p> : null}
        {message ? <p className="success-message">{message}</p> : null}

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Sending…' : 'Resend email'}
        </button>

        <p>
          <Link to="/login">Back to login</Link>
        </p>
      </form>
    </div>
  )
}
