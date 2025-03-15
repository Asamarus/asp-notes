import { useEffect, useRef, useState } from 'react'
import { useCombobox, useComputedColorScheme } from '@mantine/core'
import useFetch from '@/shared/lib/useFetch'
import useCustomEventListener, { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { events } from '@/shared/config'
import { notesApi, setFilters } from '@/entities/note'
import store from '@/shared/lib/store'
import { openNoteModal } from '@/widgets/noteModal'
import getCurrentSection from '@/shared/model/getCurrentSection'
import { useDocumentVisibility } from '@mantine/hooks'

import { CloseButton, Combobox, TextInput, Loader, ScrollArea } from '@mantine/core'
import { MdSearch } from 'react-icons/md'

import type { AutocompleteNote } from '@/entities/note'

type Options = {
  notes: AutocompleteNote[]
  books: string[]
  tags: string[]
}

function handleOptionSelect(value: string) {
  const [type, stringValue] = value.split('_')

  switch (type) {
    case 'note':
      openNoteModal({ id: parseInt(stringValue, 10) })
      break
    case 'book':
      store.dispatch(setFilters({ book: stringValue }))
      dispatchCustomEvent(events.notesList.search)
      break
    case 'tag':
      store.dispatch(setFilters({ tags: [stringValue] }))
      dispatchCustomEvent(events.notesList.search)
      break
    default:
      break
  }
}

function NotesSearch() {
  const colorScheme = useComputedColorScheme('light')
  const { request, isLoading } = useFetch(notesApi.autocompleteNotes, { debounce: true })
  const combobox = useCombobox()
  const [value, setValue] = useState('')
  const [options, setOptions] = useState<Options>({ notes: [], books: [], tags: [] })
  const ignoreAutoCompleteRef = useRef(false)
  const inputRef = useRef<HTMLInputElement>(null)
  const documentState = useDocumentVisibility()

  const autoComplete = (searchTerm: string) => {
    if (ignoreAutoCompleteRef.current || searchTerm.length < 2) return

    const book = store.getState().notes.filters.book
    request(
      {
        SearchTerm: searchTerm,
        Section: getCurrentSection(),
        Book: book,
      },
      ({ data }) => {
        if (data && !ignoreAutoCompleteRef.current) {
          const notes: AutocompleteNote[] = []

          data.notes?.forEach((note) => {
            notes.push({
              id: note.id ?? 0,
              title: note.title ?? '',
            })
          })

          setOptions({
            notes: notes,
            books: data.books ?? [],
            tags: data.tags ?? [],
          })

          const hasOptions =
            (data.notes?.length ?? 0) > 0 ||
            (data.books?.length ?? 0) > 0 ||
            (data.tags?.length ?? 0) > 0

          if (hasOptions && !combobox.dropdownOpened) {
            combobox.openDropdown()
          } else if (!hasOptions && combobox.dropdownOpened) {
            combobox.closeDropdown()
          }
        }
      },
    )
  }

  useEffect(() => {
    const initialSearchTerm = store.getState().notes.filters.searchTerm

    if (initialSearchTerm) {
      setValue(initialSearchTerm)
    }
  }, [])

  useCustomEventListener(events.notesSearch.setValue, (payload) => {
    if (typeof payload === 'string') {
      setValue(payload)
    }
  })

  useEffect(() => {
    if (
      documentState === 'hidden' &&
      inputRef.current &&
      inputRef.current === document.activeElement
    ) {
      inputRef.current?.blur()
    }
  }, [documentState])

  const hasOptions = options.notes.length > 0 || options.books.length > 0 || options.tags.length > 0

  return (
    <Combobox
      onOptionSubmit={(value) => {
        handleOptionSelect(value)
        combobox.closeDropdown()
      }}
      store={combobox}
      withinPortal={true}
    >
      <Combobox.Target>
        <TextInput
          ref={inputRef}
          placeholder="Search"
          value={value}
          onChange={(event) => {
            setValue(event.currentTarget.value)
            ignoreAutoCompleteRef.current = false
            autoComplete(event.currentTarget.value)
          }}
          onClick={() => hasOptions && combobox.openDropdown()}
          onFocus={() => hasOptions && combobox.openDropdown()}
          onBlur={() => combobox.closeDropdown()}
          onKeyUp={(e) => {
            if (e.key === 'Enter') {
              combobox.closeDropdown()
              store.dispatch(setFilters({ searchTerm: value }))
              dispatchCustomEvent(events.notesList.search)
              ignoreAutoCompleteRef.current = true
            }
          }}
          leftSection={
            <MdSearch
              size={25}
              color={colorScheme === 'dark' ? '#e9ecef' : '#000'}
            />
          }
          rightSection={
            isLoading ? (
              <Loader
                size={20}
                color={colorScheme === 'dark' ? '#fff' : 'gray'}
              />
            ) : value.length > 0 ? (
              <CloseButton
                size="sm"
                onMouseDown={(event) => event.preventDefault()}
                onClick={() => {
                  setValue('')
                  store.dispatch(setFilters({ searchTerm: '' }))
                  dispatchCustomEvent(events.notesList.search)
                }}
                aria-label="Clear value"
              />
            ) : undefined
          }
        />
      </Combobox.Target>

      <Combobox.Dropdown>
        <Combobox.Options>
          <ScrollArea.Autosize
            type="scroll"
            mah={200}
          >
            {options.notes.length > 0 && (
              <Combobox.Group label="Notes">
                {options.notes.map((note) => (
                  <Combobox.Option
                    value={`note_${note.id}`}
                    key={note.id}
                  >
                    {note.title}
                  </Combobox.Option>
                ))}
              </Combobox.Group>
            )}
            {options.books.length > 0 && (
              <Combobox.Group label="Books">
                {options.books.map((book) => (
                  <Combobox.Option
                    value={`book_${book}`}
                    key={book}
                  >
                    {book}
                  </Combobox.Option>
                ))}
              </Combobox.Group>
            )}
            {options.tags.length > 0 && (
              <Combobox.Group label="Tags">
                {options.tags.map((tag) => (
                  <Combobox.Option
                    value={`tag_${tag}`}
                    key={tag}
                  >
                    {tag}
                  </Combobox.Option>
                ))}
              </Combobox.Group>
            )}
          </ScrollArea.Autosize>
        </Combobox.Options>
      </Combobox.Dropdown>
    </Combobox>
  )
}

export default NotesSearch
