import { openModal, closeModal } from '@/components/modalsManager'

import ChangePasswordModalLoader from './ChangePasswordModalLoader'

const modalId = 'changePasswordModal'

function openChangePasswordModal() {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: 'Change password',
      closeOnClickOutside: false,
      size: 400,
    },
    data: {
      children: <ChangePasswordModalLoader />,
    },
  })
}

function closeChangePasswordModal() {
  closeModal(modalId)
}

export { openChangePasswordModal, closeChangePasswordModal }
