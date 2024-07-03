import { render, screen } from '@test-utils'
import NotFoundPage from './NotFoundPage'

describe('NotFoundPage', () => {
  it('renders the title, text, and button', () => {
    render(<NotFoundPage />)

    expect(screen.getByText('Nothing to see here')).toBeInTheDocument()
    expect(screen.getByText(/Page you are trying to open does not exist./i)).toBeInTheDocument()
    expect(screen.getByText('Take me back to home page')).toBeInTheDocument()
    expect(screen.getByText('Take me back to home page').closest('a')).toHaveAttribute('href', '/')
  })
})
