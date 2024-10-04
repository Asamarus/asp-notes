import { render as testingLibraryRender } from '@testing-library/react'
import { MantineProvider } from '@mantine/core'
import { Notifications } from '@mantine/notifications'
import { theme } from '../src/app/config/theme'
import { MemoryRouter } from 'react-router-dom'

export function render(ui: React.ReactNode, { initialEntries } = { initialEntries: ['/'] }) {
  return testingLibraryRender(<>{ui}</>, {
    wrapper: ({ children }: { children: React.ReactNode }) => (
      <MantineProvider theme={theme}>
        <MemoryRouter initialEntries={initialEntries}>
          <Notifications />
          {children}
        </MemoryRouter>
      </MantineProvider>
    ),
  })
}
