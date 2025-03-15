import type { components } from '@/shared/api'

const sectionsMocks: components['schemas']['SectionsItemResponse'][] = [
  {
    id: 1,
    name: 'it',
    displayName: 'IT',
    color: '#f9a825',
  },
  {
    id: 2,
    name: 'dot_net',
    displayName: '.Net',
    color: '#607d8b',
  },
  {
    id: 3,
    name: 'front_end',
    displayName: 'Front-end',
    color: '#4caf50',
  },
  {
    id: 4,
    name: 'database',
    displayName: 'Database',
    color: '#44bdb9',
  },
]

export const getSectionsListResponseMock = sectionsMocks

export const createSectionRequestMock: components['schemas']['SectionsCreateRequest'] = {
  name: 'newSection',
  displayName: 'New Section',
  color: '#000000',
}

export const createSectionResponseMock = sectionsMocks

export const updateSectionRequestMock: {
  id: number
  request: components['schemas']['SectionsUpdateRequest']
} = {
  id: 1,
  request: {
    displayName: 'Updated Section 1',
    color: '#000000',
  },
}

export const updateSectionResponseMock = sectionsMocks

export const deleteSectionRequestMock = 1

export const deleteSectionResponseMock = sectionsMocks

export const reorderSectionsRequestMock: components['schemas']['SectionsReorderRequest'] = {
  ids: [3, 2, 1],
}

export const reorderSectionsResponseMock = sectionsMocks
