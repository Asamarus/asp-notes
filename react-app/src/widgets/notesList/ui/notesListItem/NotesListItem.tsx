import { openNoteModal } from '@/widgets/noteModal'
import { openSourcesListModal } from '@/widgets/sources'
import { openBooksModal } from '@/widgets/booksModal'
import { openTagsModal } from '@/widgets/tagsModal'
import { setFilters } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import store from '@/shared/lib/store'

import { Info, Sources, ChangeBookIcon, Book, EditTagsIcon, Tags } from '@/entities/note'
import NoteContextMenu from '@/features/noteContextMenu'
import Preview from '../preview'
import { ScrollArea } from '@mantine/core'

import styles from './NotesListItem.module.css'

export interface NotesListItemProps {
  /** Note's id */
  id: number
}
function NotesListItem({ id }: NotesListItemProps) {
  return (
    <div className={styles['wrapper']}>
      <div className={styles['top']}>
        <Info
          id={id}
          flexItem
        />
        <NoteContextMenu
          id={id}
          changeNoteTab={(tab) => {
            openNoteModal({ id, tab })
          }}
        />
      </div>
      <div className={styles['sources-and-book']}>
        <Sources
          id={id}
          onClick={() => {
            openSourcesListModal(id)
          }}
        />
        <div className={styles['book-wrapper']}>
          <ChangeBookIcon
            onClick={() => {
              openBooksModal(id)
            }}
          />
          <Book
            id={id}
            onClick={(book) => {
              store.dispatch(setFilters({ book }))
              dispatchCustomEvent(events.notesList.search)
            }}
            maxWidth={120}
          />
        </div>
      </div>
      <div
        className={styles['preview']}
        onClick={() => {
          openNoteModal({ id })
        }}
      >
        <Preview id={id} />
      </div>
      <div className={styles['bottom']}>
        <EditTagsIcon
          onClick={() => {
            openTagsModal(id)
          }}
        />
        <div className={styles['spacer']} />
        <ScrollArea scrollbarSize={5}>
          <Tags
            id={id}
            onClick={(tag) => {
              store.dispatch(setFilters({ tags: [tag] }))
              dispatchCustomEvent(events.notesList.search)
            }}
          />
        </ScrollArea>
      </div>
    </div>
  )
}

export default NotesListItem
