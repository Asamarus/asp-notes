import useAppSelector from '@/shared/lib/useAppSelector'
import { useComputedColorScheme } from '@mantine/core'
import { notesApi } from '@/entities/note'
import useFetch from '@/shared/lib/useFetch'
import store from '@/shared/lib/store'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { openNoteModal } from '@/widgets/noteModal'
import { openSelectSectionForNewNoteModal } from './ui/changeSectionModal'

import { Button, Tooltip } from '@mantine/core'
import { MdAdd } from 'react-icons/md'

function AddNote() {
  const { request, isLoading } = useFetch(notesApi.createNote)
  const colorScheme = useComputedColorScheme('light')
  const noSections = useAppSelector((state) => state.sections.list.length === 0)
  const sectionColor = useAppSelector((state) => state.sections.current?.color)
  const sectionName = useAppSelector((state) => state.sections.current?.name)
  const allNotesSectionName = useAppSelector((state) => state.sections.allNotesSection.name)

  const color = colorScheme === 'dark' ? '#3b3b3b' : sectionColor
  const isAllNotesSection = sectionName === allNotesSectionName
  let label = isAllNotesSection ? 'Add new note' : `Add new note to ${sectionName}`

  if (noSections) {
    label = 'You need to create a section first'
  }

  return (
    <Tooltip label={label}>
      <Button
        disabled={noSections}
        loading={isLoading}
        mb={10}
        fullWidth
        color={color}
        leftSection={<MdAdd size={20} />}
        onClick={() => {
          dispatchCustomEvent(events.drawer.close)
          if (isAllNotesSection) {
            openSelectSectionForNewNoteModal()
          } else {
            const book = store.getState().notes.filters.book
            request(
              {
                section: store.getState().sections.current?.name ?? '',
                ...(book && {
                  book: book,
                }),
              },
              ({ data }) => {
                if (data) {
                  dispatchCustomEvent(events.notesList.search)
                  if (data.note.id) {
                    openNoteModal({ id: data.note.id, tab: 'edit' })
                  }
                }
              },
            )
          }
        }}
      >
        Add new note
      </Button>
    </Tooltip>
  )
}

export default AddNote
