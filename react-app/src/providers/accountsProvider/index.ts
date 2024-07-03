import { createContext } from 'react'
import AccountsProvider from './AccountsProvider'

import type { Context } from './types'

export const AccountsContext = createContext<Context>({} as Context)

export default AccountsProvider
