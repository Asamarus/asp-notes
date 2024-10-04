import { useEffect, useState, useCallback } from 'react'
import { events } from '@/shared/config'
import noop from '@/shared/lib/noop'
import useCustomEventListener from '@/shared/lib/useCustomEventListener'
import useCrossTabEventListener, {
  dispatchCrossTabEvent,
} from '@/shared/lib/useCrossTabEventListener'
import { openLoginModal, loginModalSettings, closeLoginModal } from '@/widgets/loginModal'
import useFetch from '@/shared/lib/useFetch'
import { usersApi, setUser } from '@/entities/user'
import { setSections } from '@/entities/section'
import { setNote, removeNote } from '@/entities/note'
import useAppDispatch from '@/shared/lib/useAppDispatch'

import { Modal } from '@mantine/core'
import LoginModalLoader from '@/widgets/loginModal/LoginModalLoader'
import Loading from '@/shared/ui/loading'

import type { User } from '@/entities/user'
import type { CrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import type { Section } from '@/entities/section'
import type { Note } from '@/entities/note'

export interface ApplicationLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}

function handleUnAuthorizedEvent() {
  openLoginModal()
  dispatchCrossTabEvent(events.user.unAuthorized)
}

function ApplicationLoader({ children }: ApplicationLoaderProps) {
  const [isLoading, setIsLoading] = useState(true)
  const [isLoginModalOpened, setIsLoginModalOpened] = useState(false)
  const dispatch = useAppDispatch()

  const handleCrossTabEvent = useCallback(
    ({ eventName, payload }: CrossTabEvent) => {
      switch (eventName) {
        case events.user.unAuthorized:
          openLoginModal()
          break
        case events.user.loggedIn:
          if (payload && (payload as { user: User }).user) {
            const userData = (payload as { user: User }).user
            const user = {
              id: userData?.id ?? '',
              email: userData?.email ?? '',
            }
            dispatch(setUser(user))
            setIsLoading(false)
            setIsLoginModalOpened(false)
            closeLoginModal()
          }
          break
        case events.sections.updated:
          if (payload && (payload as Section[])) {
            const newSections = payload as Section[]
            dispatch(setSections(newSections))
          }
          break
        case events.note.updated:
          if (payload && (payload as Note)) {
            const newNote = payload as Note
            dispatch(setNote({ id: newNote.id, note: newNote }))
          }
          break
        case events.note.deleted:
          if (payload && typeof payload === 'number') {
            dispatch(removeNote(payload))
          }
          break
      }
    },
    [dispatch],
  )

  const handleUserLoggedInEvent = useCallback(
    (payload: unknown) => {
      if (payload && (payload as { user: User }).user) {
        const userData = (payload as { user: User }).user
        const user = {
          id: userData?.id ?? '',
          email: userData?.email ?? '',
        }
        dispatch(setUser(user))
        setIsLoading(false)
        setIsLoginModalOpened(false)
      }
    },
    [dispatch],
  )

  useCustomEventListener(events.user.unAuthorized, handleUnAuthorizedEvent)
  useCustomEventListener(events.user.loggedIn, handleUserLoggedInEvent)
  useCrossTabEventListener(handleCrossTabEvent)
  const { request: getUserRequest } = useFetch(usersApi.getUser)

  useEffect(() => {
    getUserRequest(({ data }) => {
      if (data) {
        const user = {
          id: data.id ?? '',
          email: data.email ?? '',
        }
        dispatch(setUser(user))

        closeLoginModal()
        dispatchCrossTabEvent(events.user.loggedIn, { user })
        setIsLoading(false)
      } else {
        setIsLoginModalOpened(true)
      }
    })
  }, [getUserRequest, dispatch])

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

export default ApplicationLoader
