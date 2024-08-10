import { useEffect } from 'react'
import { useSectionsStore } from '@/store'
import { sectionsActions } from '@/actions'
import useCrossTabEventListener from '@/hooks/useCrossTabEventListener'
import events from '@/events'

import Loading from '@/components/loading'

import type { Section } from '@/store/sections'
import type { CrossTabEvent } from '@/hooks/useCrossTabEventListener'

export interface SectionsLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}

const handleCrossTabEvent = ({ eventName, payload }: CrossTabEvent) => {
  if (eventName === events.sections.updated) {
    if (payload && (payload as Section[])) {
      const newSections = payload as Section[]
      useSectionsStore.getState().setSections(newSections)
    }
  }
}

function SectionsLoader({ children }: SectionsLoaderProps) {
  const sectionsLoaded = useSectionsStore((state) => state.sectionsLoaded)

  useEffect(() => {
    sectionsActions.getSections()
  }, [])

  useCrossTabEventListener(handleCrossTabEvent)

  return !sectionsLoaded ? <Loading full /> : children
}

export default SectionsLoader
