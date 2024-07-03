import { render, screen } from '@test-utils'

import Logo from './Logo'

describe('Logo Component', () => {
  it('renders without crashing', () => {
    render(<Logo />)
  })

  it('renders with correct link', () => {
    render(<Logo />)
    const linkElement = screen.getByRole('link')
    expect(linkElement).toHaveAttribute('href', '/')
  })

  it('renders with correct text', () => {
    render(<Logo />)
    const textElement = screen.getByText(/ProjectTemplate/i)
    expect(textElement).toBeInTheDocument()
  })
})
