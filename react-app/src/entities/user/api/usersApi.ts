import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function login(request: components['schemas']['UsersLoginRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/users/login', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('accountsService.login', error)
    return { data: null, error: null }
  }
}

export async function getUser() {
  try {
    const { data, error } = await getClient().GET('/api/users')
    return { data, error }
  } catch (error) {
    console.error('usersApi.getUser', error)
    return { data: null, error: null }
  }
}

export async function logout() {
  try {
    const { data, error } = await getClient().POST('/api/users/logout')
    return { data, error }
  } catch (error) {
    console.error('usersApi.logout', error)
    return { data: null, error: null }
  }
}

export async function changePassword(request: components['schemas']['UsersChangePasswordRequest']) {
  try {
    const { data, error } = await getClient().PUT('/api/users/password', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('usersApi.changePassword', error)
    return { data: null, error: null }
  }
}
