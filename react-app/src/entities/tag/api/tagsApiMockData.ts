import type { components } from '@/shared/api'

export const getTagsListRequestMock = 'section1'

export const getTagsListResponseMock: components['schemas']['ItemNameCountResponse'][] = [
  {
    name: 'tag1',
    count: 1,
  },
  {
    name: 'tag2',
    count: 2,
  },
]
