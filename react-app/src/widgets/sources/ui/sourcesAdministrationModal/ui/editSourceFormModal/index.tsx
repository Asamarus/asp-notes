import { openModal, closeModal } from '@/shared/ui/modalsManager'

import EditSourceFormModalLoader from './EditSourceFormModalLoader'

import type { NoteSource } from '@/entities/note'

const modalId = 'editSourceFormModal'

function openEditSourceFormModal(noteId: number, source: NoteSource) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: `Edit source: #${source.id}`,
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: (
        <EditSourceFormModalLoader
          noteId={noteId}
          source={source}
        />
      ),
    },
  })
}

function closeEditSourceFormModal() {
  closeModal(modalId)
}

export { openEditSourceFormModal, closeEditSourceFormModal }
