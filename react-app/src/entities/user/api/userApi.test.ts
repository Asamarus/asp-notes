import { login, getUser, logout, changePassword } from './usersApi'
import * as usersMocks from './usersApiMockData'

describe('usersApi', () => {
  it('should login a user', async () => {
    const { data } = await login(usersMocks.loginRequestMock)
    expect(data).toEqual(usersMocks.loginResponseMock)
  })

  it('should get user', async () => {
    const { data } = await getUser()
    expect(data).toEqual(usersMocks.getUserResponseMock)
  })

  it('should logout a user', async () => {
    const { data } = await logout()
    expect(data).toEqual(null)
  })

  it('should change password', async () => {
    const { data } = await changePassword(usersMocks.changePasswordRequestMock)
    expect(data).toEqual(undefined)
  })
})
