import type { components } from '@/shared/api'

const testSources: components['schemas']['NotesSource'][] = [
  {
    id: '1',
    title: 'Source 1',
    link: 'https://example.com',
    description: 'Source 1 description',
    showImage: false,
  },
  {
    id: '2',
    title: 'Source 2',
    link: 'https://example.com',
    description: 'Source 2 description',
    showImage: false,
  },
]

export const addNoteSourceRequestMock: {
  noteId: number
  request: components['schemas']['NotesSourceCreateRequest']
} = {
  noteId: 1,
  request: {
    link: 'https://example.com',
  },
}

export const addNoteSourceResponseMock = testSources

export const updateNoteSourceRequestMock: {
  noteId: number
  sourceId: string
  request: components['schemas']['NotesSourceUpdateRequest']
} = {
  noteId: 1,
  sourceId: '1',
  request: {
    link: 'https://example.com',
    title: 'Source 1',
    description: 'Source 1 description',
    image: 'https://example.com/image.jpg',
    showImage: false,
  },
}

export const updateNoteSourceResponseMock = testSources

export const removeNoteSourceRequestMock: { noteId: number; sourceId: string } = {
  noteId: 1,
  sourceId: '1',
}

export const removeNoteSourceResponseMock = testSources

export const reorderNoteSourcesRequestMock: {
  noteId: number
  request: components['schemas']['NotesSourcesReorderRequest']
} = {
  noteId: 1,
  request: { sourceIds: ['1', '2'] },
}

export const reorderNoteSourcesResponseMock = testSources
