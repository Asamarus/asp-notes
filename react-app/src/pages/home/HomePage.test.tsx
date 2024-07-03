import { render, screen, userEvent } from '@test-utils'
import HomePage from './HomePage'

describe('HomePage', () => {
  it('renders the title and text', () => {
    render(<HomePage />)

    expect(screen.getByText('Home page')).toBeInTheDocument()
    expect(screen.getByText('This is home page')).toBeInTheDocument()
  })

  it('logs "Test" when the button is clicked', async () => {
    const consoleSpy = vi.spyOn(console, 'log')
    render(<HomePage />)

    await userEvent.click(screen.getByText('Test'))

    expect(consoleSpy).toHaveBeenCalledWith('Test')
  })
})
