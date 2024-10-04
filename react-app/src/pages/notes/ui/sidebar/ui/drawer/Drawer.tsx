import { useState } from 'react'
import useCustomEventListener from '@/shared/lib/useCustomEventListener'
import { events } from '@/shared/config'
import clsx from 'clsx'

import styles from './Drawer.module.css'

export interface DrawerProps {
  /** The content of the component */
  children?: React.ReactNode
}
function Drawer({ children }: DrawerProps) {
  const [opened, setOpened] = useState(false)

  useCustomEventListener(events.drawer.open, () => {
    setOpened(true)
  })

  useCustomEventListener(events.drawer.close, () => {
    setOpened(false)
  })

  return (
    <div>
      <div
        className={clsx(styles['wrapper'], {
          [styles['wrapper-opened']]: opened,
        })}
      >
        {children}
      </div>
      {opened && (
        <div
          className={styles['overlay']}
          onClick={() => {
            setOpened(false)
          }}
        />
      )}
    </div>
  )
}

export default Drawer
