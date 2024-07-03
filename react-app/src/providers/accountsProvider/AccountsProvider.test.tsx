import { render, screen, waitFor, userEvent, dispatchStorageEvent } from '@test-utils'
import { AccountsContext } from './index'
import AccountsProvider from './AccountsProvider'
import { useContext } from 'react'
import { HttpResponse, http } from 'msw'
import events from '@/events'
import { accountsMocks } from '@/mocks/api/data'
import { server } from '@/mocks/api/server'

const mockUser = accountsMocks.mockUser

function TestUserInfo() {
  const { user } = useContext(AccountsContext)
  return (
    <div data-testid="user-provider">
      <div>{user?.id}</div>
      <div>{user?.email}</div>
    </div>
  )
}

vi.mock('@/modals')

describe('AccountsProvider', () => {
  afterAll(() => {
    vi.restoreAllMocks()
  })

  it('should display loading initially', async () => {
    render(<AccountsProvider />)

    expect(await screen.findByLabelText('Loading')).toBeInTheDocument()
  })

  it('should render AccountsContext.Provider with correct state on successful fetch', async () => {
    render(
      <AccountsProvider>
        <TestUserInfo />
      </AccountsProvider>,
    )

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()
    expect(await screen.findByText(mockUser.id)).toBeInTheDocument()
  })

  it('should still display loading on unsuccessful fetch', async () => {
    server.use(
      http.post('/api/Accounts/getUser', () => {
        return new HttpResponse('Test error', { status: 500 })
      }),
    )

    render(
      <AccountsProvider>
        <TestUserInfo />
      </AccountsProvider>,
    )

    await waitFor(() => {
      expect(screen.getByLabelText('Loading')).toBeInTheDocument()
    })
  })

  it('should display login modal when user is not authenticated', async () => {
    server.use(
      http.post('/api/Accounts/getUser', () => {
        return new HttpResponse(null, { status: 401 })
      }),
    )

    render(
      <AccountsProvider>
        <TestUserInfo />
      </AccountsProvider>,
    )

    expect(await screen.findAllByText('Login')).toHaveLength(2)
  })

  it('should update the AccountsContext and render the user email when the login callback is triggered', async () => {
    const testUserEmail = 'test@test.com'

    function TestConsumer() {
      const { login } = useContext(AccountsContext)
      return (
        <>
          <button onClick={() => login(testUserEmail, 'password')}>Login</button>
          <TestUserInfo />
        </>
      )
    }

    render(
      <AccountsProvider>
        <TestConsumer />
      </AccountsProvider>,
    )

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    server.use(
      http.post('/api/Accounts/login', () => {
        return HttpResponse.json({ id: 2, email: 'test@test.com' })
      }),
    )

    await userEvent.click(screen.getByText('Login'))

    expect(await screen.findByText(testUserEmail)).toBeInTheDocument()
  })

  it('should reload page when logout callback is triggered', async () => {
    const original = window.location
    const mockReload = vi.fn()

    Object.defineProperty(window, 'location', {
      configurable: true,
      value: { reload: mockReload },
    })

    function TestConsumer() {
      const { logout } = useContext(AccountsContext)
      return (
        <>
          <button onClick={logout}>Logout</button>
          <TestUserInfo />
        </>
      )
    }

    render(
      <AccountsProvider>
        <TestConsumer />
      </AccountsProvider>,
    )

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    await userEvent.click(screen.getByText('Logout'))

    await waitFor(() => expect(mockReload).toHaveBeenCalled())

    // Restore the original function after the test is done
    Object.defineProperty(window, 'location', { configurable: true, value: original })
  })

  it('should close ChangePasswordModal when logout changePassword is triggered', async () => {
    const { closeChangePasswordModal } = await import('@/modals')

    function TestConsumer() {
      const { changePassword } = useContext(AccountsContext)
      return (
        <>
          <button
            onClick={() => {
              changePassword('currentPassword', 'newPassword', 'passwordRepeat')
            }}
          >
            changePassword
          </button>
          <TestUserInfo />
        </>
      )
    }

    render(
      <AccountsProvider>
        <TestConsumer />
      </AccountsProvider>,
    )

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    await userEvent.click(screen.getByText('changePassword'))

    await waitFor(() => expect(closeChangePasswordModal).toHaveBeenCalled())
  })

  it('should show login modal when events.user.unAuthorized cross tab event is triggered', async () => {
    const { openLoginModal } = await import('@/modals')

    render(
      <AccountsProvider>
        <TestUserInfo />
      </AccountsProvider>,
    )

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    dispatchStorageEvent(events.user.unAuthorized, null)

    await waitFor(() => expect(openLoginModal).toHaveBeenCalled())
  })

  it('should close login modal and update AccountsContext when events.user.loggedIn cross tab event is triggered', async () => {
    const { closeLoginModal } = await import('@/modals')
    const testUser = { id: 2, email: 'test@mail.com' }

    render(
      <AccountsProvider>
        <TestUserInfo />
      </AccountsProvider>,
    )

    expect(await screen.findByText(mockUser.email)).toBeInTheDocument()

    dispatchStorageEvent(events.user.loggedIn, { user: testUser })

    await waitFor(() => expect(closeLoginModal).toHaveBeenCalled())

    expect(await screen.findByText(testUser.email)).toBeInTheDocument()
  })
})
