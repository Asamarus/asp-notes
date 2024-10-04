import { notesApiMockHandlers } from '@/entities/note'
import { sectionsApiMockHandlers } from '@/entities/section'
import { sourcesApiMockHandlers } from '@/entities/source'
import { booksApiMockHandlers } from '@/entities/book'
import { tagsApiMockHandlers } from '@/entities/tag'
import { usersApiMockHandlers } from '@/entities/user'

export const handlers = [
  ...sectionsApiMockHandlers,
  ...notesApiMockHandlers,
  ...sourcesApiMockHandlers,
  ...booksApiMockHandlers,
  ...tagsApiMockHandlers,
  ...usersApiMockHandlers,
]
