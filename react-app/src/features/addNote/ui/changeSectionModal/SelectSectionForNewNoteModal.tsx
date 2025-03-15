import { useForm } from '@mantine/form'
import { notesApi } from '@/entities/note'
import useFetch from '@/shared/lib/useFetch'
import useAppSelector from '@/shared/lib/useAppSelector'
import { closeSelectSectionForNewNoteModal } from '.'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import store from '@/shared/lib/store'
import { openNoteModal } from '@/widgets/noteModal'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import { Select, Fieldset, Group, Button } from '@mantine/core'

function SelectSectionForNewNoteModal() {
  const { request, isLoading } = useFetch(notesApi.createNote)
  const color = useCurrentColor()
  const sections = useAppSelector((state) => state.sections.list)

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      section: '',
    },
    validate: {
      section: (value) => (!value ? 'Section is required' : null),
    },
  })

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        const book = store.getState().notes.filters.book
        request(
          {
            section: values.section,
            ...(book && {
              book: book,
            }),
          },
          ({ data }) => {
            if (data) {
              closeSelectSectionForNewNoteModal()
              dispatchCustomEvent(events.notesList.search)
              if (data.id) {
                openNoteModal({ id: data.id, tab: 'edit' })
              }
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
            Add
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default SelectSectionForNewNoteModal
