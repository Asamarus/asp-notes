import { render, screen } from '@test-utils'
import LoadingModal from './LoadingModal'

describe('LoadingModal', () => {
  it('renders without crashing', () => {
    render(<LoadingModal />)
  })

  it('displays the correct message', () => {
    render(<LoadingModal message="Test loading message" />)

    expect(screen.getByText('Test loading message')).toBeInTheDocument()
  })

  it('displays default message when no message prop is provided', () => {
    render(<LoadingModal />)

    expect(screen.getByText('Loading...')).toBeInTheDocument()
  })

  it('adds and removes beforeunload event listener', () => {
    const addEventListenerSpy = vi.spyOn(window, 'addEventListener')
    const removeEventListenerSpy = vi.spyOn(window, 'removeEventListener')

    const { unmount } = render(<LoadingModal />)

    expect(addEventListenerSpy).toHaveBeenCalledWith('beforeunload', expect.any(Function))

    unmount()

    expect(removeEventListenerSpy).toHaveBeenCalledWith('beforeunload', expect.any(Function))
  })
})
