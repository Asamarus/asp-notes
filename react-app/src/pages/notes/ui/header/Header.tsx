import useViewportSize from '@/shared/lib/useViewportSize'
import useAppSelector from '@/shared/lib/useAppSelector'
import { useComputedColorScheme } from '@mantine/core'

import DrawerControl from './ui/drawerControl'
import Title from './ui/title'
import NotesSearch from '@/widgets/notesSearch'
import RefreshNotesList from '@/features/refreshNotesList'
import OpenInNewWindow from '@/features/openInNewWindow'
import ToggleColorScheme from '@/features/toggleColorScheme'
import UserInfo from '@/widgets/userInfo'

import styles from './Header.module.css'

function Header() {
  const colorScheme = useComputedColorScheme('light')
  const backgroundColor = useAppSelector((state) => state.sections.current?.color)
  const { width } = useViewportSize()

  if (width === 0) {
    return null
  }

  const isDesktop = width >= 1024
  const headerBackgroundColor = colorScheme === 'dark' ? '#202020' : backgroundColor

  return (
    <div
      className={styles['wrapper']}
      style={{ backgroundColor: headerBackgroundColor }}
    >
      {!isDesktop && <DrawerControl />}
      {!isDesktop && <Title />}

      <div className={styles['search-wrapper']}>
        <NotesSearch />
      </div>
      <div className={styles['divider']}></div>
      <div className={styles['right-controls']}>
        <RefreshNotesList />
        <OpenInNewWindow />
        <ToggleColorScheme displayType="notesPage" />
        <UserInfo displayType="notesPage" />
      </div>
    </div>
  )
}

export default Header
