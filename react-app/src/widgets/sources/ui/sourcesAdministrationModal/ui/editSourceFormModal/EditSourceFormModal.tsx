import { useForm } from '@mantine/form'
import { sourcesApi } from '@/entities/source'
import useFetch from '@/shared/lib/useFetch'
import { events } from '@/shared/config'
import store from '@/shared/lib/store'
import { setNoteSources } from '@/entities/note'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import { showSuccess } from '@/shared/lib/notifications'

import {
  Fieldset,
  TextInput,
  Textarea,
  Checkbox,
  Button,
  Group,
  BackgroundImage,
} from '@mantine/core'

import type { NoteSource } from '@/entities/note'

export interface EditSourceFormModalProps {
  noteId: number
  source: NoteSource
}

function EditSourceFormModal({ noteId, source }: EditSourceFormModalProps) {
  const { request, isLoading } = useFetch(sourcesApi.updateNoteSource)
  const color = useCurrentColor()

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      link: source.link,
      title: source.title,
      description: source.description,
      showImage: source.showImage,
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
            sourceId: source.id,
            request: {
              title: values.title,
              description: values.description,
              link: values.link,
              showImage: values.showImage,
            },
          },
          ({ data }) => {
            if (data) {
              store.dispatch(setNoteSources({ id: noteId, sources: data }))
              dispatchCrossTabEvent(events.note.updated, store.getState().notes.collection[noteId])
              showSuccess('Source is updated!')
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
        <TextInput
          withAsterisk
          label="Title"
          key={form.key('title')}
          {...form.getInputProps('title')}
        />
        <Textarea
          label="Description"
          key={form.key('description')}
          {...form.getInputProps('description')}
          mb={10}
        />
        {source.image && (
          <BackgroundImage
            mb={10}
            src={source.image}
            style={{ width: '100%', height: '200px' }}
          />
        )}
        <Checkbox
          color={color}
          label="Show image"
          key={form.key('showImage')}
          {...form.getInputProps('showImage', { type: 'checkbox' })}
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

export default EditSourceFormModal
