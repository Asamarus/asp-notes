import { Group, Text, useProps, Button, type MantineColor } from '@mantine/core'
import ModalHeader from '@/modals/misc/modalHeader'
import styles from './ConfirmationModal.module.css'

export interface ConfirmationModalProps {
  onModalClose(): void
  onClose?: () => void
  color?: MantineColor
  onCancel?(): void
  onConfirm(): void
  title?: string
  message: string
  cancelLabel?: string
  confirmLabel?: string
  showClose?: boolean
}

const defaultProps: Partial<ConfirmationModalProps> = {
  color: 'red',
  title: 'Confirm action',
  message: 'Are you sure?',
  cancelLabel: 'Cancel',
  confirmLabel: 'Confirm',
  showClose: true,
}

function ConfirmationModal(_props: ConfirmationModalProps) {
  const props = useProps('ConfirmationModal', defaultProps, _props)
  const {
    color,
    onCancel,
    onConfirm,
    title,
    message,
    cancelLabel,
    confirmLabel,
    onClose,
    showClose,
    onModalClose,
  } = props

  return (
    <>
      <ModalHeader
        color={color}
        onClose={() => {
          onClose?.()
          onModalClose()
        }}
        showClose={showClose}
      >
        <Text>{title}</Text>
      </ModalHeader>
      <div className={styles['content']}>
        <Text mb="md">{message}</Text>

        <Group justify="flex-end">
          <Button
            variant="default"
            onClick={() => {
              onCancel?.()
              onModalClose()
            }}
          >
            {cancelLabel}
          </Button>

          <Button
            color={color}
            onClick={() => {
              onConfirm()
              onModalClose()
            }}
          >
            {confirmLabel}
          </Button>
        </Group>
      </div>
    </>
  )
}

export default ConfirmationModal
