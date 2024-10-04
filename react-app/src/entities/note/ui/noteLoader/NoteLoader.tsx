import { useEffect } from 'react'
import { notesApi } from '../..'
import createFetch from '@/shared/lib/createFetch'
import store from '@/shared/lib/store'
import useAppSelector from '@/shared/lib/useAppSelector'
import { setNote } from '../..'
import noop from '@/shared/lib/noop'
import getNoteFromResponse from '../../model/getNoteFromResponse'

import Loading from '@/shared/ui/loading'

export interface NoteLoaderProps {
  /** Note's id */
  id: number
  /** The content of the component */
  children?: React.ReactNode
}

const getNoteRequest = createFetch(notesApi.getNote, noop)

function NoteLoader({ id, children }: NoteLoaderProps) {
  const noteIsEmpty = useAppSelector((state) => state.notes.collection[id]?.createdAt === undefined)

  useEffect(() => {
    if (store.getState().notes.collection[id]?.createdAt === undefined) {
      getNoteRequest({ id }, ({ data }) => {
        if (data) {
          const note = getNoteFromResponse(data.note)
          store.dispatch(setNote({ id: note.id, note }))
        }
      })
    }
  }, [id])

  if (noteIsEmpty) {
    return <Loading full />
  }

  return children
}

export default NoteLoader
