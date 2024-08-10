import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useAutocompleteStore, useNotesStore, useSectionsStore } from '@/store'

import Loading from '@/components/loading'

export interface NotesSectionLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function NotesSectionLoader({ children }: NotesSectionLoaderProps) {
  const { section } = useParams<{ section: string }>()
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    setLoading(true)

    if (section) {
      const sections = useSectionsStore.getState().sections
      const sectionData = sections.find((x) => x.name === section)

      if (sectionData) {
        useSectionsStore.getState().setCurrentSection(sectionData)
      }
    } else {
      useSectionsStore.getState().setCurrentSection({
        id: 0,
        name: 'all',
        displayName: 'Notes',
        color: '#1e88e5',
      })
    }

    useAutocompleteStore.getState().reset()
    useAutocompleteStore.getState().setSearchTerm('')
    useNotesStore.getState().reset()

    setLoading(false)
  }, [section])

  if (loading) {
    return <Loading full />
  }

  return children
}

export default NotesSectionLoader
