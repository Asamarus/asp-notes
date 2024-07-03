import { render, screen, userEvent } from '@test-utils'
import ModalHeader from './ModalHeader'

describe('ModalHeader', () => {
  it('renders the children and close button by default', () => {
    render(<ModalHeader onClose={() => {}}>Test</ModalHeader>)

    expect(screen.getByText('Test')).toBeInTheDocument()
    expect(screen.getByRole('button')).toBeInTheDocument()
  })

  it('does not render the close button when showClose is false', () => {
    render(
      <ModalHeader
        showClose={false}
        onClose={() => {}}
      >
        Test
      </ModalHeader>,
    )

    expect(screen.getByText('Test')).toBeInTheDocument()
    expect(screen.queryByRole('button')).not.toBeInTheDocument()
  })

  it('calls onClose when the close button is clicked', async () => {
    const onClose = vi.fn()
    render(<ModalHeader onClose={onClose}>Test</ModalHeader>)

    await userEvent.click(screen.getByRole('button'))

    expect(onClose).toHaveBeenCalled()
  })
})
