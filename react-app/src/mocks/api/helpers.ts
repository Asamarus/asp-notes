import { delay as mswDelay } from 'msw'

export async function delay() {
  if (import.meta.env.MODE === 'test') {
    return Promise.resolve()
  } else {
    return await mswDelay()
  }
}
