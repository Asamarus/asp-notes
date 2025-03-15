import type { components } from '@/shared/api'

export const mockUser = {
  id: 1,
  email: 'user@mail.com',
  password: '123456',
}

export const loginRequestMock: components['schemas']['UsersLoginRequest'] = {
  email: mockUser.email,
  password: mockUser.password,
}

export const loginResponseMock: components['schemas']['UsersLoginResponse'] = {
  id: mockUser.id,
  email: mockUser.email,
}

export const getUserResponseMock: components['schemas']['UsersCurrentUserResponse'] = {
  id: mockUser.id.toString(),
  email: mockUser.email,
}

export const changePasswordRequestMock: components['schemas']['UsersChangePasswordRequest'] = {
  currentPassword: '123456',
  newPassword: '123456',
  passwordRepeat: '123456',
}
