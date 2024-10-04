import { openModal, closeModal } from '@/shared/ui/modalsManager'

import ChangeSectionModalLoader from './ChangeSectionModalLoader'

const modalId = 'changeSectionModal'

function openChangeSectionModal(noteId: number) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: `Change note #${noteId} section`,
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: <ChangeSectionModalLoader noteId={noteId} />,
    },
  })
}

function closeChangeSectionModal() {
  closeModal(modalId)
}

export { openChangeSectionModal, closeChangeSectionModal }
