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
    // The body is not a valid JSON object
  }

  return json
}

const unAuthorizedMiddleWare: Middleware = {
  async onResponse(res) {
    const { status } = res
    if (status === 401) {
      dispatch(events.user.unAuthorized)
    }

    return res
  },
}

const errorHandlingMiddleWare: Middleware = {
  async onResponse(res) {
    const { status, body } = res
    if ((status === 400 || status === 500) && body !== null) {
      const jsonBody = await getBodyAsJson(res)

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

    return res
  },
}

const successHandlingMiddleWare: Middleware = {
  async onResponse(res) {
    const { status, body } = res
    if (status === 200 && body !== null) {
      const jsonBody = await getBodyAsJson(res)
      if (
        jsonBody.message &&
        typeof jsonBody.message === 'string' &&
        get(jsonBody, 'showNotification', true)
      ) {
        showSuccess(jsonBody.message)
      }
    }

    return res
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
