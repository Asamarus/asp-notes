import { getClient } from './client'

import { components } from '@/misc/openapi'

export async function login(request: components['schemas']['LoginRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/accounts/login', {
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
    const { data, error } = await getClient().POST('/api/accounts/getUser')
    return { data, error }
  } catch (error) {
    console.error('accountsService.getUser', error)
    return { data: null, error: null }
  }
}

export async function logout() {
  try {
    const { data, error } = await getClient().POST('/api/accounts/logout')
    return { data, error }
  } catch (error) {
    console.error('accountsService.logout', error)
    return { data: null, error: null }
  }
}

export async function changePassword(request: components['schemas']['ChangePasswordRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/accounts/changePassword', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('accountsService.changePassword', error)
    return { data: null, error: null }
  }
}
