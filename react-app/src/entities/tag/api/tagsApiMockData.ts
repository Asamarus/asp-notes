import type { components } from '@/shared/api'

export const getTagsListRequestMock: components['schemas']['GetTagsListRequest'] = {
  section: 'section1',
}

export const getTagsListResponseMock: components['schemas']['TagItemResponse'][] = [
  {
    name: 'tag1',
    count: 1,
  },
  {
    name: 'tag2',
    count: 2,
  },
]

export const autocompleteTagsRequestMock: components['schemas']['AutocompleteTagsRequest'] = {
  searchTerm: 'tag',
  section: 'section1',
}

export const autocompleteTagsResponseMock: string[] = ['tag1', 'tag2']
