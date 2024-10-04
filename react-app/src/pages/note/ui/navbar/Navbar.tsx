import useAppSelector from '@/shared/lib/useAppSelector'

import { ActionIcon, useComputedColorScheme } from '@mantine/core'
import { Link } from 'react-router-dom'
import { MdArrowBack } from 'react-icons/md'

import styles from './Navbar.module.css'

export interface NavbarProps {
  /** Note's id */
  id: number
}
function Navbar({ id }: NavbarProps) {
  const colorScheme = useComputedColorScheme('light')
  const noteSection = useAppSelector((state) => state.notes.collection[id].section)
  const section = useAppSelector((state) =>
    state.sections.list.find((section) => section.name === noteSection),
  )

  return (
    <div
      className={styles['wrapper']}
      style={{ backgroundColor: colorScheme === 'dark' ? '#202020' : section?.color }}
    >
      <Link
        to={`/notes/${section?.name}`}
        className={styles['link']}
      >
        <ActionIcon
          size={30}
          variant="transparent"
          color={colorScheme === 'dark' ? 'gray' : '#fff'}
        >
          <MdArrowBack size={30} />
        </ActionIcon>
      </Link>
      <div className={styles['title-wrapper']}>
        <div className={styles['title']}>{section?.displayName}</div>
      </div>
    </div>
  )
}

export default Navbar
