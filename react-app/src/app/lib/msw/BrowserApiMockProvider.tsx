import { useState, useEffect } from 'react'

async function enableMocking() {
  return false
  if (process.env.NODE_ENV !== 'development') {
    return
  }

  const { worker } = await import('./browser')

  return worker.start({ onUnhandledRequest: 'bypass' })
}

export interface BrowserApiMockProviderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function BrowserApiMockProvider({ children }: BrowserApiMockProviderProps) {
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    enableMocking().then(() => setLoading(false))
  }, [])

  if (loading) {
    return null
  }

  return <>{children}</>
}

export default BrowserApiMockProvider
