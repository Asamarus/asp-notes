import { openModal, closeModal, openConfirmationModal } from '@/shared/ui/modalsManager'
import store from '@/shared/lib/store'
import { setNoteIsNotSaved } from '@/entities/note/'

import NoteModal from './NoteModal'

import type { ModalData } from '@/shared/ui/modalsManager'

export const modalData: ModalData = {
  name: 'note',
  inUrl: true,
  component: NoteModal,
  modalProps: {
    closeOnClickOutside: false,
    size: 1024,
    withCloseButton: false,
    trapFocus: false,
    overlayProps: {
      blur: 5,
    },
    padding: 0,
    onCloseConfirm: (id) => {
      const notSaved = store.getState().notes.noteIsNotSaved

      if (notSaved) {
        openConfirmationModal({
          title: 'Your note data is not saved!',
          message: 'Are you sure you do not want to save changes?',
          confirmLabel: 'Close without saving',
          onConfirm: () => {
            store.dispatch(setNoteIsNotSaved(false))
            closeModal(id)
          },
        })
      } else {
        closeModal(id)
      }
    },
  },
}

type NoteModalData = {
  id: number
  tab?: 'view' | 'edit' | 'delete'
}

function openNoteModal(data: NoteModalData) {
  openModal({
    name: 'note',
    data: data,
  })
}

function closeNoteModal() {
  closeModal('note')
}

export { openNoteModal, closeNoteModal }
