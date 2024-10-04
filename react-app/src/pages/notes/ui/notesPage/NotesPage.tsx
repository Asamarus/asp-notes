import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { setCurrentSection } from '@/entities/section'
import store from '@/shared/lib/store'
import useAppSelector from '@/shared/lib/useAppSelector'

import Loading from '@/shared/ui/loading'
import Header from '../header'
import NotesFiltersStatusBar from '@/widgets/notesFiltersStatusBar'
import Sidebar from '../sidebar'
import Content from '../content'
import WrapperComponent from '@/shared/ui/wrapperComponent'

function NotesPage() {
  const { section } = useParams<{ section: string }>()
  const [isLoading, setIsLoading] = useState(true)
  const currentSectionName = useAppSelector((state) => state.sections.current?.name)

  useEffect(() => {
    setIsLoading(true)
    if (section) {
      const sections = store.getState().sections.list
      const sectionData = sections.find((x) => x.name === section)

      if (sectionData) {
        store.dispatch(setCurrentSection(sectionData))
      }
    } else {
      const allNotesSection = store.getState().sections.allNotesSection
      store.dispatch(setCurrentSection({ ...allNotesSection, id: 0 }))
    }

    setIsLoading(false)
  }, [section])

  if (isLoading) {
    return <Loading full />
  }

  return (
    <WrapperComponent key={currentSectionName}>
      <Header />
      <NotesFiltersStatusBar />
      <Sidebar />
      <Content />
    </WrapperComponent>
  )
}

export default NotesPage
