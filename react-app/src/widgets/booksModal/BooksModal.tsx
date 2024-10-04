import { useEffect, useState } from 'react'
import useFetch from '@/shared/lib/useFetch'
import { booksApi } from '@/entities/book'
import { getNoteFromResponse, notesApi, setNote } from '@/entities/note'
import store from '@/shared/lib/store'
import getCurrentSection from '@/shared/model/getCurrentSection'
import { useCombobox } from '@mantine/core'
import { setFilters } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { closeBooksModal } from '.'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import Loading from '@/shared/ui/loading'
import {
  Button,
  Group,
  Combobox,
  TextInput,
  CheckIcon,
  CloseButton,
  ScrollArea,
} from '@mantine/core'

import type { Book } from '@/entities/book'

import styles from './BooksModal.module.css'

export interface BooksModalProps {
  noteId?: number
}

function BooksModal({ noteId = -1 }: BooksModalProps) {
  const isChangeBookModal = noteId > 0
  const { request: getBooksRequest, isLoading: isGetBooksLoading } = useFetch(
    booksApi.getBooksList,
    {
      initialIsLoading: true,
    },
  )
  const { request: updateNoteBookRequest, isLoading: isUpdateNoteBookLoading } = useFetch(
    notesApi.updateNoteBook,
  )
  const combobox = useCombobox()
  const [books, setBooks] = useState<Book[]>([])
  const [selectedBook, setSelectedBook] = useState('')
  const [search, setSearch] = useState('')
  const color = useCurrentColor()

  useEffect(() => {
    if (noteId !== -1) {
      const noteBook = store.getState().notes.collection[noteId]?.book ?? ''

      if (noteBook) {
        setSelectedBook(noteBook)
      }
    } else {
      const book = store.getState().notes.filters.book
      if (book) {
        setSelectedBook(book)
      }
    }

    getBooksRequest({ section: getCurrentSection(noteId) }, ({ data }) => {
      if (data) {
        setBooks(data.map((book) => ({ name: book?.name ?? '', count: book.count })))
      }
    })
  }, [getBooksRequest, noteId])

  if (isGetBooksLoading) {
    return <Loading full />
  }

  const handleCreateBook = () => {
    setBooks((currentBooks) => [{ name: search, count: 0 }, ...currentBooks])
    setSelectedBook(search)
    setSearch('')
  }

  const options = books
    .filter((item) => item.name.toLowerCase().includes(search.toLowerCase().trim()))
    .map((item) => (
      <Combobox.Option
        value={item.name}
        key={item.name}
        active={selectedBook === item.name}
      >
        <Group gap="xs">
          {selectedBook === item.name && <CheckIcon size={12} />}
          <span>{`${item.name} (${item.count})`}</span>
        </Group>
      </Combobox.Option>
    ))

  return (
    <>
      <Combobox
        store={combobox}
        onOptionSubmit={(value) => {
          if (value === '$create') {
            handleCreateBook()
          } else {
            setSelectedBook(value === selectedBook ? '' : value)
          }
        }}
      >
        <Combobox.EventsTarget>
          <TextInput
            placeholder="Search books"
            classNames={{ input: styles['input'] }}
            value={search}
            onChange={(event) => {
              setSearch(event.currentTarget.value)
              combobox.updateSelectedOptionIndex()
            }}
            onKeyUp={(e) => {
              if (
                e.key === 'Enter' &&
                (options.length === 0 || !books.some((book) => book.name === search.trim())) &&
                search.trim().length >= 2 &&
                isChangeBookModal
              ) {
                handleCreateBook()
              }
            }}
            rightSection={
              search.length > 0 ? (
                <CloseButton
                  size="sm"
                  onMouseDown={(event) => event.preventDefault()}
                  onClick={() => setSearch('')}
                  aria-label="Clear value"
                />
              ) : undefined
            }
          />
        </Combobox.EventsTarget>

        <div className={styles['list']}>
          <Combobox.Options>
            <ScrollArea
              h={200}
              type="always"
            >
              {options}
              {options.length === 0 && !isChangeBookModal && (
                <Combobox.Empty>Nothing found....</Combobox.Empty>
              )}
              {options.length === 0 && search.trim().length >= 2 && isChangeBookModal && (
                <Combobox.Option value="$create">+ Create new "{search}" book </Combobox.Option>
              )}
            </ScrollArea>
          </Combobox.Options>
        </div>
      </Combobox>
      <Group
        justify="flex-end"
        mt="md"
      >
        <Button
          color={color}
          loading={isUpdateNoteBookLoading}
          onClick={() => {
            if (isChangeBookModal) {
              updateNoteBookRequest({ id: noteId, book: selectedBook }, ({ data }) => {
                if (data) {
                  const note = getNoteFromResponse(data.note)
                  store.dispatch(setNote({ id: note.id, note }))
                  dispatchCrossTabEvent(events.note.updated, note)
                }
              })
            } else {
              store.dispatch(setFilters({ book: selectedBook }))
              dispatchCustomEvent(events.notesList.search)
            }
            closeBooksModal()
          }}
        >
          {isChangeBookModal ? "Update note's book" : 'Apply book filter'}
        </Button>
      </Group>
    </>
  )
}

export default BooksModal
