import { render, screen, userEvent, waitFor } from '@test-utils'
import { AccountsContext } from '@/providers/accountsProvider'
import UserInfo from './UserInfo'
import { accountsMocks } from '@/mocks/api/data'

const mockUser = accountsMocks.mockUser

vi.mock('@/modals')

describe('UserInfo', () => {
  afterAll(() => {
    vi.restoreAllMocks()
  })
  it('displays user email when user is provided', async () => {
    render(
      <AccountsContext.Provider
        value={{
          user: mockUser,
          logout: () => {},
          isLoginLoading: false,
          isChangePasswordLoading: false,
          isLogoutLoading: false,
          login: () => {},
          changePassword: () => {},
        }}
      >
        <UserInfo />
      </AccountsContext.Provider>,
    )

    await userEvent.click(screen.getByLabelText('User account'))

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()
  })

  it('opens the change password modal when the change password option is clicked', async () => {
    const { openChangePasswordModal } = await import('@/modals')

    render(
      <AccountsContext.Provider
        value={{
          user: mockUser,
          logout: () => {},
          isLoginLoading: false,
          isChangePasswordLoading: false,
          isLogoutLoading: false,
          login: () => {},
          changePassword: () => {},
        }}
      >
        <UserInfo />
      </AccountsContext.Provider>,
    )

    await userEvent.click(screen.getByLabelText('User account'))

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    await userEvent.click(screen.getByText('Change password'))

    await waitFor(() => expect(openChangePasswordModal).toHaveBeenCalled())
  })

  it('triggers logout when the logout option is clicked', async () => {
    const logoutMock = vi.fn()
    render(
      <AccountsContext.Provider
        value={{
          user: mockUser,
          logout: logoutMock,
          isLoginLoading: false,
          isChangePasswordLoading: false,
          isLogoutLoading: false,
          login: () => {},
          changePassword: () => {},
        }}
      >
        <UserInfo />
      </AccountsContext.Provider>,
    )

    await userEvent.click(screen.getByLabelText('User account'))

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    await userEvent.click(screen.getByText('Logout'))

    await waitFor(() => expect(logoutMock).toHaveBeenCalled())
  })
})
