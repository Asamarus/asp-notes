import { Text, Progress } from '@mantine/core'
import { useEffect } from 'react'

Progress

export interface LoadingModalProps {
  message?: string
}
function LoadingModal(props: LoadingModalProps) {
  const { message = 'Loading...' } = props

  useEffect(() => {
    const listener = (event: BeforeUnloadEvent) => {
      // Cancel the event as stated by the standard.
      event.preventDefault()
      // Included for legacy support, e.g. Chrome/Edge < 119
      event.returnValue = true
    }
    window.addEventListener('beforeunload', listener)
    return () => window.removeEventListener('beforeunload', listener)
  }, [])

  return (
    <>
      <Text mb={10}>{message}</Text>
      <Progress
        value={100}
        size="xl"
        animated
      />
    </>
  )
}

export default LoadingModal
