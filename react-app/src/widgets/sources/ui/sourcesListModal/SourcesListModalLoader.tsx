import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const SourcesListModal = lazy(() => import('./SourcesListModal'))

export interface SourcesListModalLoaderProps {
  noteId: number
}

function SourcesListModalLoader({ noteId }: SourcesListModalLoaderProps) {
  return (
    <ComponentLoader>
      <SourcesListModal noteId={noteId} />
    </ComponentLoader>
  )
}

export default SourcesListModalLoader
