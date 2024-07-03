import { login, getUser, logout, changePassword } from './accountsService'
import { accountsMocks } from '@/mocks/api/data'

describe('accountsService', () => {
  it('should login a user', async () => {
    const { data } = await login(accountsMocks.loginRequestMock)
    expect(data).toEqual(accountsMocks.loginResponseMock)
  })

  it('should get user', async () => {
    const { data } = await getUser()
    expect(data).toEqual(accountsMocks.getUserResponseMock)
  })

  it('should logout a user', async () => {
    const { data } = await logout()
    expect(data).toEqual(accountsMocks.logoutResponseMock)
  })

  it('should change password', async () => {
    const { data } = await changePassword(accountsMocks.changePasswordRequestMock)
    expect(data).toEqual(accountsMocks.changePasswordResponseMock)
  })
})
