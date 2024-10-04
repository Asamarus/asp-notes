import { openModal, closeModal } from '@/shared/ui/modalsManager'

import SelectSectionForNewNoteModalLoader from './SelectSectionForNewNoteModalLoader'

const modalId = 'selectSectionForNewNoteModal'

function openSelectSectionForNewNoteModal() {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: 'Select section for new note',
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: <SelectSectionForNewNoteModalLoader />,
    },
  })
}

function closeSelectSectionForNewNoteModal() {
  closeModal(modalId)
}

export { openSelectSectionForNewNoteModal, closeSelectSectionForNewNoteModal }
