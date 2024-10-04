import { openModal, closeModal } from '@/shared/ui/modalsManager'

import ConfirmationModalLoader from './ConfirmationModalLoader'
import type { ConfirmationModalProps } from './ConfirmationModal'

const modalId = 'confirmationModal'

function openConfirmationModal(props: Omit<ConfirmationModalProps, 'onModalClose'>) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      closeOnClickOutside: false,
      closeOnEscape: false,
      withCloseButton: false,
      size: 400,
      padding: 0,
      styles: {
        content: {
          background: 'transparent',
        },
      },
    },
    data: {
      children: (
        <ConfirmationModalLoader
          {...props}
          onModalClose={closeConfirmationModal}
        />
      ),
    },
  })
}

function closeConfirmationModal() {
  closeModal(modalId)
}

export { openConfirmationModal, closeConfirmationModal }
