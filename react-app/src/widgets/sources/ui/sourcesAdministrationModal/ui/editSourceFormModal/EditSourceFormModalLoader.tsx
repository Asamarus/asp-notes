import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

import type { NoteSource } from '@/entities/note'

const EditSourceFormModal = lazy(() => import('./EditSourceFormModal'))

export interface EditSourceFormModalLoaderProps {
  noteId: number
  source: NoteSource
}

function EditSourceFormModalLoader({ noteId, source }: EditSourceFormModalLoaderProps) {
  return (
    <ComponentLoader>
      <EditSourceFormModal
        noteId={noteId}
        source={source}
      />
    </ComponentLoader>
  )
}

export default EditSourceFormModalLoader
