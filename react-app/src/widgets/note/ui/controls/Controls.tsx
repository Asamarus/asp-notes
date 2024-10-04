import { openSourcesListModal } from '@/widgets/sources'
import useAppSelector from '@/shared/lib/useAppSelector'
import store from '@/shared/lib/store'
import { useHotkeys } from '@mantine/hooks'
import { openBooksModal } from '@/widgets/booksModal'
import rison from 'rison'
import { setFilters } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { useNavigate } from 'react-router-dom'

import { Book, ChangeBookIcon, Sources } from '@/entities/note'
import { ActionIcon, Tooltip } from '@mantine/core'
import { MdArrowBack, MdArrowForward } from 'react-icons/md'
import NoteContentSearch from '@/features/noteContentSearch'
import TableOfContents from '@/features/noteTableOfContents'

import type { NoteProps } from '../../Note'

import commonStyles from '@/shared/ui/commonStyles.module.css'
import styles from './Controls.module.css'

type ControlsProps = Pick<NoteProps, 'id' | 'tab' | 'displayType' | 'onClose'> & {
  setCurrentNoteId: (id: number) => void
}

function Controls({ id, tab, displayType, setCurrentNoteId, onClose }: ControlsProps) {
  const navigate = useNavigate()
  const showArrows = useAppSelector((state) => state.notes.ids.length > 1)

  const handlePreviousNoteClick = () => {
    const noteIds = store.getState().notes.ids
    const currentIndex = noteIds.indexOf(id)
    const previousIndex = (currentIndex - 1 + noteIds.length) % noteIds.length
    setCurrentNoteId(noteIds[previousIndex])
  }

  const handleNextNoteClick = () => {
    const noteIds = store.getState().notes.ids
    const currentIndex = noteIds.indexOf(id)
    const nextIndex = (currentIndex + 1) % noteIds.length
    setCurrentNoteId(noteIds[nextIndex])
  }

  useHotkeys([
    ['alt+,', handlePreviousNoteClick],
    ['alt+.', handleNextNoteClick],
  ])

  const navControls = (
    <>
      {displayType === 'modal' && showArrows && (
        <>
          <Tooltip label={'Previous note. Alt + ,'}>
            <ActionIcon
              size={26}
              onClick={handlePreviousNoteClick}
            >
              <MdArrowBack
                className={commonStyles['action-icon']}
                size={26}
              />
            </ActionIcon>
          </Tooltip>
          <Tooltip label={'Next note Alt + .'}>
            <ActionIcon
              size={26}
              onClick={handleNextNoteClick}
            >
              <MdArrowForward
                className={commonStyles['action-icon']}
                size={26}
              />
            </ActionIcon>
          </Tooltip>
        </>
      )}

      <TableOfContents
        key={`toc-${id}`}
        id={id}
      />
      <NoteContentSearch
        key={`search-${id}`}
        id={id}
      />
    </>
  )

  return (
    <div className={styles['wrapper']}>
      {tab === 'view' && navControls}
      <Sources
        id={id}
        onClick={() => {
          openSourcesListModal(id)
        }}
      />
      <div className={styles['divider']} />
      <ChangeBookIcon
        onClick={() => {
          openBooksModal(id)
        }}
      />
      <Book
        id={id}
        onClick={(book) => {
          if (displayType === 'page') {
            const searchParams = new URLSearchParams()
            searchParams.append(
              'list',
              rison.encode({
                book,
              }),
            )
            const section = store.getState().notes.collection[id]?.section ?? ''
            navigate(`/notes/${section}?${searchParams.toString()}`)
          } else if (displayType === 'modal') {
            store.dispatch(setFilters({ book }))
            dispatchCustomEvent(events.notesList.search)
            onClose()
          }
        }}
        maxWidth={300}
      />
    </div>
  )
}

export default Controls
