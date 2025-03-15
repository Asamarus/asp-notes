import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const ChangeSectionModal = lazy(() => import('./ChangeSectionModal'))

export interface ChangeSectionModalLoaderProps {
  noteId: number
  closeNoteModal?: () => void
}

function ChangeSectionModalLoader({ noteId, closeNoteModal }: ChangeSectionModalLoaderProps) {
  return (
    <ComponentLoader>
      <ChangeSectionModal
        noteId={noteId}
        closeNoteModal={closeNoteModal}
      />
    </ComponentLoader>
  )
}

export default ChangeSectionModalLoader
