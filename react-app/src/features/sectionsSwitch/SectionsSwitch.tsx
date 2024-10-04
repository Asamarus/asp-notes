import { useState } from 'react'
import useAppSelector from '@/shared/lib/useAppSelector'
import { useComputedColorScheme } from '@mantine/core'

import { Link } from 'react-router-dom'
import { Collapse } from '@mantine/core'
import { MdExpandMore, MdExpandLess } from 'react-icons/md'

import styles from './SectionsSwitch.module.css'

function SectionsSwitch() {
  const colorScheme = useComputedColorScheme('light')
  const allNotesSection = useAppSelector((state) => state.sections.allNotesSection)
  const currentSection = useAppSelector((state) => state.sections.current)
  const sections = useAppSelector((state) => state.sections.list)
  const [opened, setOpened] = useState(false)

  const sectionsList = [allNotesSection, ...sections].filter((s) => s.name !== currentSection?.name)

  return (
    <>
      <div
        className={styles['header']}
        style={{
          backgroundColor:
            colorScheme === 'dark' ? 'rgba(255, 255, 255, 0.055)' : currentSection?.color,
        }}
        onClick={() => {
          setOpened((prev) => !prev)
        }}
      >
        <span className={styles['title']}>{currentSection?.displayName}</span>
        {!opened && (
          <MdExpandMore
            className={styles['caret']}
            size={30}
          />
        )}
        {opened && (
          <MdExpandLess
            className={styles['caret']}
            size={30}
          />
        )}
      </div>
      <Collapse in={opened}>
        {sectionsList.map(({ name, displayName, color }) => (
          <Link
            key={name}
            className={styles['section']}
            to={`/notes/${name !== allNotesSection.name ? name : ''}`}
            {...(colorScheme !== 'dark' ? { style: { backgroundColor: color } } : {})}
          >
            {displayName}
          </Link>
        ))}
      </Collapse>
    </>
  )
}

export default SectionsSwitch
