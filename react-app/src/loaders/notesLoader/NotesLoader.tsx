import { useEffect, useRef } from 'react'
import { useNotesStore } from '@/store'
import { notesActions } from '@/actions'
import useCrossTabEventListener from '@/hooks/useCrossTabEventListener'
import useCustomEventListener from '@/hooks/useCustomEventListener'
import events from '@/events'
import { useNavigationType, useSearchParams } from 'react-router-dom'
import rison from 'rison'

import Loading from '@/components/loading'

import type { CrossTabEvent } from '@/hooks/useCrossTabEventListener'
import type { Note } from '@/store/notes'

export interface NotesLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}

const handleCrossTabEvent = ({ eventName, payload }: CrossTabEvent) => {
  if (eventName === events.note.updated) {
    if (payload && (payload as Note)) {
      const newNote = payload as Note
      useNotesStore.getState().setNote(newNote.id, newNote)
    }
  }
}

const updateUrl = (
  setSearchParams: (callback: (searchParams: URLSearchParams) => URLSearchParams) => void,
) => {
  const listParams = notesActions.getUrlParams()

  setSearchParams((searchParams) => {
    searchParams.set('list', rison.encode(listParams))
    return searchParams
  })
}

function NotesLoader({ children }: NotesLoaderProps) {
  const navType = useNavigationType()
  const [searchParams, setSearchParams] = useSearchParams()

  const list = searchParams.get('list')

  const isLoading = useNotesStore((state) => state.isLoading)
  const page = useNotesStore((state) => state.page)

  const firstUpdate = useRef(true)

  useCustomEventListener(events.notesList.updateUrl, () => {
    updateUrl(setSearchParams)
  })

  useCrossTabEventListener(handleCrossTabEvent)

  useEffect(() => {
    if (firstUpdate.current) {
      firstUpdate.current = false
      if (list) {
        notesActions.updateFromUrl(list)
        notesActions.search()
      } else {
        const listParams = notesActions.getUrlParams()

        setSearchParams((searchParams) => {
          searchParams.set('list', rison.encode(listParams))
          return searchParams
        })
        notesActions.search()
      }
    } else {
      if (navType === 'POP' && list) {
        notesActions.updateFromUrl(list)
        notesActions.search()
      }
    }
  }, [list, navType, setSearchParams])

  if (isLoading && page === 1) {
    return <Loading full />
  }

  return children
}

export default NotesLoader
