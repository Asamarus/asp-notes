import useAppSelector from '@/shared/lib/useAppSelector'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import store from '@/shared/lib/store'
import { reset, setFilters } from '@/entities/note'
import units from '@/shared/lib/units'
import { useComputedColorScheme } from '@mantine/core'

import { ActionIcon, Tooltip, ScrollArea } from '@mantine/core'
import { MdFilterAltOff, MdBook, MdLabel, MdCalendarMonth, MdClose } from 'react-icons/md'

import styles from './NotesFiltersStatusBar.module.css'

const cases = {
  nom: 'note',
  gen: 'notes',
  plu: 'notes',
}

function renderText({
  keywords,
  withoutBook,
  withoutTags,
  inRandomOrder,
}: {
  keywords: string[]
  withoutBook: boolean
  withoutTags: boolean
  inRandomOrder: boolean
}) {
  const extra = []

  if (keywords.length > 0) {
    extra.push(`with keywords: "${keywords.join(', ')}"`)
  }

  if (withoutBook) {
    extra.push('without book')
  }

  if (withoutTags) {
    extra.push('without tags')
  }

  if (inRandomOrder) {
    extra.push('in random order')
  }

  return extra.join(' ')
}

function NotesFiltersStatusBar() {
  const colorScheme = useComputedColorScheme('light')
  const numberOfNotes = useAppSelector((state) => state.notes.metadata.total)
  const book = useAppSelector((state) => state.notes.filters.book)
  const tags = useAppSelector((state) => state.notes.filters.tags)
  const withoutBook = useAppSelector((state) => state.notes.filters.withoutBook)
  const withoutTags = useAppSelector((state) => state.notes.filters.withoutTags)
  const fromDate = useAppSelector((state) => state.notes.filters.fromDate)
  const toDate = useAppSelector((state) => state.notes.filters.toDate)
  const searchTerm = useAppSelector((state) => state.notes.filters.searchTerm)
  const inRandomOrder = useAppSelector((state) => state.notes.filters.inRandomOrder)
  const keywords = useAppSelector((state) => state.notes.metadata.keywords)

  const hasFilters =
    tags.length > 0 ||
    book ||
    withoutBook ||
    withoutTags ||
    fromDate ||
    toDate ||
    searchTerm ||
    inRandomOrder

  const infoText = `${hasFilters ? 'Found' : 'Displaying'} ${numberOfNotes} ${units(
    numberOfNotes,
    cases,
  )} ${renderText({
    keywords,
    withoutBook,
    withoutTags,
    inRandomOrder,
  })}`

  return (
    <div className={styles['wrapper']}>
      <div className={styles['left-placeholder']} />

      <ScrollArea
        className={styles['scroll-area']}
        scrollbarSize={5}
      >
        <div className={styles['inner']}>
          {hasFilters && (
            <Tooltip label="Clear all filters">
              <ActionIcon
                onClick={() => {
                  store.dispatch(reset())
                  dispatchCustomEvent(events.notesSearch.setValue, '')
                  dispatchCustomEvent(events.notesList.search)
                }}
                color={colorScheme === 'dark' ? 'gray' : '#fff'}
                variant="transparent"
                size={32}
              >
                <MdFilterAltOff size={40} />
              </ActionIcon>
            </Tooltip>
          )}
          <div className={styles['info']}>
            <span>{infoText}</span>
          </div>
          {book && (
            <Tooltip label="Click to remove book from filter">
              <div
                className={`${styles['badge']} ${styles['badge_book']}`}
                color="red"
                onClick={() => {
                  store.dispatch(setFilters({ book: '' }))
                  dispatchCustomEvent(events.notesList.search)
                }}
              >
                <MdBook size={18} />
                <span>{book}</span>
                <MdClose size={18} />
              </div>
            </Tooltip>
          )}
          {tags.map((tag: string) => (
            <Tooltip
              key={tag}
              label="Click to remove tag from filter"
            >
              <div
                className={`${styles['badge']} ${styles['badge_tag']}`}
                onClick={() => {
                  store.dispatch(setFilters({ tags: tags.filter((t) => t !== tag) }))
                  dispatchCustomEvent(events.notesList.search)
                }}
              >
                <MdLabel size={18} />
                <span>{tag}</span>
                <MdClose size={18} />
              </div>
            </Tooltip>
          ))}
          {fromDate && (
            <Tooltip label="Click to remove date from filter">
              <div
                className={`${styles['badge']} ${styles['badge_date']}`}
                onClick={() => {
                  store.dispatch(setFilters({ fromDate: '', toDate: '' }))
                  dispatchCustomEvent(events.notesList.search)
                }}
              >
                <MdCalendarMonth />
                <span>{fromDate}</span>
                <MdClose />
              </div>
            </Tooltip>
          )}
        </div>
      </ScrollArea>
      <div className={styles['scroll-placeholder']} />
    </div>
  )
}

export default NotesFiltersStatusBar
