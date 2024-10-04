import { useState, lazy } from 'react'

import Top from './ui/top'
import Controls from './ui/controls'
import Bottom from './ui/bottom'
import ComponentLoader from '@/shared/ui/componentLoader'

import styles from './Note.module.css'

const View = lazy(() => import('./ui/view'))
const Edit = lazy(() => import('./ui/edit'))
const Delete = lazy(() => import('./ui/delete'))

export interface NoteProps {
  /** Note's id */
  id: number
  tab?: 'view' | 'edit' | 'delete'
  displayType: 'modal' | 'page'
  onClose(): void
}

function Note({ id, tab = 'view', displayType, onClose }: NoteProps) {
  const [currentNoteId, setCurrentNoteId] = useState(id)
  const [currentTab, setCurrentTab] = useState(tab)

  let content = null

  switch (currentTab) {
    case 'view':
      content = (
        <ComponentLoader full>
          <View id={currentNoteId} />
        </ComponentLoader>
      )
      break
    case 'edit':
      content = (
        <ComponentLoader full>
          <Edit id={currentNoteId} />
        </ComponentLoader>
      )

      break
    case 'delete':
      content = (
        <ComponentLoader full>
          <Delete
            id={currentNoteId}
            onClose={onClose}
          />
        </ComponentLoader>
      )
      break
  }

  return (
    <div className={styles['wrapper']}>
      <div className={styles['top']}>
        <Top
          id={currentNoteId}
          onClose={onClose}
          setCurrentTab={setCurrentTab}
          displayType={displayType}
        />
        <Controls
          id={currentNoteId}
          tab={currentTab}
          displayType={displayType}
          setCurrentNoteId={setCurrentNoteId}
          onClose={onClose}
        />
      </div>
      <div
        className={styles['content']}
        data-name="note-content"
        data-display-type={displayType}
      >
        <div className={styles['content-inner']}>{content}</div>
      </div>
      <div className={styles['bottom']}>
        <Bottom
          id={currentNoteId}
          displayType={displayType}
          onClose={onClose}
        />
      </div>
    </div>
  )
}

export default Note
