import { openModal, closeModal } from '@/shared/ui/modalsManager'

import BooksModalLoader from './BooksModalLoader'

const modalId = 'booksModal'

function openBooksModal(noteId?: number) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: typeof noteId === 'number' ? `Change note #${noteId} book` : 'Filter notes by book',
      closeOnClickOutside: false,
      size: 500,
    },
    data: {
      children: <BooksModalLoader noteId={noteId} />,
    },
  })
}

function closeBooksModal() {
  closeModal(modalId)
}

export { openBooksModal, closeBooksModal }
