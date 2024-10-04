import { openModal, closeModal } from '@/shared/ui/modalsManager'

import TagsModalLoader from './TagsModalLoader'

const modalId = 'tagsModal'

function openTagsModal(noteId?: number) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: typeof noteId === 'number' ? `Edit note #${noteId} tags` : 'Filter notes by tags',
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: <TagsModalLoader noteId={noteId} />,
    },
  })
}

function closeTagsModal() {
  closeModal(modalId)
}

export { openTagsModal, closeTagsModal }
