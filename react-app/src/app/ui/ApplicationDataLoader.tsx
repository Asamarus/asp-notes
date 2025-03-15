import { useState, useEffect } from 'react'
import { applicationDataApi } from '@/entities/applicationData'
import { setAllNotesSection, setSections } from '@/entities/section'
import useFetch from '@/shared/lib/useFetch'
import useAppDispatch from '@/shared/lib/useAppDispatch'

import Loading from '@/shared/ui/loading'

export interface ApplicationDataLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function ApplicationDataLoader({ children }: ApplicationDataLoaderProps) {
  const [isLoading, setIsLoading] = useState(true)
  const { request: getApplicationDataRequest } = useFetch(applicationDataApi.getApplicationData)
  const dispatch = useAppDispatch()

  useEffect(() => {
    getApplicationDataRequest(({ data }) => {
      if (data) {
        dispatch(setSections(data.sections))
        dispatch(setAllNotesSection(data.allNotesSection))
        setIsLoading(false)
      }
    })
  }, [getApplicationDataRequest, dispatch])

  return <> {isLoading ? <Loading full /> : children}</>
}

export default ApplicationDataLoader
