import type { components } from '@/shared/api'

export const getBooksListRequestMock = 'section1'

export const getBooksListResponseMock: components['schemas']['ItemNameCountResponse'][] = [
  {
    name: 'book1',
    count: 1,
  },
  {
    name: 'book2',
    count: 10,
  },
  {
    name: 'book3',
    count: 15,
  },
]
