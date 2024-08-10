import { useState, useCallback, useRef } from 'react'
import randomId from '@/utils/randomId'
import useIsMounted from '@/hooks/useIsMounted'

type GetData<T, E, Arg> = [Arg] extends [undefined]
  ? () => Promise<{ data: T | null; error: E | null }>
  : (arg: Arg) => Promise<{ data: T | null; error: E | null }>

type OnResult<T, E> = (result: { data: T | null; error: E | null }) => void

type RequestFunction<T, E, Arg> = [Arg] extends [undefined]
  ? (onResult?: OnResult<T, E>) => void
  : (arg: Arg, onResult?: OnResult<T, E>) => void

interface UseFetchOptions {
  concurrent?: boolean
  debounce?: boolean
  debounceTimeout?: number
  initialIsLoading?: boolean
}

function useFetch<T, E, Arg = undefined>(
  getData: GetData<T, E, Arg>,
  options: UseFetchOptions = {},
) {
  const {
    concurrent = false,
    debounce = false,
    debounceTimeout = 500,
    initialIsLoading = false,
  } = options

  const isMounted = useIsMounted()
  const activeRequestId = useRef('')
  const isLoadingRef = useRef(false)
  const [isLoading, setIsLoading] = useState(initialIsLoading)
  const debounceTimer = useRef<NodeJS.Timeout | null>(null)

  const request = useCallback(
    (...args: unknown[]) => {
      if (!concurrent && isLoadingRef.current) {
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
        isLoadingRef.current = true
        if (isMounted()) {
          setIsLoading(true)
        }

        activeRequestId.current = randomId()
        const currentRequestId = activeRequestId.current

        const result = await getData(typeof arg === 'undefined' ? (undefined as Arg) : arg)

        // If concurrent is true, only return result for the latest request
        if (concurrent && currentRequestId !== activeRequestId.current) {
          return
        }

        onResult?.(result)
        isLoadingRef.current = false
        if (isMounted()) {
          setIsLoading(false)
        }
      }

      if (debounce) {
        if (debounceTimer.current) {
          clearTimeout(debounceTimer.current)
        }

        debounceTimer.current = setTimeout(executeRequest, debounceTimeout)
      } else {
        executeRequest()
      }
    },
    [getData, concurrent, debounce, debounceTimeout, isMounted],
  )

  return {
    request: request as RequestFunction<T, E, Arg>,
    isLoading,
  }
}

export default useFetch
