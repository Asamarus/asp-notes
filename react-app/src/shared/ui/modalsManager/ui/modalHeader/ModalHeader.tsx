import type { MantineColor } from '@mantine/core'
import { ActionIcon, useMantineTheme, useProps, CloseIcon } from '@mantine/core'

import styles from './ModalHeader.module.css'

export interface ModalHeaderProps {
  /** Color from theme.colors */
  color?: MantineColor

  /** Show close button */
  showClose?: boolean

  /** Called when close button clicked */
  onClose?(): void

  /** The content of the component */
  children?: React.ReactNode
}

const defaultProps: Partial<ModalHeaderProps> = {
  color: 'blue',
  showClose: true,
}

function ModalHeader(_props: ModalHeaderProps) {
  const theme = useMantineTheme()
  const props = useProps('ModalHeader', defaultProps, _props)
  const { color, showClose, onClose, children } = props

  const backgroundColor = theme.variantColorResolver({ color, theme, variant: 'filled' }).background

  return (
    <div
      className={styles['wrapper']}
      style={{ backgroundColor: backgroundColor }}
    >
      <div className={styles['inner']}>{children}</div>
      {showClose && (
        <div
          className={styles['close-wrapper']}
          onClick={onClose}
        >
          <ActionIcon
            color="#fff"
            variant="transparent"
            aria-label="Close"
          >
            <CloseIcon size={'20px'} />
          </ActionIcon>
        </div>
      )}
    </div>
  )
}

export default ModalHeader
