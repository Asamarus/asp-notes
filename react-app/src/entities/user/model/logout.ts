import createFetch from '@/shared/lib/createFetch'
import noop from '@/shared/lib/noop'
import { usersApi } from '..'

const logoutRequest = createFetch(usersApi.logout, noop)

function logout() {
  logoutRequest((data) => {
    if (data) {
      window.location.reload()
    }
  })
}

export default logout
