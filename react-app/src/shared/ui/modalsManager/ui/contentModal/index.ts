import type { ContentModalProps } from './ContentModal'
export type { ContentModalProps }
import type { ModalData } from '../../model/types'
import ContentModal from './ContentModal'

export const modalData: ModalData = {
  name: 'content',
  inUrl: false,
  component: ContentModal,
  modalProps: {
    withCloseButton: true,
  },
}

export default modalData
