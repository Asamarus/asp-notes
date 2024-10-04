import type { Note } from './types'
import type { components } from '@/shared/api'

function getNoteFromResponse(note: components['schemas']['NoteItemResponse']): Note {
  return {
    id: note.id ?? 0,
    createdAt: note.createdAt ?? '',
    updatedAt: note.updatedAt ?? '',
    title: note.title ?? '',
    section: note.section ?? '',
    content: note.content ?? '',
    preview: note.preview ?? '',
    tags: note.tags ?? [],
    book: note.book ?? '',
    sources:
      note.sources?.map((source) => ({
        id: source.id ?? '',
        title: source.title ?? '',
        link: source.link ?? '',
        description: source.description ?? '',
        image: source.image ?? '',
        showImage: source.showImage ?? false,
      })) ?? [],
  }
}

export default getNoteFromResponse
