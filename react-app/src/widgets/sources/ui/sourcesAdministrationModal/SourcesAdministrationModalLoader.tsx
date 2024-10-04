import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const SourcesAdministrationModal = lazy(() => import('./SourcesAdministrationModal'))

export interface SourcesAdministrationModalLoaderProps {
  noteId: number
}

function SourcesAdministrationModalLoader({ noteId }: SourcesAdministrationModalLoaderProps) {
  return (
    <ComponentLoader>
      <SourcesAdministrationModal noteId={noteId} />
    </ComponentLoader>
  )
}

export default SourcesAdministrationModalLoader
