import { BrowserRouter } from 'react-router-dom'
import { MantineProvider } from '@mantine/core'
import { Provider } from 'react-redux'
import { theme } from '../config/theme'
import { Notifications } from '@mantine/notifications'
import { BrowserApiMockProvider } from '@/app/lib/msw'
import UserLoader from './UserLoader'
import ApplicationDataLoader from './ApplicationDataLoader'

import store from '../model/store'

export interface ProvidersProps {
  /** The content of the component */
  children?: React.ReactNode
}
function Providers({ children }: ProvidersProps) {
  return (
    <BrowserApiMockProvider>
      <BrowserRouter>
        <MantineProvider theme={theme}>
          <Provider store={store}>
            <Notifications position="top-center" />
            <UserLoader>
              <ApplicationDataLoader>{children}</ApplicationDataLoader>
            </UserLoader>
          </Provider>
        </MantineProvider>
      </BrowserRouter>
    </BrowserApiMockProvider>
  )
}

export default Providers
