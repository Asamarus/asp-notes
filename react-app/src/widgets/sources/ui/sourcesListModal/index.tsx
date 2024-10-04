import { openModal, closeModal } from '@/shared/ui/modalsManager'

import SourcesListModalLoader from './SourcesListModalLoader'

const modalId = 'sourcesListModal'

function openSourcesListModal(noteId: number) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: '',
      closeOnClickOutside: true,
      closeOnEscape: true,
      withCloseButton: false,
      centered: true,
      size: 400,
    },
    data: {
      children: <SourcesListModalLoader noteId={noteId} />,
    },
  })
}

function closeSourcesListModal() {
  closeModal(modalId)
}

export { openSourcesListModal, closeSourcesListModal }
