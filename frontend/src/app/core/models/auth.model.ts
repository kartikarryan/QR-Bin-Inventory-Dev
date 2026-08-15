export interface ManagerLoginRequest {
  email: string;
  password: string;
}

export interface ManagerProfile {
  id: number;
  email: string;
  fullName: string;
  organizationId: number;
  organizationName: string;
}

export interface ManagerLoginResponse {
  token: string;
  expiresAt: string;
  manager: ManagerProfile;
}
