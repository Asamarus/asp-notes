import { useEffect } from 'react'
import { useCalendarStore } from '@/store'

export interface CalendarLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function CalendarLoader({ children }: CalendarLoaderProps) {
  useEffect(() => {
    useCalendarStore.getState().setIsMounted(true)

    return () => {
      useCalendarStore.getState().setIsMounted(false)
      useCalendarStore.getState().reset()
    }
  }, [])

  return children
}

export default CalendarLoader
