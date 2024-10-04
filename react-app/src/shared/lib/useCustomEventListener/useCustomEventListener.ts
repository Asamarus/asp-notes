import { useCallback, useEffect } from 'react'

export function dispatch(eventName: string, payload?: unknown) {
  const event = new CustomEvent(eventName, { detail: payload })

  document.dispatchEvent(event)
}

function useCustomEventListener(eventName: string, listener: (payload: unknown) => void) {
  const handleEvent = useCallback(
    (event: Event) => {
      listener?.((event as CustomEvent).detail)
    },
    [listener],
  )

  useEffect(() => {
    document.addEventListener(eventName, handleEvent)
    return () => document.removeEventListener(eventName, handleEvent)
  }, [eventName, listener, handleEvent])
}

export default useCustomEventListener
