import { applicationDataApiMockHandlers } from '@/entities/applicationData'
import { sectionsApiMockHandlers } from '@/entities/section'
import { notesApiMockHandlers } from '@/entities/note'
import { sourcesApiMockHandlers } from '@/entities/source'
import { booksApiMockHandlers } from '@/entities/book'
import { tagsApiMockHandlers } from '@/entities/tag'
import { usersApiMockHandlers } from '@/entities/user'

export const handlers = [
  ...applicationDataApiMockHandlers,
  ...sectionsApiMockHandlers,
  ...notesApiMockHandlers,
  ...sourcesApiMockHandlers,
  ...booksApiMockHandlers,
  ...tagsApiMockHandlers,
  ...usersApiMockHandlers,
]
