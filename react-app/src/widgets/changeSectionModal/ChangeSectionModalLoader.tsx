import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const ChangeSectionModal = lazy(() => import('./ChangeSectionModal'))

export interface ChangeSectionModalLoaderProps {
  noteId: number
}

function ChangeSectionModalLoader({ noteId }: ChangeSectionModalLoaderProps) {
  return (
    <ComponentLoader>
      <ChangeSectionModal noteId={noteId} />
    </ComponentLoader>
  )
}

export default ChangeSectionModalLoader
