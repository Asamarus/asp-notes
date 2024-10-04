import useViewportSize from '@/shared/lib/useViewportSize'

import { ScrollArea } from '@mantine/core'
import Drawer from '../drawer'
import SectionsSwitch from '@/features/sectionsSwitch'
import AddNote from '@/features/addNote'
import NotesFilters from '@/widgets/notesFilters'
import { NavLink } from '@mantine/core'
import { Link } from 'react-router-dom'
import { MdSettings } from 'react-icons/md'

import styles from './Sidebar.module.css'

function Sidebar() {
  const { height, width } = useViewportSize()
  const isDesktop = width >= 1024

  const content = (
    <ScrollArea style={{ height: `${height}px` }}>
      <SectionsSwitch />
      <div className={styles['inner']}>
        <AddNote />
        <NotesFilters />
      </div>
      <Link to="/settings">
        <NavLink
          component="div"
          label="Settings"
          leftSection={<MdSettings />}
        />
      </Link>
    </ScrollArea>
  )

  return isDesktop ? (
    <div className={styles['wrapper']}>
      <div
        className={styles['decorative']}
        style={{ height: `${height}px` }}
      />
      <div className={styles['content']}>{content}</div>
    </div>
  ) : (
    <Drawer>{content}</Drawer>
  )
}

export default Sidebar
