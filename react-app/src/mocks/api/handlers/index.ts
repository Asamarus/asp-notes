import { handlers as accountsHandlers } from './accounts'
import { handlers as applicationHandlers } from './application'

export const handlers = [...accountsHandlers, ...applicationHandlers]
