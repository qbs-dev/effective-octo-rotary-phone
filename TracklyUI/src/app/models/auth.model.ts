export interface AuthRequest {
  email: string,
  password: string,
  fingerprint: string
}

export interface AuthResponse {
  message: string;
  accessToken: string;
}
