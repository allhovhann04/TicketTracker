import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import { decodeJwtPayload } from './jwt'
import { clearStoredToken, getStoredToken, setStoredToken } from './tokenStorage'

interface AuthContextValue {
  isAuthenticated: boolean
  userId: string | null
  login: (token: string) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => getStoredToken())

  const userId = useMemo(() => {
    if (!token) {
      return null
    }

    const payload = decodeJwtPayload(token)
    return typeof payload?.sub === 'string' ? payload.sub : null
  }, [token])

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: token !== null,
      userId,
      login: (newToken: string) => {
        setStoredToken(newToken)
        setToken(newToken)
      },
      logout: () => {
        clearStoredToken()
        setToken(null)
      },
    }),
    [token, userId],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
