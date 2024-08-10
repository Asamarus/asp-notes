import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface User {
  id: string
  email: string
}

type LoadingState =
  | 'isLoading'
  | 'isGetUserLoading'
  | 'isLoginLoading'
  | 'isLogoutLoading'
  | 'isChangePasswordLoading'

type State = {
  isLoading: boolean
  isGetUserLoading: boolean
  isLoginLoading: boolean
  isLogoutLoading: boolean
  isChangePasswordLoading: boolean
  isLoginModalOpened: boolean
  user: User | null
}

type Actions = {
  setIsLoading: (stateName: LoadingState, isLoading: boolean) => void
  setUser: (user: User | null) => void
  setIsLoginModalOpened: (isLoginModalOpened: boolean) => void
}

export default create<State & Actions>()(
  immer((set) => ({
    isLoading: true,
    isGetUserLoading: false,
    isLoginLoading: false,
    isLogoutLoading: false,
    isChangePasswordLoading: false,
    isLoginModalOpened: false,
    user: null,
    setIsLoading: (stateName, isLoading) => {
      set((state) => {
        state[stateName] = isLoading
      })
    },
    setUser: (user) => {
      set((state) => {
        state.user = user
      })
    },
    setIsLoginModalOpened: (isLoginModalOpened) => {
      set({ isLoginModalOpened })
    },
  })),
)
