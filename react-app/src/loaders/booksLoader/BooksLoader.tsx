import { useEffect } from 'react'
import { useBooksStore } from '@/store'
import { booksActions } from '@/actions'

import Loading from '@/components/loading'

export interface BooksLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function BooksLoader({ children }: BooksLoaderProps) {
  const isLoading = useBooksStore((state) => state.isLoading)

  useEffect(() => {
    useBooksStore.getState().setIsMounted(true)
    booksActions.getBooks()
    return () => {
      useBooksStore.getState().setIsMounted(false)
      useBooksStore.getState().reset()
    }
  }, [])

  return isLoading ? <Loading full /> : children
}

export default BooksLoader
