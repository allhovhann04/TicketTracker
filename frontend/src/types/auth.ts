export interface RegisterRequest {
  email: string
  password: string
  displayName: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  expiresAt: string
}

export interface ResendVerificationRequest {
  email: string
}

export interface MessageResponse {
  message: string
}
