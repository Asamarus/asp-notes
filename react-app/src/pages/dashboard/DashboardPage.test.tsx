import { render, screen } from '@test-utils'
import { ApplicationContext } from '@/providers/applicationProvider'
import DashboardPage from './DashboardPage'

describe('DashboardPage', () => {
  it('renders the title and text', () => {
    render(
      <ApplicationContext.Provider value={{ initialData: null }}>
        <DashboardPage />
      </ApplicationContext.Provider>,
    )

    expect(screen.getByText('Dashboard')).toBeInTheDocument()
    expect(screen.getByText('Paper is the most basic ui component')).toBeInTheDocument()
  })

  it('renders the initial data if it exists', () => {
    const initialData = { title: 'Test title', someData: 'Test data' }

    render(
      <ApplicationContext.Provider value={{ initialData }}>
        <DashboardPage />
      </ApplicationContext.Provider>,
    )

    expect(screen.getByText(`initialData.title: ${initialData.title}`)).toBeInTheDocument()
    expect(screen.getByText(`initialData.someData: ${initialData.someData}`)).toBeInTheDocument()
  })
})
