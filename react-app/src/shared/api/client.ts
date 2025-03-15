import createClient, { Middleware } from 'openapi-fetch'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { events } from '@/shared/config'
import { showError, showSuccess } from '@/shared/lib/notifications'
import get from 'lodash/get'

import type { paths } from '@/shared/api'

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
      dispatchCustomEvent(events.user.unAuthorized)
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

        if (
          jsonBody.errors &&
          typeof jsonBody.errors === 'object' &&
          Object.keys(jsonBody.errors).length > 0
        ) {
          Object.keys(jsonBody.errors).forEach((key) => {
            const errorArray = jsonBody.errors[key]
            if (Array.isArray(errorArray)) {
              const errorMessages = errorArray
                .filter((error: unknown) => typeof error === 'string')
                .join('\n')
              if (errorMessages) {
                showError(errorMessages)
              }
            }
          })
        } else if (jsonBody.title) {
          showError(jsonBody.title)
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
        get(jsonBody, 'showNotification', false)
      ) {
        showSuccess(jsonBody.message)
      }
    }

    return response
  },
}

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
