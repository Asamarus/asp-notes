import { useForm, Controller } from 'react-hook-form'
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

type Inputs = {
  section: string
}

function SelectSectionForNewNoteModal() {
  const { request, isLoading } = useFetch(notesApi.createNote)
  const color = useCurrentColor()
  const sections = useAppSelector((state) => state.sections.list)

  const {
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<Inputs>({})

  return (
    <form
      onSubmit={handleSubmit((formData) => {
        const book = store.getState().notes.filters.book
        request(
          {
            section: formData.section,
            ...(book && {
              book: book,
            }),
          },
          ({ data }) => {
            if (data) {
              closeSelectSectionForNewNoteModal()
              dispatchCustomEvent(events.notesList.search)
              if (data.note.id) {
                openNoteModal({ id: data.note.id, tab: 'edit' })
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
        <Controller
          name="section"
          control={control}
          rules={{
            required: 'Section is required',
          }}
          render={({ field }) => (
            <Select
              withAsterisk
              label="Section"
              {...field}
              data={sections.map((o) => ({ value: o.name, label: o.displayName }))}
              error={errors.section && errors.section.message}
            />
          )}
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
