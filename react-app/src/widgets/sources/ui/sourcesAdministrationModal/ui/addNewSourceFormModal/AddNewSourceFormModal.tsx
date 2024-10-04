import { useForm } from 'react-hook-form'
import { sourcesApi } from '@/entities/source'
import useFetch from '@/shared/lib/useFetch'
import { getNoteSourcesFromResponse, setNoteSources } from '@/entities/note'
import store from '@/shared/lib/store'
import { events } from '@/shared/config'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import { Fieldset, TextInput, Button, Group } from '@mantine/core'

export interface SectionFormModalProps {
  noteId: number
}

type Inputs = {
  link: string
}

function AddNewSourceFormModal({ noteId }: SectionFormModalProps) {
  const { request, isLoading } = useFetch(sourcesApi.addNoteSource)
  const color = useCurrentColor()

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<Inputs>({
    defaultValues: {
      link: '',
    },
  })

  return (
    <form
      onSubmit={handleSubmit((formData) => {
        request(
          {
            noteId,
            link: formData.link,
          },
          ({ data }) => {
            if (data) {
              reset()
              const payload = getNoteSourcesFromResponse(data.sources ?? [])
              store.dispatch(setNoteSources({ id: noteId, sources: payload }))
              dispatchCrossTabEvent(events.note.updated, store.getState().notes.collection[noteId])
            }
          },
        )
      })}
    >
      <Fieldset
        disabled={isLoading}
        variant="unstyled"
      >
        <TextInput
          withAsterisk
          label="Link"
          {...register('link', {
            required: 'Link is required',
          })}
          error={errors.link && errors.link.message}
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

export default AddNewSourceFormModal
