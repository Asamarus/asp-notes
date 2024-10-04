import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import useFetch from '@/shared/lib/useFetch'
import { getNoteFromResponse, notesApi, setNote, setNoteIsNotSaved } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useAppSelector from '@/shared/lib/useAppSelector'
import store from '@/shared/lib/store'
import useCurrentColor from '@/shared/lib/useCurrentColor'

import { Controller } from 'react-hook-form'
import NoteEditor from '@/shared/ui/noteEditor'
import { Fieldset, TextInput, Button, Group, Input } from '@mantine/core'

import styles from './Edit.module.css'

export interface EditProps {
  /** Note's id */
  id: number
}

type Inputs = {
  title: string
  content: string
}

function Edit({ id }: EditProps) {
  const { request, isLoading } = useFetch(notesApi.updateNote)
  const color = useCurrentColor()
  const title = useAppSelector((state) => state.notes.collection[id]?.title ?? '')
  const content = useAppSelector((state) => state.notes.collection[id]?.content ?? '')

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { isDirty },
  } = useForm<Inputs>({
    defaultValues: {
      title: title,
      content: content,
    },
  })

  useEffect(() => {
    if (store.getState().notes.noteIsNotSaved !== isDirty) {
      store.dispatch(setNoteIsNotSaved(isDirty))
    }
  }, [isDirty])

  useEffect(() => {
    const listener = (event: BeforeUnloadEvent) => {
      const notSaved = store.getState().notes.noteIsNotSaved
      if (notSaved) {
        event.preventDefault()
        event.returnValue = true
      }
    }
    window.addEventListener('beforeunload', listener)
    return () => window.removeEventListener('beforeunload', listener)
  }, [])

  return (
    <form
      onSubmit={handleSubmit((formData) => {
        request(
          {
            id: id,
            title: formData.title,
            content: formData.content,
          },
          ({ data }) => {
            if (data) {
              const note = getNoteFromResponse(data.note)
              store.dispatch(setNote({ id, note }))
              dispatchCrossTabEvent(events.note.updated, note)
              if (store.getState().notes.noteIsNotSaved) {
                store.dispatch(setNoteIsNotSaved(false))
              }

              reset({ title: note.title, content: note.content })
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
          label="Title"
          {...register('title')}
        />
        <Controller
          name="content"
          control={control}
          render={({ field }) => (
            <Input.Wrapper label="Content">
              <div className={styles['editor']}>
                <NoteEditor
                  disabled={isLoading}
                  value={field.value}
                  onBlur={field.onBlur}
                  onChange={(_, editor) => {
                    const value = editor.getData()
                    field.onChange(value)
                  }}
                />
              </div>
            </Input.Wrapper>
          )}
        />
        <Group
          justify="flex-end"
          mt="md"
        >
          <Button
            type="submit"
            loading={isLoading}
            color={color}
          >
            Save
          </Button>
        </Group>
      </Fieldset>
    </form>
  )
}

export default Edit
