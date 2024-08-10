import { handlers as accountsHandlers } from './accounts'
import { handlers as applicationHandlers } from './application'
import { handlers as sectionsHandlers } from './sections'
import { handlers as notesHandlers } from './notes'
import { handlers as booksHandlers } from './books'
import { handlers as tagsHandlers } from './tags'
import { handlers as sourcesHandlers } from './sources'

export const handlers = [
  ...accountsHandlers,
  ...applicationHandlers,
  ...sectionsHandlers,
  ...notesHandlers,
  ...booksHandlers,
  ...tagsHandlers,
  ...sourcesHandlers,
]
