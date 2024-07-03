export interface State {
  user: User | null
}

export interface Context extends State {
  isLoginLoading: boolean
  isChangePasswordLoading: boolean
  isLogoutLoading: boolean
  login: (email: string, password: string) => void
  changePassword: (currentPassword: string, newPassword: string, passwordRepeat: string) => void
  logout: () => void
}

export interface User {
  id: string
  email: string
}
