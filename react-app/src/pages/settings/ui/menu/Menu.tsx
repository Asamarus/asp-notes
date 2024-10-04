import { NavLink } from '@mantine/core'
import { NavLink as RouterNavLink } from 'react-router-dom'
import { MdHome, MdList } from 'react-icons/md'

import styles from './Menu.module.css'

function Menu({ close }: { close: () => void }) {
  return (
    <div className={styles['wrapper']}>
      <RouterNavLink to="/">
        {({ isActive }) => (
          <NavLink
            className={styles['link']}
            data-active={isActive || undefined}
            component="div"
            label="Home"
            leftSection={<MdHome className={styles['link-icon']} />}
            active={isActive}
            onClick={close}
          />
        )}
      </RouterNavLink>
      <RouterNavLink to="/settings/sections">
        {({ isActive }) => (
          <NavLink
            className={styles['link']}
            data-active={isActive || undefined}
            component="div"
            label="Sections"
            leftSection={
              <MdList
                className={styles['link-icon']}
                size={30}
              />
            }
            active={isActive}
            onClick={close}
          />
        )}
      </RouterNavLink>
    </div>
  )
}

export default Menu
