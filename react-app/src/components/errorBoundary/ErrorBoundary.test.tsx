import { render, screen } from '@test-utils'
import ErrorBoundary from './ErrorBoundary'

// Mock component that throws an error
const ErrorComponent = () => {
  throw new Error('Test error')
}

describe('ErrorBoundary', () => {
  it('renders children correctly when there is no error', () => {
    render(
      <ErrorBoundary>
        <div>Test Child</div>
      </ErrorBoundary>,
    )

    expect(screen.getByText('Test Child')).toBeInTheDocument()
  })

  it('catches error and displays error UI', () => {
    vi.spyOn(console, 'error').mockImplementation(() => {}) // Silence console.error for this test

    render(
      <ErrorBoundary>
        <ErrorComponent />
      </ErrorBoundary>,
    )

    expect(screen.getByText('500')).toBeInTheDocument()
    expect(screen.getByText('Something went wrong.')).toBeInTheDocument()
    expect(
      screen.getByText(
        'Error occurred while rendering this page. Please try to refresh the page or contact support.',
      ),
    ).toBeInTheDocument()
    expect(screen.getByText('Refresh the page')).toBeInTheDocument()

    vi.restoreAllMocks() // Restore console.error after the test
  })
})
