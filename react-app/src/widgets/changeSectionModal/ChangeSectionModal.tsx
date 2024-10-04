import { useForm, Controller } from 'react-hook-form'
import { getNoteFromResponse, notesApi } from '@/entities/note'
import useFetch from '@/shared/lib/useFetch'
import useAppSelector from '@/shared/lib/useAppSelector'
import { closeChangeSectionModal } from '.'
import { events } from '@/shared/config'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import { Select, Fieldset, Group, Button } from '@mantine/core'

type Inputs = {
  section: string
}

export interface ChangeSectionModalProps {
  noteId: number
}

function ChangeSectionModal({ noteId }: ChangeSectionModalProps) {
  const { request, isLoading } = useFetch(notesApi.updateNoteSection)
  const color = useCurrentColor()
  const section = useAppSelector((state) => state.notes.collection[noteId]?.section ?? '')
  const sections = useAppSelector((state) => state.sections.list)

  const {
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<Inputs>({
    defaultValues: {
      section: section,
    },
  })

  return (
    <form
      onSubmit={handleSubmit((data) => {
        request(
          {
            id: noteId,
            section: data.section,
          },
          ({ data }) => {
            if (data) {
              closeChangeSectionModal()
              dispatchCustomEvent(events.notesList.search)
              dispatchCrossTabEvent(events.note.updated, getNoteFromResponse(data.note))
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
            Save
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default ChangeSectionModal
