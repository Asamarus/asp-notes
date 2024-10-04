import useAppSelector from '@/shared/lib/useAppSelector'

import { Info } from '@/entities/note'
import { CloseButton } from '@mantine/core'
import NoteContextMenu from '@/features/noteContextMenu'

import type { NoteProps } from '../../Note'

import styles from './Top.module.css'

interface TopProps extends Omit<NoteProps, 'tab'> {
  setCurrentTab: (tab: 'view' | 'edit' | 'delete') => void
}

function Top({ id, onClose, setCurrentTab, displayType }: TopProps) {
  const isNotSaved = useAppSelector((state) => state.notes.noteIsNotSaved)

  return (
    <div className={styles['top']}>
      <Info
        id={id}
        flexItem
      />
      <NoteContextMenu
        id={id}
        isNotSaved={isNotSaved}
        changeNoteTab={(tab) => {
          setCurrentTab(tab)
        }}
      />
      {displayType === 'modal' && (
        <CloseButton
          ml={5}
          onClick={onClose}
        />
      )}
    </div>
  )
}

export default Top
