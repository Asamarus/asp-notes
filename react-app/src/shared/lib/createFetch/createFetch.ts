import randomId from '@/shared/lib/randomId'

type GetData<T, E, Arg> = [Arg] extends [undefined]
  ? () => Promise<{ data: T | null; error: E | null }>
  : (arg: Arg) => Promise<{ data: T | null; error: E | null }>

type OnResult<T, E> = (result: { data: T | null; error: E | null }) => void

type RequestFunction<T, E, Arg> = [Arg] extends [undefined]
  ? (onResult?: OnResult<T, E>) => void
  : (arg: Arg, onResult?: OnResult<T, E>) => void

interface FetchOptions {
  concurrent?: boolean
  debounce?: boolean
  debounceTimeout?: number
}

function createFetch<T, E, Arg = undefined>(
  getData: GetData<T, E, Arg>,
  onLoadingChange: (isLoading: boolean) => void,
  options: FetchOptions = {},
) {
  const { concurrent = false, debounce = false, debounceTimeout = 500 } = options

  let activeRequestId = ''
  let isLoading = false
  let debounceTimer: NodeJS.Timeout | null = null

  const request = (...args: unknown[]) => {
    if (!concurrent && isLoading) {
      return // Ignore concurrent requests if not allowed
    }

    let arg = undefined
    let onResult = undefined
    if (args.length === 1) {
      if (args[0] instanceof Function) {
        onResult = args[0] as OnResult<T, E>
      } else {
        arg = args[0] as Arg
      }
    } else if (args.length === 2) {
      arg = args[0] as Arg
      onResult = args[1] as OnResult<T, E>
    }

    const executeRequest = async () => {
      isLoading = true
      onLoadingChange(true)

      activeRequestId = randomId()
      const currentRequestId = activeRequestId

      const result = await getData(typeof arg === 'undefined' ? (undefined as Arg) : arg)

      // If concurrent is true, only return result for the latest request
      if (concurrent && currentRequestId !== activeRequestId) {
        return
      }

      onResult?.(result)
      isLoading = false
      onLoadingChange(false)
    }

    if (debounce) {
      if (debounceTimer) {
        clearTimeout(debounceTimer)
      }

      debounceTimer = setTimeout(executeRequest, debounceTimeout)
    } else {
      executeRequest()
    }
  }

  return request as RequestFunction<T, E, Arg>
}

export default createFetch
