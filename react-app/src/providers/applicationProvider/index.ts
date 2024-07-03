import { createContext } from 'react'
import ApplicationProvider from './ApplicationProvider'

import type { Context } from './types'

export const ApplicationContext = createContext<Context>({} as Context)

export default ApplicationProvider
