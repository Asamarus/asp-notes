import { lazy } from 'react'
import ComponentLoader from '@/shared/ui/componentLoader'

const BooksModal = lazy(() => import('./BooksModal'))

export interface BooksModalLoaderProps {
  noteId?: number
}

function BooksModalLoader({ noteId }: BooksModalLoaderProps) {
  return (
    <ComponentLoader>
      <BooksModal noteId={noteId} />
    </ComponentLoader>
  )
}

export default BooksModalLoader
