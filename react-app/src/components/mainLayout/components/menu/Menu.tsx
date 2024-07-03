import { NavLink } from '@mantine/core'
import { NavLink as RouterNavLink } from 'react-router-dom'

import { MdHome, MdDashboard, MdPages } from 'react-icons/md'

function Menu({ close }: { close: () => void }) {
  return (
    <>
      <RouterNavLink to="/">
        {({ isActive }) => (
          <NavLink
            component="div"
            label="Home"
            leftSection={<MdHome size={30} />}
            active={isActive}
            onClick={close}
          />
        )}
      </RouterNavLink>
      <RouterNavLink to="/dashboard">
        {({ isActive }) => (
          <NavLink
            component="div"
            label="Dashboard"
            leftSection={<MdDashboard size={30} />}
            active={isActive}
            onClick={close}
          />
        )}
      </RouterNavLink>
      <RouterNavLink to="/page">
        {({ isActive }) => (
          <NavLink
            component="div"
            label="Page"
            leftSection={<MdPages size={30} />}
            active={isActive}
            onClick={close}
          />
        )}
      </RouterNavLink>
    </>
  )
}

export default Menu
