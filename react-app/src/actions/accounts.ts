import createFetch from '@/utils/createFetch'
import { accountsService } from '@/services'
import { useAccountsStore } from '@/store'
import events from '@/events'
import { closeChangePasswordModal, closeLoginModal, openLoginModal } from '@/modals'
import { CrossTabEvent, dispatch as dispatchCrossTabEvent } from '@/hooks/useCrossTabEventListener'

import type { User } from '@/store/accounts'

const getUserRequest = createFetch(accountsService.getUser, (isLoading) => {
  useAccountsStore.getState().setIsLoading('isGetUserLoading', isLoading)
})

const loginRequest = createFetch(accountsService.login, (isLoading) => {
  useAccountsStore.getState().setIsLoading('isLoginLoading', isLoading)
})

const logoutRequest = createFetch(accountsService.logout, (isLoading) => {
  useAccountsStore.getState().setIsLoading('isLogoutLoading', isLoading)
})

const changePasswordRequest = createFetch(accountsService.changePassword, (isLoading) => {
  useAccountsStore.getState().setIsLoading('isChangePasswordLoading', isLoading)
})

function getUser() {
  getUserRequest(({ data }) => {
    if (data) {
      const user = {
        id: data.id ?? '',
        email: data.email ?? '',
      }
      useAccountsStore.getState().setUser(user)
      closeLoginModal()
      dispatchCrossTabEvent(events.user.loggedIn, { user })
      useAccountsStore.getState().setIsLoading('isLoading', false)
    } else {
      useAccountsStore.getState().setIsLoginModalOpened(true)
    }
  })
}

function login(email: string, password: string) {
  loginRequest({ email, password }, ({ data }) => {
    if (data) {
      const user = {
        id: data.id ?? '',
        email: data.email ?? '',
      }

      useAccountsStore.getState().setIsLoginModalOpened(false)
      useAccountsStore.getState().setUser(user)
      closeLoginModal()
      dispatchCrossTabEvent(events.user.loggedIn, { user })
      useAccountsStore.getState().setIsLoading('isLoading', false)
    }
  })
}

function logout() {
  logoutRequest((data) => {
    if (data) {
      window.location.reload()
    }
  })
}

function changePassword(currentPassword: string, newPassword: string, passwordRepeat: string) {
  changePasswordRequest({ currentPassword, newPassword, passwordRepeat }, ({ data }) => {
    if (data) {
      closeChangePasswordModal()
    }
  })
}

const handleCrossTabEvent = ({ eventName, payload }: CrossTabEvent) => {
  if (eventName === events.user.unAuthorized) {
    openLoginModal()
  } else if (eventName === events.user.loggedIn) {
    if (payload && (payload as { user: User }).user) {
      const userData = (payload as { user: User }).user
      const user = {
        id: userData?.id ?? '',
        email: userData?.email ?? '',
      }
      useAccountsStore.getState().setIsLoading('isLoading', false)
      useAccountsStore.getState().setIsLoginModalOpened(false)
      useAccountsStore.getState().setUser(user)
      closeLoginModal()
    }
  }
}

function handleUnAuthorizedEvent() {
  openLoginModal()
  dispatchCrossTabEvent(events.user.unAuthorized)
}

export default {
  getUser,
  login,
  logout,
  changePassword,
  handleCrossTabEvent,
  handleUnAuthorizedEvent,
}
