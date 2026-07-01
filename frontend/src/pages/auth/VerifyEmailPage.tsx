import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { verifyEmail } from '../../api/auth'
import { getErrorMessage } from '../../api/errors'

type VerificationStatus = 'loading' | 'success' | 'error'

export function VerifyEmailPage() {
  const [searchParams] = useSearchParams()
  const token = searchParams.get('token')

  const [status, setStatus] = useState<VerificationStatus>('loading')
  const [message, setMessage] = useState('')

  useEffect(() => {
    if (!token) {
      setStatus('error')
      setMessage('This verification link is missing its token.')
      return
    }

    let isCancelled = false

    verifyEmail(token)
      .then((result) => {
        if (!isCancelled) {
          setStatus('success')
          setMessage(result.message)
        }
      })
      .catch((error: unknown) => {
        if (!isCancelled) {
          setStatus('error')
          setMessage(getErrorMessage(error, 'This verification link is invalid or has expired.'))
        }
      })

    return () => {
      isCancelled = true
    }
  }, [token])

  return (
    <div className="auth-page">
      <h1>Email verification</h1>

      {status === 'loading' ? <p>Verifying your email…</p> : null}

      {status === 'success' ? (
        <div className="success-message">
          <p>{message}</p>
          <Link to="/login">Continue to login</Link>
        </div>
      ) : null}

      {status === 'error' ? (
        <div className="error-message">
          <p>{message}</p>
          <Link to="/resend-verification">Resend verification email</Link>
        </div>
      ) : null}
    </div>
  )
}
