import { lazy } from 'react'

import { NoteLoader } from '@/entities/note'
import ComponentLoader from '@/shared/ui/componentLoader'

const Note = lazy(() => import('@/widgets/note'))

export interface NoteModalProps {
  id: number
  tab?: 'view' | 'edit' | 'delete'
  onModalClose(): void
}

function NoteModal({ id, tab, onModalClose }: NoteModalProps) {
  return (
    <NoteLoader id={id}>
      <ComponentLoader>
        <div style={{ height: `calc(100vh - 10dvh)` }}>
          <Note
            id={id}
            tab={tab}
            onClose={onModalClose}
            displayType="modal"
          />
        </div>
      </ComponentLoader>
    </NoteLoader>
  )
}

export default NoteModal
