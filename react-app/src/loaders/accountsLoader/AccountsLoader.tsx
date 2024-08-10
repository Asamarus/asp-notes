import { useEffect } from 'react'
import { useAccountsStore } from '@/store'
import { accountsActions } from '@/actions'
import useCustomEventListener from '@/hooks/useCustomEventListener'
import events from '@/events'
import noop from '@/utils/noop'
import useCrossTabEventListener from '@/hooks/useCrossTabEventListener'
import { loginModalSettings } from '@/modals'

import { Modal } from '@mantine/core'
import LoginModalLoader from '@/modals/components/loginModal/LoginModalLoader'
import Loading from '@/components/loading'

export interface AccountsLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}

function AccountsLoader({ children }: AccountsLoaderProps) {
  useCustomEventListener(events.user.unAuthorized, accountsActions.handleUnAuthorizedEvent)
  useCrossTabEventListener(accountsActions.handleCrossTabEvent)

  useEffect(() => {
    accountsActions.getUser()
  }, [])

  const isLoading = useAccountsStore((state) => state.isLoading)
  const isLoginModalOpened = useAccountsStore((state) => state.isLoginModalOpened)

  return (
    <>
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
    </>
  )
}

export default AccountsLoader
