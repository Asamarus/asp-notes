import { useCallback, useEffect } from 'react'

export const STORAGE_EVENT = 'cross-tab-event'

export type CrossTabEvent = {
  eventName: string
  payload?: unknown
}

export function dispatch(eventName: string, payload?: unknown) {
  try {
    localStorage.setItem(STORAGE_EVENT, JSON.stringify({ eventName, payload }))
    localStorage.removeItem(STORAGE_EVENT)
  } catch (error) {
    console.error('Error dispatching cross tab event:', error)
  }
}

function useCrossTabEventListener(listener: ({ eventName, payload }: CrossTabEvent) => void) {
  const handleEvent = useCallback(
    (event: Event) => {
      if ((event as StorageEvent).key !== STORAGE_EVENT) return

      try {
        const message = JSON.parse((event as StorageEvent).newValue || '{}')

        if (message.eventName === undefined) return

        if (message.payload === undefined) {
          message.payload = null
        }

        listener?.(message)
      } catch (error) {
        console.error('Error handling cross tab event:', error)
      }
    },
    [listener],
  )

  useEffect(() => {
    window.addEventListener('storage', handleEvent)
    return () => window.removeEventListener('storage', handleEvent)
  }, [listener, handleEvent])
}

export default useCrossTabEventListener
