import { useEffect } from 'react'
import { useApplicationStore } from '@/store'
import { applicationActions } from '@/actions'

import Loading from '@/components/loading'

export interface ApplicationLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function ApplicationLoader({ children }: ApplicationLoaderProps) {
  const isLoading = useApplicationStore((state) => state.isLoading)

  useEffect(() => {
    applicationActions.getInitialData()
  }, [])

  return isLoading ? <Loading full /> : children
}

export default ApplicationLoader
