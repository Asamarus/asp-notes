import { openModal, closeModal } from '@/shared/ui/modalsManager'

import ChangeSectionModalLoader from './ChangeSectionModalLoader'

const modalId = 'changeSectionModal'

function openChangeSectionModal(noteId: number, closeNoteModal?: () => void) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: `Change note #${noteId} section`,
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: (
        <ChangeSectionModalLoader
          noteId={noteId}
          closeNoteModal={closeNoteModal}
        />
      ),
    },
  })
}

function closeChangeSectionModal() {
  closeModal(modalId)
}

export { openChangeSectionModal, closeChangeSectionModal }
