import '@mantine/core/styles.css'
import '@mantine/notifications/styles.css'
//import '@mantine/dates/styles.css'
//import '@mantine/code-highlight/styles.css'

import './App.css'

import ErrorBoundary from './components/errorBoundary'
import ModalsManager from '@/components/modalsManager'
import appModals from '@/modals'
import { defaultModals } from '@/components/modalsManager'
import MainLayout from './components/mainLayout'

import Content from './Content'

import Providers from './Providers'
import commonModalProps from './modals/commonModalProps'

const modals = { ...defaultModals, ...appModals }

export default function App() {
  return (
    <Providers>
      <ErrorBoundary>
        <MainLayout>
          <Content />
        </MainLayout>
        <ModalsManager
          modals={modals}
          commonModalProps={commonModalProps}
        />
      </ErrorBoundary>
    </Providers>
  )
}
