import { useForm } from 'react-hook-form'
import { sourcesApi } from '@/entities/source'
import useFetch from '@/shared/lib/useFetch'
import { events } from '@/shared/config'
import store from '@/shared/lib/store'
import { getNoteSourcesFromResponse, setNoteSources } from '@/entities/note'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useCurrentColor from '@/shared/lib/useCurrentColor'

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

type Inputs = {
  link: string
  title: string
  description: string
  showImage: boolean
}

function EditSourceFormModal({ noteId, source }: EditSourceFormModalProps) {
  const { request, isLoading } = useFetch(sourcesApi.updateNoteSource)
  const color = useCurrentColor()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Inputs>({
    defaultValues: {
      link: source.link,
      title: source.title,
      description: source.description,
      showImage: source.showImage,
    },
  })

  return (
    <form
      onSubmit={handleSubmit((formData) => {
        request(
          {
            noteId,
            sourceId: source.id,
            link: formData.link,
            description: formData.description,
            showImage: formData.showImage,
          },
          ({ data }) => {
            if (data) {
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
        <TextInput
          withAsterisk
          label="Title"
          {...register('title', {
            required: 'Title is required',
          })}
          error={errors.title && errors.title.message}
        />
        <Textarea
          label="Description"
          {...register('description')}
          error={errors.description && errors.description.message}
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
          {...register('showImage')}
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
