import { BrowserRouter } from 'react-router-dom'
import { MantineProvider } from '@mantine/core'
import { Provider } from 'react-redux'
import { theme } from '../config/theme'
import { Notifications } from '@mantine/notifications'
//import { BrowserApiMockProvider } from '@/app/lib/msw'
import ApplicationLoader from './ApplicationLoader'

import store from '../model/store'

export interface ProvidersProps {
  /** The content of the component */
  children?: React.ReactNode
}
function Providers({ children }: ProvidersProps) {
  return (
    //<BrowserApiMockProvider>
    <BrowserRouter>
      <MantineProvider theme={theme}>
        <Provider store={store}>
          <Notifications position="top-center" />
          <ApplicationLoader>{children}</ApplicationLoader>
        </Provider>
      </MantineProvider>
    </BrowserRouter>
    //</BrowserApiMockProvider>
  )
}

export default Providers
