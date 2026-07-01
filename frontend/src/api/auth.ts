import { apiClient } from './client'
import type {
  LoginRequest,
  LoginResponse,
  MessageResponse,
  RegisterRequest,
  ResendVerificationRequest,
} from '../types/auth'

export async function register(request: RegisterRequest): Promise<MessageResponse> {
  const response = await apiClient.post<MessageResponse>('/api/auth/register', request)
  return response.data
}

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const response = await apiClient.post<LoginResponse>('/api/auth/login', request)
  return response.data
}

export async function verifyEmail(token: string): Promise<MessageResponse> {
  const response = await apiClient.get<MessageResponse>('/api/auth/verify-email', {
    params: { token },
  })
  return response.data
}

export async function resendVerification(request: ResendVerificationRequest): Promise<MessageResponse> {
  const response = await apiClient.post<MessageResponse>('/api/auth/resend-verification', request)
  return response.data
}
