import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import { clearStoredToken, getStoredToken, setStoredToken } from './tokenStorage'

interface AuthContextValue {
  isAuthenticated: boolean
  login: (token: string) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => getStoredToken())

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: token !== null,
      login: (newToken: string) => {
        setStoredToken(newToken)
        setToken(newToken)
      },
      logout: () => {
        clearStoredToken()
        setToken(null)
      },
    }),
    [token],
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
