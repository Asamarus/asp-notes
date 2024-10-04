import { openModal, closeModal } from '@/shared/ui/modalsManager'

import LoadingModalLoader from './LoadingModalLoader'
import type { LoadingModalProps } from './LoadingModal'

const modalId = 'loadingModal'

function openLoadingModal(props: LoadingModalProps) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      closeOnClickOutside: false,
      closeOnEscape: false,
      withCloseButton: false,
      size: 400,
    },
    data: {
      children: <LoadingModalLoader {...props} />,
    },
  })
}

function closeLoadingModal() {
  closeModal(modalId)
}

export { openLoadingModal, closeLoadingModal }
