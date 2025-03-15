import { useForm } from '@mantine/form'
import { sourcesApi } from '@/entities/source'
import useFetch from '@/shared/lib/useFetch'
import { setNoteSources } from '@/entities/note'
import store from '@/shared/lib/store'
import { events } from '@/shared/config'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import { showSuccess } from '@/shared/lib/notifications'

import { Fieldset, TextInput, Button, Group } from '@mantine/core'

export interface SectionFormModalProps {
  noteId: number
}

function AddNewSourceFormModal({ noteId }: SectionFormModalProps) {
  const { request, isLoading } = useFetch(sourcesApi.addNoteSource)
  const color = useCurrentColor()

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      link: '',
    },
    validate: {
      link: (value) => {
        if (!value) {
          return 'URL cannot be empty'
        }
        try {
          new URL(value)
          return null
        } catch {
          return 'Invalid URL'
        }
      },
    },
  })

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        request(
          {
            noteId,
            request: {
              link: values.link,
            },
          },
          ({ data }) => {
            if (data) {
              form.reset()
              store.dispatch(setNoteSources({ id: noteId, sources: data }))
              dispatchCrossTabEvent(events.note.updated, store.getState().notes.collection[noteId])
              showSuccess('New source is added!')
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
          key={form.key('link')}
          {...form.getInputProps('link')}
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
