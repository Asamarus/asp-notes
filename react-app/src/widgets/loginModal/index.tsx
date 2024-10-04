import { openModal, closeModal } from '@/shared/ui/modalsManager'

import LoginModalLoader from './LoginModalLoader'

import type { ModalSettings } from '@/shared/ui/modalsManager'

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
