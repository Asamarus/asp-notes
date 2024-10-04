import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const TagsModal = lazy(() => import('./TagsModal'))

export interface TagsModalLoaderProps {
  noteId?: number
}

function TagsModalLoader({ noteId }: TagsModalLoaderProps) {
  return (
    <ComponentLoader>
      <TagsModal noteId={noteId} />
    </ComponentLoader>
  )
}

export default TagsModalLoader
