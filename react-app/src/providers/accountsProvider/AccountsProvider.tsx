import { useReducer, useEffect, useCallback, useState } from 'react'
import { accountsService } from '@/services'
import userReducer, { ActionType } from './reducer'
import { State, User } from './types'
import Loading from '@/components/loading'
import useIsMounted from '@/hooks/useIsMounted'
import useFetch from '@/hooks/useFetch'
import useCustomEventListener from '@/hooks/useCustomEventListener'
import events from '@/events'
import {
  openLoginModal,
  closeChangePasswordModal,
  loginModalSettings,
  closeLoginModal,
} from '@/modals'
import { Modal } from '@mantine/core'
import LoginModalLoader from '@/modals/components/loginModal/LoginModalLoader'
import noop from '@/utils/noop'
import { AccountsContext } from './index'
import useCrossTabEventListener, {
  dispatch as dispatchCrossTabEvent,
} from '@/hooks/useCrossTabEventListener'
import type { CrossTabEvent } from '@/hooks/useCrossTabEventListener'

const defaultState: State = {
  user: null,
}

function onUnAuthorized() {
  openLoginModal()
  dispatchCrossTabEvent(events.user.unAuthorized)
}

export interface AccountsProviderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function AccountsProvider({ children }: AccountsProviderProps) {
  const isMounted = useIsMounted()
  const [isLoading, setIsLoading] = useState(true)
  const [state, dispatch] = useReducer(userReducer, defaultState)
  const [isLoginModalOpened, setIsLoginModalOpened] = useState(false)
  const { request: getUserRequest } = useFetch(accountsService.getUser)
  const { request: loginRequest, isLoading: isLoginLoading } = useFetch(accountsService.login)
  const { request: logoutRequest, isLoading: isLogoutLoading } = useFetch(accountsService.logout)
  const { request: changePasswordRequest, isLoading: isChangePasswordLoading } = useFetch(
    accountsService.changePassword,
  )

  const onCrossTabEvent = useCallback(({ eventName, payload }: CrossTabEvent) => {
    if (eventName === events.user.unAuthorized) {
      openLoginModal()
    } else if (eventName === events.user.loggedIn) {
      if (payload && (payload as { user: User }).user) {
        const userData = (payload as { user: User }).user
        const user = {
          id: userData?.id ?? '',
          email: userData?.email ?? '',
        }
        dispatch({ type: ActionType.SetUser, payload: user })
        setIsLoginModalOpened(false)
        closeLoginModal()
        setIsLoading(false)
      }
    }
  }, [])

  useCustomEventListener(events.user.unAuthorized, onUnAuthorized)
  useCrossTabEventListener(onCrossTabEvent)

  useEffect(() => {
    getUserRequest(({ data }) => {
      if (!isMounted()) return

      if (data) {
        const user = {
          id: data.id ?? '',
          email: data.email ?? '',
        }
        dispatch({ type: ActionType.SetUser, payload: user })
        setIsLoading(false)
      } else {
        setIsLoginModalOpened(true)
      }
    })
  }, [isMounted, getUserRequest])

  const login = useCallback(
    (email: string, password: string) => {
      loginRequest({ email, password }, ({ data }) => {
        if (!isMounted()) return
        if (data) {
          const user = {
            id: data.id ?? '',
            email: data.email ?? '',
          }
          dispatch({ type: ActionType.SetUser, payload: user })
          setIsLoginModalOpened(false)
          closeLoginModal()
          setIsLoading(false)
          dispatchCrossTabEvent(events.user.loggedIn, { user })
        }
      })
    },
    [loginRequest, isMounted],
  )

  const changePassword = useCallback(
    (currentPassword: string, newPassword: string, passwordRepeat: string) => {
      changePasswordRequest({ currentPassword, newPassword, passwordRepeat }, ({ data }) => {
        if (data) {
          closeChangePasswordModal()
        }
      })
    },
    [changePasswordRequest],
  )

  const logout = useCallback(() => {
    logoutRequest((data) => {
      if (data) {
        window.location.reload()
      }
    })
  }, [logoutRequest])

  return (
    <AccountsContext.Provider
      value={{
        ...state,
        isLoginLoading,
        isLogoutLoading,
        isChangePasswordLoading,
        login,
        changePassword,
        logout,
      }}
    >
      {isLoading ? <Loading full /> : children}
      {isLoginModalOpened && (
        <Modal
          opened
          onClose={noop}
          {...loginModalSettings}
        >
          <LoginModalLoader />
        </Modal>
      )}
    </AccountsContext.Provider>
  )
}

export default AccountsProvider
