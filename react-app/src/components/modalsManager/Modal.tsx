import { useEffect, forwardRef } from 'react'
import { Modal, ModalProps } from '@mantine/core'

export interface ModalWrapperProps extends ModalProps {
  onMounted?: () => void
}

const ModalWrapper = forwardRef<HTMLDivElement, ModalWrapperProps>(
  ({ onMounted, ...props }, ref) => {
    useEffect(() => {
      if (onMounted) {
        onMounted()
      }
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
      <Modal
        ref={ref}
        withinPortal
        {...props}
      />
    )
  },
)

export default ModalWrapper
