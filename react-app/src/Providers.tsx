import { BrowserRouter } from 'react-router-dom'
import { MantineProvider } from '@mantine/core'
import { theme } from './misc/theme'
import AccountsProvider from './providers/accountsProvider'
import ApplicationProvider from './providers/applicationProvider'
import { Notifications } from '@mantine/notifications'
import BrowserApiMockProvider from '@/mocks/api/BrowserApiMockProvider'

export interface ProvidersProps {
  /** The content of the component */
  children?: React.ReactNode
}
function Providers({ children }: ProvidersProps) {
  return (
    <BrowserApiMockProvider>
      <BrowserRouter>
        <MantineProvider theme={theme}>
          <Notifications position="top-center" />
          <AccountsProvider>
            <ApplicationProvider>{children}</ApplicationProvider>
          </AccountsProvider>
        </MantineProvider>
      </BrowserRouter>
    </BrowserApiMockProvider>
  )
}

export default Providers
