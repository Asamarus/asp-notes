import { openModal, closeModal } from '@/shared/ui/modalsManager'

import AddNewSourceFormModalLoader from './AddNewSourceFormModalLoader'

const modalId = 'addNewSourceFormModal'

function openAddNewSourceFormModal(noteId: number) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: 'Add new source',
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: <AddNewSourceFormModalLoader noteId={noteId} />,
    },
  })
}

function closeAddNewSourceFormModal() {
  closeModal(modalId)
}

export { openAddNewSourceFormModal, closeAddNewSourceFormModal }
