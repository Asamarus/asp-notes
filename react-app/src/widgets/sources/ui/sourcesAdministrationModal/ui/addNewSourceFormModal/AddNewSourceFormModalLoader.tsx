import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const AddNewSourceFormModal = lazy(() => import('./AddNewSourceFormModal'))

export interface AddNewSourceFormModalLoaderProps {
  noteId: number
}

function AddNewSourceFormModalLoader({ noteId }: AddNewSourceFormModalLoaderProps) {
  return (
    <ComponentLoader>
      <AddNewSourceFormModal noteId={noteId} />
    </ComponentLoader>
  )
}

export default AddNewSourceFormModalLoader
