import { BrowserRouter } from 'react-router-dom'
import { MantineProvider } from '@mantine/core'
import { theme } from '@/misc/theme'
import { Notifications } from '@mantine/notifications'
import BrowserApiMockProvider from '@/mocks/api/BrowserApiMockProvider'
import AccountsLoader from '@/loaders/accountsLoader'
import ApplicationLoader from '@/loaders/appplicationLoader'
import SectionsLoader from '@/loaders/sectionsLoader'

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
          <AccountsLoader>
            <ApplicationLoader>
              <SectionsLoader>{children}</SectionsLoader>
            </ApplicationLoader>
          </AccountsLoader>
        </MantineProvider>
      </BrowserRouter>
    </BrowserApiMockProvider>
  )
}

export default Providers
