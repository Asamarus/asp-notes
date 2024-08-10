import { useEffect } from 'react'
import { useNotesStore } from '@/store'
import { noteActions } from '@/actions'

import Loading from '@/components/loading'

export interface NoteLoaderProps {
  /** Note's id */
  id: number
  /** The content of the component */
  children?: React.ReactNode
}

function NoteLoader({ id, children }: NoteLoaderProps) {
  const createAt = useNotesStore((state) => state.notes[id]?.createdAt)

  useEffect(() => {
    if (!useNotesStore.getState().notes[id]?.createdAt) {
      noteActions.getNoteById(id)
    }
  }, [id])

  if (!createAt) {
    return <Loading full />
  }

  return children
}

export default NoteLoader
