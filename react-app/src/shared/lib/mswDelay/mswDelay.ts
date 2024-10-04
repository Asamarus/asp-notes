import { delay } from 'msw'

export async function mswDelay() {
  if (import.meta.env.MODE === 'test') {
    return Promise.resolve()
  } else {
    return await delay()
  }
}
