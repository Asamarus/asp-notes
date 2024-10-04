import ModalsManager from './ui/modalsManager'
import ContentModal from './ui/contentModal'

const defaultModals = {
  [ContentModal.name]: ContentModal,
}

export { defaultModals }

export { openModal, closeModal } from './model/events'
export { openConfirmationModal, closeConfirmationModal } from './ui/confirmationModal'
export { openLoadingModal, closeLoadingModal } from './ui/loadingModal'

export type { ModalsManagerProps } from './ui/modalsManager'
export type { ModalSettings, ModalData } from './model/types'

export default ModalsManager
