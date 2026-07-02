import { cleanup, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { afterEach, describe, expect, it, vi } from 'vitest'
import * as authApi from '../../api/auth'
import { AuthProvider } from '../../auth/AuthContext'
import { LoginPage } from './LoginPage'

vi.mock('../../api/auth')

function renderLoginPage() {
  return render(
    <AuthProvider>
      <MemoryRouter initialEntries={['/login']}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/" element={<div>Board placeholder</div>} />
        </Routes>
      </MemoryRouter>
    </AuthProvider>,
  )
}

afterEach(() => {
  cleanup()
  localStorage.clear()
  vi.clearAllMocks()
})

describe('LoginPage', () => {
  it('shows the backend error message and a resend link when login fails with an unverified email', async () => {
    const user = userEvent.setup()
    vi.mocked(authApi.login).mockRejectedValue({
      isAxiosError: true,
      response: { status: 403, data: { message: 'Email address has not been verified.' } },
    })

    renderLoginPage()

    await user.type(screen.getByLabelText('Email'), 'test@example.com')
    await user.type(screen.getByLabelText('Password'), 'password123')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByText('Email address has not been verified.')).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Resend verification email' })).toBeInTheDocument()
  })

  it('stores the token and navigates to the board after a successful login', async () => {
    const user = userEvent.setup()
    vi.mocked(authApi.login).mockResolvedValue({
      accessToken: 'test-access-token',
      expiresAt: new Date(Date.now() + 60_000).toISOString(),
    })

    renderLoginPage()

    await user.type(screen.getByLabelText('Email'), 'test@example.com')
    await user.type(screen.getByLabelText('Password'), 'password123')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByText('Board placeholder')).toBeInTheDocument()
    expect(localStorage.getItem('ticketTracker.accessToken')).toBe('test-access-token')
  })
})
