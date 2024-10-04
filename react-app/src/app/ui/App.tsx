import '@mantine/core/styles.css'
import '@mantine/notifications/styles.css'
import '@mantine/dates/styles.css'
//import '@mantine/code-highlight/styles.css'

import './App.css'

import ErrorBoundary from '@/shared/ui/errorBoundary'
import ModalsManager, { defaultModals } from '@/shared/ui/modalsManager'
import { modalData as noteModalData } from '@/widgets/noteModal'

import Pages from './Pages'

import Providers from './Providers'
import commonModalProps from '../config/commonModalProps'

const appModals = {
  [noteModalData.name]: noteModalData,
}

const modals = { ...defaultModals, ...appModals }

export default function App() {
  return (
    <Providers>
      <ErrorBoundary>
        <Pages />
        <ModalsManager
          modals={modals}
          commonModalProps={commonModalProps}
        />
      </ErrorBoundary>
    </Providers>
  )
}
