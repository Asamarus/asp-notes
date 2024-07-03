import { render, screen, userEvent } from '@test-utils'
import { AccountsContext } from '@/providers/accountsProvider'
import ChangePasswordModal from './ChangePasswordModal'

function ChangePasswordModalWrapper({ mockChangePassword = vi.fn() }) {
  return (
    <AccountsContext.Provider
      value={{
        changePassword: mockChangePassword,
        isChangePasswordLoading: false,
        isLoginLoading: false,
        isLogoutLoading: false,
        login: vi.fn(),
        logout: vi.fn(),
        user: null,
      }}
    >
      <ChangePasswordModal />
    </AccountsContext.Provider>
  )
}

describe('ChangePasswordModal', () => {
  it('renders the password inputs and button', () => {
    render(<ChangePasswordModalWrapper />)

    expect(screen.getByLabelText(/current password/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/new password\s*\*$/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/new password confirmation/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Update password' })).toBeInTheDocument()
  })

  it('calls changePassword when the form is submitted', async () => {
    const mockChangePassword = vi.fn()
    render(<ChangePasswordModalWrapper mockChangePassword={mockChangePassword} />)

    await userEvent.type(screen.getByLabelText(/current password/i), 'currentPassword')
    await userEvent.type(screen.getByLabelText(/new password\s*\*$/i), 'newPassword')
    await userEvent.type(screen.getByLabelText(/new password confirmation/i), 'newPassword')

    await userEvent.click(screen.getByRole('button', { name: 'Update password' }))

    expect(mockChangePassword).toHaveBeenCalledWith('currentPassword', 'newPassword', 'newPassword')
  })

  it('displays validation error messages', async () => {
    render(<ChangePasswordModal />)

    // Try to submit the form without entering anything
    await userEvent.click(screen.getByRole('button', { name: /update password/i }))

    // Wait for the error messages to appear
    expect(await screen.findByText('Current password is required')).toBeInTheDocument()
    expect(await screen.findByText('New password is required')).toBeInTheDocument()
    expect(await screen.findByText('Password confirmation is required')).toBeInTheDocument()

    // Enter a password that's too short
    await userEvent.type(screen.getByLabelText(/current password\s*\*$/i), '123')
    await userEvent.type(screen.getByLabelText(/new password\s*\*$/i), '123')
    await userEvent.type(screen.getByLabelText(/new password confirmation\s*\*$/i), '123')

    // Try to submit the form again
    await userEvent.click(screen.getByRole('button', { name: /update password/i }))

    // Wait for the error messages to appear
    expect(await screen.findAllByText('Minimum password length is 6')).toHaveLength(3)

    await userEvent.type(screen.getByLabelText(/new password\s*\*$/i), '321')
    await userEvent.type(screen.getByLabelText(/new password confirmation\s*\*$/i), '123')

    expect(await screen.findByText('Passwords do not match')).toBeInTheDocument()
  })
})
