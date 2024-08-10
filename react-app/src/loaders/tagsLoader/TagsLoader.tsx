import { useEffect } from 'react'
import { useTagsStore } from '@/store'
import { tagsActions } from '@/actions'

import Loading from '@/components/loading'

export interface TagsLoaderProps {
  /** The content of the component */
  children?: React.ReactNode
}
function TagsLoader({ children }: TagsLoaderProps) {
  const isLoading = useTagsStore((state) => state.isLoading)

  useEffect(() => {
    useTagsStore.getState().setIsMounted(true)
    tagsActions.getTags()
    return () => {
      useTagsStore.getState().setIsMounted(false)
      useTagsStore.getState().reset()
    }
  }, [])

  return isLoading ? <Loading full /> : children
}

export default TagsLoader
