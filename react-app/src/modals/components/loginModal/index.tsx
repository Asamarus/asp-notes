import { openModal, closeModal } from '@/components/modalsManager'

import LoginModalLoader from './LoginModalLoader'
import { ModalSettings } from '@/components/modalsManager/types'

const modalId = 'loginModal'

export const loginModalSettings: ModalSettings = {
  closeOnClickOutside: false,
  closeOnEscape: false,
  withCloseButton: false,
  size: 300,
  overlayProps: {
    blur: 5,
  },
}

function openLoginModal() {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: loginModalSettings,
    data: {
      children: <LoginModalLoader />,
    },
  })
}

function closeLoginModal() {
  closeModal(modalId)
}

export { openLoginModal, closeLoginModal }
