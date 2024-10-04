import { openModal, closeModal } from '@/shared/ui/modalsManager'

import SourcesAdministrationModalLoader from './SourcesAdministrationModalLoader'

const modalId = 'sourcesAdministrationModal'

function openSourcesAdministrationModal(noteId: number) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: `Edit note #${noteId} sources`,
      closeOnClickOutside: false,
      size: 600,
    },
    data: {
      children: <SourcesAdministrationModalLoader noteId={noteId} />,
    },
  })
}

function closeSourcesAdministrationModal() {
  closeModal(modalId)
}

export { openSourcesAdministrationModal, closeSourcesAdministrationModal }
