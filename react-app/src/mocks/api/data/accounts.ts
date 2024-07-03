import { components } from '@/misc/openapi'

export const mockUser = {
  id: '1',
  email: 'user@mail.com',
  password: '123456',
}

export const loginRequestMock: components['schemas']['LoginRequest'] = {
  email: mockUser.email,
  password: mockUser.password,
}

export const loginResponseMock: components['schemas']['LoginResponse'] = {
  id: mockUser.id,
  email: mockUser.email,
}

export const getUserResponseMock: components['schemas']['UserResponse'] = {
  id: mockUser.id,
  email: mockUser.email,
}

export const logoutResponseMock: components['schemas']['SuccessResponse'] = {
  message: 'You are logged out!',
  showNotification: false,
}

export const changePasswordRequestMock: components['schemas']['ChangePasswordRequest'] = {
  currentPassword: '123456',
  newPassword: '123456',
  passwordRepeat: '123456',
}

export const changePasswordResponseMock: components['schemas']['SuccessResponse'] = {
  message: 'Password is changed!',
}
