import { useEffect, useState } from 'react'
import useFetch from '@/shared/lib/useFetch'
import { tagsApi } from '@/entities/tag'
import { notesApi, setNote } from '@/entities/note'
import store from '@/shared/lib/store'
import getCurrentSection from '@/shared/model/getCurrentSection'
import { useCombobox } from '@mantine/core'
import { setFilters } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { closeTagsModal } from '.'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import noop from '@/shared/lib/noop'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import { showSuccess } from '@/shared/lib/notifications'

import Loading from '@/shared/ui/loading'
import { Button, Group, Checkbox, ScrollArea, Combobox, Pill, PillsInput } from '@mantine/core'
import { MdClose } from 'react-icons/md'

import type { Tag } from '@/entities/tag'

import styles from './TagsModal.module.css'

export interface TagsModalProps {
  noteId?: number
}

function TagsModal({ noteId = -1 }: TagsModalProps) {
  const isEditTagsModal = noteId > 0
  const { request: getTagsRequest, isLoading: isGetTagsLoading } = useFetch(tagsApi.getTagsList, {
    initialIsLoading: true,
  })
  const { request: updateTagsRequest, isLoading: isUpdateTagsLoading } = useFetch(
    notesApi.updateNote,
  )
  const combobox = useCombobox()
  const [tags, setTags] = useState<Tag[]>([])
  const [selectedTags, setSelectedTags] = useState<string[]>([])
  const [search, setSearch] = useState('')
  const color = useCurrentColor()

  useEffect(() => {
    if (noteId !== -1) {
      const noteTags = store.getState().notes.collection[noteId]?.tags ?? []

      if (noteTags.length > 0) {
        setSelectedTags(noteTags)
      }
    } else {
      const tags = store.getState().notes.filters.tags ?? []
      if (tags.length > 0) {
        setSelectedTags(tags)
      }
    }

    getTagsRequest(getCurrentSection(noteId), ({ data }) => {
      if (data) {
        setTags(data.map((tag) => ({ name: tag?.name ?? '', count: tag.count })))
      }
    })
  }, [getTagsRequest, noteId])

  if (isGetTagsLoading) {
    return <Loading full />
  }

  const handleValueRemove = (val: string) =>
    setSelectedTags((current) => current.filter((v) => v !== val))

  const handleCreateTag = () => {
    setTags((currentTags) => [{ name: search, count: 0 }, ...currentTags])
    setSelectedTags((currentSelectedTags) => [...currentSelectedTags, search])
    setSearch('')
  }

  const values = selectedTags.map((item) => (
    <div
      className={styles['pill']}
      key={item}
      style={{ backgroundColor: color }}
      onClick={() => handleValueRemove(item)}
    >
      {item} <MdClose size={18} />
    </div>
  ))

  const options = tags
    .filter((item) => item.name.toLowerCase().includes(search.toLowerCase().trim()))
    .map((item) => (
      <Combobox.Option
        value={item.name}
        key={item.name}
        active={selectedTags.includes(item.name)}
      >
        <Group gap="xs">
          <Checkbox
            color={color}
            checked={selectedTags.includes(item.name)}
            onChange={noop}
            aria-hidden
            tabIndex={-1}
            className={styles['checkbox']}
          />
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
            handleCreateTag()
          } else {
            setSelectedTags((currentSelectedTags) =>
              currentSelectedTags.includes(value)
                ? currentSelectedTags.filter((v) => v !== value)
                : [...currentSelectedTags, value],
            )
          }
        }}
      >
        <PillsInput
          classNames={{ input: styles['input'] }}
          onKeyUp={(e) => {
            if (
              e.key === 'Enter' &&
              (options.length === 0 || !tags.some((tag) => tag.name === search.trim())) &&
              search.trim().length >= 2 &&
              isEditTagsModal
            ) {
              handleCreateTag()
            }
          }}
        >
          <ScrollArea
            h={70}
            type="always"
            scrollbarSize={5}
            scrollbars="y"
          >
            <Pill.Group>
              {values}
              <Combobox.EventsTarget>
                <PillsInput.Field
                  value={search}
                  placeholder="Search tags"
                  onChange={(event) => {
                    combobox.updateSelectedOptionIndex()
                    setSearch(event.currentTarget.value)
                  }}
                />
              </Combobox.EventsTarget>
            </Pill.Group>
          </ScrollArea>
        </PillsInput>

        <div className={styles['list']}>
          <Combobox.Options>
            <ScrollArea
              h={200}
              type="always"
            >
              {options}
              {options.length === 0 && !isEditTagsModal && (
                <Combobox.Empty>Nothing found....</Combobox.Empty>
              )}
              {options.length === 0 && search.trim().length >= 2 && isEditTagsModal && (
                <Combobox.Option value="$create">+ Create new "{search}" tag </Combobox.Option>
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
          loading={isUpdateTagsLoading}
          onClick={() => {
            if (isEditTagsModal) {
              updateTagsRequest({ id: noteId, request: { tags: selectedTags } }, ({ data }) => {
                if (data) {
                  store.dispatch(setNote({ id: data.id, note: data }))
                  dispatchCrossTabEvent(events.note.updated, data)
                  showSuccess('Note tags are updated!')
                }
              })
            } else {
              store.dispatch(setFilters({ tags: selectedTags }))
              dispatchCustomEvent(events.notesList.search)
            }
            closeTagsModal()
          }}
        >
          {isEditTagsModal ? "Update note's tags" : 'Apply tags filter'}
        </Button>
      </Group>
    </>
  )
}

export default TagsModal
