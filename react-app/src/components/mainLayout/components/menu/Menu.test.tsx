import { render, screen, userEvent, cleanup } from '@test-utils'

import Menu from './Menu'

describe('Menu', () => {
  it('renders without crashing', () => {
    render(<Menu close={() => {}} />)
  })

  it('calls close function when a link is clicked', async () => {
    const closeMock = vi.fn()
    render(<Menu close={closeMock} />)

    const linkElement = screen.getByText(/Home/i)
    await userEvent.click(linkElement)

    expect(closeMock).toHaveBeenCalled()
  })

  it('sets the active link correctly', () => {
    render(<Menu close={() => {}} />)

    let linkElement = screen.getByText(/Home/i)
    expect(linkElement.closest('[data-active="true"]')).toBeInTheDocument()

    cleanup()

    render(<Menu close={() => {}} />, { initialEntries: ['/dashboard'] })

    linkElement = screen.getByText(/Dashboard/i)
    expect(linkElement.closest('[data-active="true"]')).toBeInTheDocument()
  })
})
