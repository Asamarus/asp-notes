import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const SelectSectionForNewNoteModal = lazy(() => import('./SelectSectionForNewNoteModal'))

function SelectSectionForNewNoteModalLoader() {
  return (
    <ComponentLoader>
      <SelectSectionForNewNoteModal />
    </ComponentLoader>
  )
}

export default SelectSectionForNewNoteModalLoader
