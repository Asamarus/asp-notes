import { useEffect } from 'react'
import { useForm } from '@mantine/form'
import useFetch from '@/shared/lib/useFetch'
import { notesApi, setNote, setNoteIsNotSaved } from '@/entities/note'
import { events } from '@/shared/config'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import useAppSelector from '@/shared/lib/useAppSelector'
import store from '@/shared/lib/store'
import useCurrentColor from '@/shared/lib/useCurrentColor'
import { showSuccess } from '@/shared/lib/notifications'

import NoteEditor from '@/shared/ui/noteEditor'
import { Fieldset, TextInput, Button, Group, Input } from '@mantine/core'

import styles from './Edit.module.css'

export interface EditProps {
  /** Note's id */
  id: number
}

function Edit({ id }: EditProps) {
  const { request, isLoading } = useFetch(notesApi.updateNote)
  const color = useCurrentColor()
  const title = useAppSelector((state) => state.notes.collection[id]?.title ?? '')
  const content = useAppSelector((state) => state.notes.collection[id]?.content ?? '')

  const form = useForm({
    mode: 'uncontrolled',
    initialValues: {
      title,
      content,
    },
    onValuesChange: () => {
      // TODO: check if note is changed
      store.dispatch(setNoteIsNotSaved(true))
    },
  })

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

  const { onBlur: noteEditorOnBlur } = form.getInputProps('content')

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        request(
          {
            id: id,
            request: {
              title: values.title,
              content: values.content,
            },
          },
          ({ data }) => {
            if (data) {
              store.dispatch(setNote({ id, note: data }))
              dispatchCrossTabEvent(events.note.updated, data)
              if (store.getState().notes.noteIsNotSaved) {
                store.dispatch(setNoteIsNotSaved(false))
              }

              showSuccess('Note is updated!')
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
          key={form.key('title')}
          {...form.getInputProps('title')}
        />
        <Input.Wrapper label="Content">
          <div className={styles['editor']}>
            <NoteEditor
              disabled={isLoading}
              value={content}
              onBlur={noteEditorOnBlur}
              onChange={(_, editor) => {
                form.setFieldValue('content', editor.getData())
                //store.dispatch(setNoteIsNotSaved(true))
              }}
            />
          </div>
        </Input.Wrapper>
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
