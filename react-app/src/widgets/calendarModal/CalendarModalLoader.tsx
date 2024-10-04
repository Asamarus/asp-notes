import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const CalendarModal = lazy(() => import('./CalendarModal'))

function CalendarModalLoader() {
  return (
    <ComponentLoader>
      <CalendarModal />
    </ComponentLoader>
  )
}

export default CalendarModalLoader
