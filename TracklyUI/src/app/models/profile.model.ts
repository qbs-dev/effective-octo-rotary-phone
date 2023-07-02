export interface ProfileBase {
  firstName: string,
  lastName: string,
  middleName: string,
}

export interface Profile extends ProfileBase {
  id: number,
  email: string,
  isEmailVerified: boolean,
  registrationDate: Date
  lastAccessDate: Date
}

export interface RegisterRequest extends ProfileBase {
  email: string,
  password: string,
}

