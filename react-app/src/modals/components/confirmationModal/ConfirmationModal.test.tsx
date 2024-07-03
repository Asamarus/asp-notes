import { render, screen, userEvent } from '@test-utils'
import ConfirmationModal from './ConfirmationModal'

describe('ConfirmationModal', () => {
  it('renders without crashing', () => {
    render(
      <ConfirmationModal
        onModalClose={() => {}}
        onConfirm={() => {}}
        message="Test message"
      />,
    )
  })

  it('displays the correct title and message', () => {
    render(
      <ConfirmationModal
        onModalClose={() => {}}
        onConfirm={() => {}}
        title="Test title"
        message="Test message"
      />,
    )

    expect(screen.getByText('Test title')).toBeInTheDocument()
    expect(screen.getByText('Test message')).toBeInTheDocument()
  })

  it('calls onConfirm and onModalClose when confirm button is clicked', async () => {
    const onConfirmMock = vi.fn()
    const onModalCloseMock = vi.fn()

    render(
      <ConfirmationModal
        onModalClose={onModalCloseMock}
        onConfirm={onConfirmMock}
        message="Test message"
      />,
    )

    await userEvent.click(screen.getByText('Confirm'))

    expect(onConfirmMock).toHaveBeenCalled()
    expect(onModalCloseMock).toHaveBeenCalled()
  })

  it('calls onCancel and onModalClose when cancel button is clicked', async () => {
    const onCancelMock = vi.fn()
    const onModalCloseMock = vi.fn()

    render(
      <ConfirmationModal
        onModalClose={onModalCloseMock}
        onCancel={onCancelMock}
        onConfirm={() => {}}
        message="Test message"
      />,
    )

    await userEvent.click(screen.getByText('Cancel'))

    expect(onCancelMock).toHaveBeenCalled()
    expect(onModalCloseMock).toHaveBeenCalled()
  })

  it('displays the correct cancel and confirm labels', () => {
    render(
      <ConfirmationModal
        onModalClose={() => {}}
        onConfirm={() => {}}
        message="Test message"
        cancelLabel="Test cancel"
        confirmLabel="Test confirm"
      />,
    )

    expect(screen.getByText('Test cancel')).toBeInTheDocument()
    expect(screen.getByText('Test confirm')).toBeInTheDocument()
  })

  it('does not display close button when showClose is false', () => {
    render(
      <ConfirmationModal
        onModalClose={() => {}}
        onConfirm={() => {}}
        message="Test message"
        showClose={false}
      />,
    )

    expect(screen.queryByLabelText('Close')).not.toBeInTheDocument()
  })

  it('displays close button when showClose is true', () => {
    render(
      <ConfirmationModal
        onModalClose={() => {}}
        onConfirm={() => {}}
        message="Test message"
        showClose={true}
      />,
    )

    expect(screen.getByLabelText('Close')).toBeInTheDocument()
  })
})
