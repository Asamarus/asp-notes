import { render, screen, userEvent } from '@test-utils'
import { AccountsContext } from '@/providers/accountsProvider'
import LoginModal from './LoginModal'

function LoginModalWrapper({ mockLogin = vi.fn() }) {
  return (
    <AccountsContext.Provider
      value={{
        changePassword: vi.fn(),
        isChangePasswordLoading: false,
        isLoginLoading: false,
        isLogoutLoading: false,
        login: mockLogin,
        logout: vi.fn(),
        user: null,
      }}
    >
      <LoginModal />
    </AccountsContext.Provider>
  )
}

describe('LoginModal', () => {
  it('renders the password inputs and button', () => {
    render(<LoginModalWrapper />)

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Login' })).toBeInTheDocument()
  })

  it('calls login when the form is submitted', async () => {
    const mockLogin = vi.fn()
    render(<LoginModalWrapper mockLogin={mockLogin} />)

    await userEvent.type(screen.getByLabelText(/email/i), 'user@mail.com')
    await userEvent.type(screen.getByLabelText(/password/i), '123456')

    await userEvent.click(screen.getByRole('button', { name: 'Login' }))

    expect(mockLogin).toHaveBeenCalledWith('user@mail.com', '123456')
  })

  it('displays validation error messages', async () => {
    render(<LoginModal />)

    // Try to submit the form without entering anything
    await userEvent.click(screen.getByRole('button', { name: 'Login' }))

    // Wait for the error messages to appear
    expect(await screen.findByText('Email is required')).toBeInTheDocument()
    expect(await screen.findByText('Current password is required')).toBeInTheDocument()

    // Enter an invalid email
    await userEvent.type(screen.getByLabelText(/email/i), 'user')

    // Enter a password that's too short
    await userEvent.type(screen.getByLabelText(/password/i), '123')

    // Try to submit the form again
    await userEvent.click(screen.getByRole('button', { name: 'Login' }))

    // Wait for the error messages to appear
    expect(await screen.findByText('Invalid email')).toBeInTheDocument()
    expect(await screen.findByText('Minimum password length is 6')).toBeInTheDocument()
  })
})
