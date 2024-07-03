import { useReducer, useEffect, useState } from 'react'
import { applicationService } from '@/services'
import applicationReducer, { ActionType } from './reducer'
import { State } from './types'
import Loading from '@/components/loading'
import useIsMounted from '@/hooks/useIsMounted'
import useFetch from '@/hooks/useFetch'

import { ApplicationContext } from './index'

const defaultState: State = {
  initialData: null,
}

export interface ApplicationProviderProps {
  /** The content of the component */
  children?: React.ReactNode
}

function ApplicationProvider({ children }: ApplicationProviderProps) {
  const isMounted = useIsMounted()
  const [isLoading, setIsLoading] = useState(true)
  const [state, dispatch] = useReducer(applicationReducer, defaultState)
  const { request: getInitialDataRequest } = useFetch(applicationService.getInitialData)

  useEffect(() => {
    getInitialDataRequest(({ data }) => {
      if (data && isMounted()) {
        const initialData = {
          title: data.title ?? '',
          someData: data.someData ?? '',
        }
        dispatch({ type: ActionType.SetInitialData, payload: initialData })
        setIsLoading(false)
      }
    })
  }, [isMounted, getInitialDataRequest])

  if (isLoading) {
    return <Loading full />
  }

  return <ApplicationContext.Provider value={{ ...state }}>{children}</ApplicationContext.Provider>
}

export default ApplicationProvider
