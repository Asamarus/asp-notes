import createClient, { Middleware } from 'openapi-fetch'
import { dispatch } from '@/hooks/useCustomEventListener'
import events from '@/events'
import type { paths } from '@/misc/openapi'
import { showError, showSuccess } from '@/helpers/notifications'
import get from 'lodash/get'

async function getBodyAsJson(res: Response) {
  let json = null
  try {
    const clone = res.clone()
    const body = await clone.text()
    json = JSON.parse(body)
  } catch (error) {
    console.error('getBodyAsJson', error)
    // The body is not a valid JSON object
  }

  return json
}

const unAuthorizedMiddleWare: Middleware = {
  async onResponse({ response }) {
    const { status } = response
    if (status === 401) {
      dispatch(events.user.unAuthorized)
    }

    return response
  },
}

const errorHandlingMiddleWare: Middleware = {
  async onResponse({ response }) {
    const { status, body } = response
    if ((status === 400 || status === 500) && body !== null) {
      const jsonBody = await getBodyAsJson(response)

      if (jsonBody !== null) {
        if (
          jsonBody.message &&
          typeof jsonBody.message === 'string' &&
          get(jsonBody, 'showNotification', true)
        ) {
          showError(jsonBody.message)
        }

        if (jsonBody.errors && Array.isArray(jsonBody.errors)) {
          jsonBody.errors.forEach((error: unknown) => {
            if (error && typeof error === 'string') {
              showError(error)
            }
          })
        }
      }
    }

    return response
  },
}

const successHandlingMiddleWare: Middleware = {
  async onResponse({ response }) {
    const { status, body } = response
    if (status === 200 && body !== null) {
      const jsonBody = await getBodyAsJson(response)
      if (
        jsonBody.message &&
        typeof jsonBody.message === 'string' &&
        get(jsonBody, 'showNotification', true)
      ) {
        showSuccess(jsonBody.message)
      }
    }

    return response
  },
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
let client: ReturnType<typeof createClient<paths>> | null = null

export function getClient() {
  if (!client) {
    const baseUrl = import.meta.env.MODE === 'test' ? 'http://localhost:3000' : '/'

    client = createClient<paths>({
      baseUrl: baseUrl,
    })

    client.use(unAuthorizedMiddleWare)
    client.use(errorHandlingMiddleWare)
    client.use(successHandlingMiddleWare)
  }

  return client
}
