import axios from 'axios'
import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { login } from '../../api/auth'
import { getErrorMessage } from '../../api/errors'
import { useAuth } from '../../auth/AuthContext'

export function LoginPage() {
  const { login: storeToken } = useAuth()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [showResendLink, setShowResendLink] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setShowResendLink(false)
    setIsSubmitting(true)

    try {
      const result = await login({ email, password })
      storeToken(result.accessToken)
      navigate('/', { replace: true })
    } catch (err) {
      setError(getErrorMessage(err))
      if (axios.isAxiosError(err) && err.response?.status === 403) {
        setShowResendLink(true)
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="auth-page">
      <h1>Log in</h1>

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

        <label className="field">
          <span>Password</span>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            required
          />
        </label>

        {error ? <p className="error-message">{error}</p> : null}
        {showResendLink ? (
          <p>
            <Link to="/resend-verification">Resend verification email</Link>
          </p>
        ) : null}

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Logging in…' : 'Log in'}
        </button>

        <p>
          Don't have an account? <Link to="/register">Create an account</Link>
        </p>
      </form>
    </div>
  )
}
