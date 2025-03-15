import { useForm } from '@mantine/form'
import { notesApi } from '@/entities/note'
import useFetch from '@/shared/lib/useFetch'
import useAppSelector from '@/shared/lib/useAppSelector'
import { closeChangeSectionModal } from '.'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import { Select, Fieldset, Group, Button } from '@mantine/core'

export interface ChangeSectionModalProps {
  noteId: number
  closeNoteModal?: () => void
}

function ChangeSectionModal({ noteId, closeNoteModal }: ChangeSectionModalProps) {
  const { request, isLoading } = useFetch(notesApi.updateNote)
  const color = useCurrentColor()
  const section = useAppSelector((state) => state.notes.collection[noteId]?.section ?? '')
  const sections = useAppSelector((state) => state.sections.list)

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      section: section,
    },
    validate: {
      section: (value) => (!value ? 'Section is required' : null),
    },
  })

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        request(
          {
            id: noteId,
            request: { section: values.section },
          },
          ({ data }) => {
            if (data) {
              closeChangeSectionModal()
              closeNoteModal?.()
              dispatchCustomEvent(events.notesList.search)
              dispatchCrossTabEvent(events.note.updated, data)
            }
          },
        )
      })}
    >
      <Fieldset
        disabled={isLoading}
        variant="unstyled"
      >
        <Select
          withAsterisk
          label="Section"
          data={sections.map((o) => ({ value: o.name, label: o.displayName }))}
          key={form.key('section')}
          {...form.getInputProps('section')}
        />
        <Group
          justify="flex-end"
          mt="md"
        >
          <Button
            color={color}
            type="submit"
            loading={isLoading}
          >
            Save
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default ChangeSectionModal
