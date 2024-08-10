import createFetch from '@/utils/createFetch'
import { tagsService } from '@/services'
import { useTagsStore, useSectionsStore } from '@/store'

import type { components } from '@/misc/openapi'
import type { Tag } from '@/store/tags'

function getTagFromResponse(tag: components['schemas']['TagItemResponse']): Tag {
  return {
    count: tag.count ?? 0,
    name: tag.name ?? '',
    selected: false,
  }
}

function setIsLoading(isLoading: boolean) {
  if (useTagsStore.getState().isMounted) {
    useTagsStore.getState().setIsLoading(isLoading)
  }
}

const getTagsListRequest = createFetch(tagsService.getTagsList, setIsLoading, { concurrent: true })

function getTags() {
  getTagsListRequest(
    {
      section: useSectionsStore.getState().currentSection?.name,
    },
    ({ data }) => {
      if (data && useTagsStore.getState().isMounted) {
        const tags = data?.map(getTagFromResponse) ?? []

        useTagsStore.getState().setTags(tags)
      }
    },
  )
}

export default { getTags }
