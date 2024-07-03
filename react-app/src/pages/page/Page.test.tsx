import { render, screen } from '@test-utils'
import Page from './Page'

describe('Page', () => {
  it('renders the title and divs', () => {
    render(<Page />)

    expect(screen.getByText('Page')).toBeInTheDocument()
    expect(screen.getByText('Top')).toBeInTheDocument()
    expect(screen.getByText('Bottom')).toBeInTheDocument()
  })

  it('renders a div with a specific height', () => {
    render(<Page />)

    const divElement = screen.getByText('Bottom').previousSibling
    expect(divElement).toHaveStyle('height: 1000px')
  })
})
