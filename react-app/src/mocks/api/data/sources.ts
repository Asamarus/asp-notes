import type { components } from '@/misc/openapi'

const testSources = [
  {
    id: '1',
    title: 'Source 1',
    link: 'https://example.com',
    description: 'Source 1 description',
    image: 'https://example.com/image.jpg',
    showImage: false,
  },
  {
    id: '2',
    title: 'Source 2',
    link: 'https://example.com',
    description: 'Source 2 description',
    image: 'https://example.com/image.jpg',
    showImage: false,
  },
]

export const addNoteSourceRequestMock: components['schemas']['AddNoteSourceRequest'] = {
  noteId: 1,
  link: 'https://example.com',
}

export const addNoteSourceResponseMock: components['schemas']['SourcesResponse'] = {
  message: 'New note source is created successfully!',
  sources: testSources,
}

export const updateNoteSourceRequestMock: components['schemas']['UpdateNoteSourceRequest'] = {
  noteId: 1,
  sourceId: '1',
  link: 'https://example.com',
  title: 'Source 1',
  description: 'Source 1 description',
  image: 'https://example.com/image.jpg',
  showImage: false,
}

export const updateNoteSourceResponseMock: components['schemas']['SourcesResponse'] = {
  message: 'New note source is updated successfully!',
  sources: testSources,
}

export const removeNoteSourceRequestMock: components['schemas']['RemoveNoteSourceRequest'] = {
  noteId: 1,
  sourceId: '1',
}

export const removeNoteSourceResponseMock: components['schemas']['SourcesResponse'] = {
  message: 'Note source is removed successfully!',
  sources: testSources,
}

export const reorderNoteSourcesRequestMock: components['schemas']['ReorderNoteSourcesRequest'] = {
  noteId: 1,
  sourceIds: ['1', '2'],
}

export const reorderNoteSourcesResponseMock: components['schemas']['SourcesResponse'] = {
  message: 'Note sources are reordered successfully!',
  sources: testSources,
}
