import { render, screen, waitFor } from '@test-utils'
import { ApplicationContext } from './index'
import ApplicationProvider from './ApplicationProvider'
import { useContext } from 'react'

vi.mock('@/services', async (importOriginal) => {
  const mockData = { title: 'Test Title', someData: 'Test Data' }
  const mod = await importOriginal<typeof import('@/services')>()
  return {
    ...mod,
    // replace some exports
    applicationService: {
      ...mod.applicationService,
      getInitialData: vi.fn().mockResolvedValue({ data: mockData }),
    },
  }
})

function TestConsumer() {
  const { initialData } = useContext(ApplicationContext)
  return (
    <div data-testid="application-context-provider">
      {initialData?.title}
      {initialData?.someData}
    </div>
  )
}

describe('ApplicationProvider', () => {
  afterAll(() => {
    vi.restoreAllMocks()
  })

  it('should display loading initially', async () => {
    render(<ApplicationProvider />)

    await waitFor(() => {
      expect(screen.getByLabelText('Loading')).toBeInTheDocument()
    })
  })

  it('should render ApplicationContext.Provider with correct state on successful fetch', async () => {
    render(
      <ApplicationProvider>
        <TestConsumer />
      </ApplicationProvider>,
    )

    await waitFor(() => {
      expect(screen.getByTestId('application-context-provider')).toHaveTextContent('Test Title')
    })
    await waitFor(() => {
      expect(screen.getByTestId('application-context-provider')).toHaveTextContent('Test Data')
    })
  })

  it('should still display loading on unsuccessful fetch', async () => {
    const { applicationService } = await import('@/services')

    // Modify the mock for this specific test
    applicationService.getInitialData = vi.fn().mockResolvedValue({ error: 'Fetch failed' })

    render(
      <ApplicationProvider>
        <TestConsumer />
      </ApplicationProvider>,
    )

    await waitFor(() => {
      expect(screen.getByLabelText('Loading')).toBeInTheDocument()
    })
  })
})
